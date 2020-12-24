using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShowerQ.Models;
using ShowerQ.Models.Entities;
using ShowerQ.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowerQ.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "OAuth")]
    public class DormitoryAdministratorsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPhoneNumberFormatter _phoneNumberFormatter;
        private readonly string DormitoryAdministratorRoleName = "DormitoryAdministrator";

        public DormitoryAdministratorsController(
            ApplicationDbContext context, 
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

        [HttpGet]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<IEnumerable<IdentityUser>>> GetAllDormitoryAdministrators()
        {
            List<IdentityUser> administrators = new();

            foreach (var dormitory in _dbContext.Dormitories)
            {
                foreach (var admin in dormitory.Administrators)
                {
                    if (!administrators.Contains(admin))
                    {
                        administrators.Add(admin);
                    }
                }
            }

            return administrators;
        }

        [HttpGet]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<ICollection<IdentityUser>>> GetDormitoryAdministrators(int dormitoryId)
        {
            var dormitory = _dbContext.Dormitories.FirstOrDefault(d => d.Id.Equals(dormitoryId));

            if (dormitory is null)
            {
                return NotFound();
            }

            return Ok(dormitory.Administrators);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        [Authorize(AuthenticationSchemes = "OAuth", Roles = "DormitoryAdministrator, SystemAdministrator")]
        public async Task<IActionResult> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var userDataClaim = User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.UserData));

            if(userDataClaim is null)
            {
                return NotFound();
            }

            var user = _userManager.FindByIdAsync(userDataClaim.Value).Result;

            if (user is null)
            {
                return NotFound();
            }

            if (!_userManager.IsInRoleAsync(user, "SystemAdministrator").Result && !userId.Equals(user.Id)) // Only system administrator can change someone else's password.
            {
                return Unauthorized();
            }

            var result = _userManager.ChangePasswordAsync(user, currentPassword, newPassword).Result;

            if (!result.Succeeded)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return Ok();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<City>> Create(string phoneNumber, string password, int dormitoryId)
        {
            var dormitory = _dbContext.Dormitories.FirstOrDefault(d => d.Id.Equals(dormitoryId));

            if (dormitory is null)
            {
                return NotFound();
            }

            try
            {
                phoneNumber = _phoneNumberFormatter.ConvertToInternationalFormat(phoneNumber, _configuration["PhoneNumberRegion"]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            IdentityUser dormitoryAdministrator = new() 
            { 
                UserName = phoneNumber .Replace(" ", string.Empty),
                PhoneNumber = phoneNumber,
            };

            using var transaction = _dbContext.Database.BeginTransaction();

            var creationResult = await _userManager.CreateAsync(dormitoryAdministrator, password);

            if(!creationResult.Succeeded)
            {
                return StatusCode(500, new { errors = creationResult.Errors });
            }

            var role = await _roleManager.FindByNameAsync(DormitoryAdministratorRoleName);

            if (role is null)
            {
                return StatusCode(500, new { error = $"Role `{DormitoryAdministratorRoleName}` not found." });
            }

            var addToRoleResult = _userManager.AddToRoleAsync(dormitoryAdministrator, role.Name).Result;

            if (!addToRoleResult.Succeeded)
            {
                return StatusCode(500, new { errors = addToRoleResult.Errors });
            }

            dormitory.Administrators.Add(dormitoryAdministrator);

            try
            {
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return StatusCode(201, dormitoryAdministrator);
        }

        [HttpDelete]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<IActionResult> Delete(string id)
        {
            var admin = await _dbContext.Users.FindAsync(id);

            if (admin == null || !_userManager.IsInRoleAsync(admin, DormitoryAdministratorRoleName).Result)
            {
                return NotFound();
            }

            if (!_userManager.IsInRoleAsync(admin, "Tenant").Result &&
                !_userManager.IsInRoleAsync(admin, "SystemAdministrator").Result)
            {
                _dbContext.Users.Remove(admin);
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(admin, DormitoryAdministratorRoleName);
            }

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
