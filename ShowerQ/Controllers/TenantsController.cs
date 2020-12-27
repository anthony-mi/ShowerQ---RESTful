using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ShowerQ.Extensions;
using ShowerQ.Models;
using ShowerQ.Models.Entities.Users;
using ShowerQ.Models.Entities.Validators;
using ShowerQ.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowerQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "OAuth", Roles = "DormitoryAdministrator")]
    public class TenantsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPhoneNumberFormatter _phoneNumberFormatter;
        private readonly string TenantRoleName = "Tenant";

        public TenantsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IPhoneNumberFormatter phoneNumberFormatter)
        {
            _dbContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _phoneNumberFormatter = phoneNumberFormatter;
        }

        // GET: api/<TenantsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> Get()
        {
            var tenants = await _userManager.GetUsersInRoleAsync(TenantRoleName);
            return Ok(tenants);
        }

        // GET api/<TenantsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(string id)
        {
            var user = _dbContext.Users.FirstOrDefault(user => user.Id.Equals(id));
            var tenant = new Tenant(user);
            var claims = await _userManager.GetClaimsAsync(tenant);

            tenant.InitializeProperties(claims);

            if (tenant is null)
            {
                return NotFound();
            }

            if (!_userManager.IsInRoleAsync(tenant, TenantRoleName).Result)
            {
                return NotFound();
            }

            return Ok(
                new
                {
                    id = tenant.Id,
                    phoneNumber = tenant.PhoneNumber,
                    username = tenant.UserName,
                    firstName = tenant.FirstName,
                    lastName = tenant.LastName,
                    gender = tenant.Gender,
                    dormitoryId = tenant.DormitoryId,
                    room = tenant.Room,
                    priority = tenant.Priority
                });
        }

        // POST api/<TenantsController>
        [HttpPost]
        public async Task<ActionResult<Tenant>> Post([FromBody] JObject jObj)
        {
            var tenant = jObj.ToObject<Tenant>();
            tenant.DbContext = _dbContext;

            var password = jObj["Password"].Value<string>();

            try
            {
                tenant.PhoneNumber = _phoneNumberFormatter.ConvertToInternationalFormat(tenant.PhoneNumber, _configuration["PhoneNumberRegion"]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            tenant.UserName = tenant.PhoneNumber.Replace(" ", string.Empty);

            TenantValidator validator = new(_dbContext);

            var result = validator.Validate(tenant);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            tenant.Priority = CalculatePriority(tenant);

            using var transaction = _dbContext.Database.BeginTransaction();

            var creationResult = await _userManager.CreateAsync(tenant, password);

            if (!creationResult.Succeeded)
            {
                return StatusCode(500, new { errors = creationResult.Errors });
            }

            var role = await _roleManager.FindByNameAsync(TenantRoleName);

            if (role is null)
            {
                return StatusCode(500, new { error = $"Role `{TenantRoleName}` not found." });
            }

            var addToRoleResult = _userManager.AddToRoleAsync(tenant, role.Name).Result;

            if (!addToRoleResult.Succeeded)
            {
                return StatusCode(500, new { errors = addToRoleResult.Errors });
            }

            var claimsCreationResult = await _userManager.AddClaimsAsync(tenant, tenant.GenerateClaims());

            if (!claimsCreationResult.Succeeded)
            {
                return StatusCode(500, new { errors = claimsCreationResult.Errors });
            }

            tenant.Dormitory.Tenants.Add(tenant);

            try
            {
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return StatusCode(201, new { id = tenant.Id });
        }

        private uint CalculatePriority(Tenant tenant)
        {
            uint priority = 0;

            var dormitory = _dbContext.Dormitories.FirstOrDefault(d => d.Id.Equals(tenant.DormitoryId));

            if(dormitory is null)
            {
                return priority;
            }

            var dormitoryIdClaimType = GetPropertyName(() => tenant.DormitoryId);
            var genderClaimType = GetPropertyName(() => tenant.Gender);
            var priorityClaimType = GetPropertyName(() => tenant.Priority);

            var dormitoryTenants = _dbContext.UserClaims
                .Where(claim => claim.ClaimType.Equals(dormitoryIdClaimType) && claim.ClaimValue.Equals(tenant.DormitoryId.ToString()))
                .Select(claim => claim.UserId);

            if (dormitoryTenants.Count() == 0)
            {
                return priority;
            }

            var sameGenderTenants = _dbContext.UserClaims
                .Where(claim => claim.ClaimType.Equals(genderClaimType) &&
                claim.ClaimValue.Equals(tenant.Gender.ToString()) &&
                dormitoryTenants.Contains(claim.UserId))
                .Select(claim => claim.UserId);

            if(sameGenderTenants.Count() == 0)
            {
                return priority;
            }

            priority = Convert.ToUInt32(_dbContext.UserClaims
                .Where(claim => sameGenderTenants.Contains(claim.UserId) && claim.ClaimType.Equals(priorityClaimType))
                .Select(claim => Convert.ToInt32(claim.ClaimValue))
                .Average((el) => el));

            return priority;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }

        // PUT api/<TenantsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] TenantModel tenant)
        {
            if (id != tenant.Id)
            {
                return BadRequest();
            }

            TenantModelValidator validator = new(_dbContext);

            var result = validator.Validate(tenant);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            var user = _dbContext.Users.FirstOrDefault(User => User.Id.Equals(id));
            
            if(user is default(IdentityUser))
            {
                return NotFound();
            }

            user.Initialize(tenant);

            try
            {
                user.PhoneNumber = _phoneNumberFormatter.ConvertToInternationalFormat(user.PhoneNumber, _configuration["PhoneNumberRegion"]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            user.UserName = user.PhoneNumber.Replace(" ", string.Empty);

            using var transaction = _dbContext.Database.BeginTransaction();

            _dbContext.Entry(user).State = EntityState.Modified;

            await UpdateClaimsAsync(tenant, user);

            try
            {
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return Ok();
        }

        private async Task UpdateClaimsAsync(TenantModel tenant, IdentityUser user)
        {
            var ignoredProperties = typeof(IdentityUser).GetProperties().Select(p => p.Name);
            var newClaims = tenant.GenerateClaims(ignoredProperties);
            var oldClaims = _userManager.GetClaimsAsync(user).Result;

            foreach(var newClaim in newClaims)
            {
                var oldClaim = oldClaims.FirstOrDefault(c => c.Type.Equals(newClaim.Type));

                if(oldClaim is not default(Claim))
                {
                    await _userManager.RemoveClaimAsync(user, oldClaim);
                }

                await _userManager.AddClaimAsync(user, newClaim);
            }
        }

        // DELETE api/<TenantsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var tenant = await _dbContext.Users.FindAsync(id);

            if (tenant == null || !_userManager.IsInRoleAsync(tenant, TenantRoleName).Result)
            {
                return NotFound();
            }

            using var transaction = _dbContext.Database.BeginTransaction();

            await _userManager.RemoveFromRoleAsync(tenant, TenantRoleName);

            var claims = _dbContext.UserClaims.Where(cl => cl.UserId.Equals(id));

            _dbContext.UserClaims.RemoveRange(claims);

            if (!_userManager.IsInRoleAsync(tenant, "SystemAdministrator").Result &&
                !_userManager.IsInRoleAsync(tenant, "DormitoryAdministrator").Result)
            {
                _dbContext.Users.Remove(tenant);
            }

            await _dbContext.SaveChangesAsync();

            transaction.Commit();

            return Ok();
        }
    }
}
