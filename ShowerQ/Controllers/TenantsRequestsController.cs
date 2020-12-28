using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShowerQ.Models;
using ShowerQ.Models.Entities;
using ShowerQ.Models.Entities.Validators;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowerQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "OAuth", Roles = "Tenant")]
    public class TenantsRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantsRequestsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        // GET api/<TenantsRequestsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            var request = await _dbContext.TenantsRequests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return new 
            {
                id = request.Id,
                tenantId = request.TenantId,
                intervalId = request.IntervalId,
                date = request.Date
            };

        }

        // POST api/<TenantsRequestsController>
        [HttpPost]
        public async Task<ActionResult<object>> Post([FromBody] TenantsRequest tenantsRequest)
        {
            var userDataClaim = GetCurrentUserIdClaim();

            if (userDataClaim is null)
            {
                return NotFound();
            }

            tenantsRequest.TenantId = userDataClaim.Value;

            TenantsRequestValidator validator = new(_dbContext);

            var result = validator.Validate(tenantsRequest);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            _dbContext.TenantsRequests.Add(tenantsRequest);
            await _dbContext.SaveChangesAsync();

            return StatusCode(201, new { id = tenantsRequest.Id });
        }

        // DELETE api/<TenantsRequestsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _dbContext.TenantsRequests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            var userDataClaim = GetCurrentUserIdClaim();

            if (userDataClaim is null)
            {
                return NotFound();
            }

            var tenantId = userDataClaim.Value;

            if(tenantId != request.TenantId)
            {
                return Unauthorized();
            }

            _dbContext.TenantsRequests.Remove(request);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        private Claim? GetCurrentUserIdClaim()
        {
            return User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.UserData));
        }
    }
}
