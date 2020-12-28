using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowerQ.Models;
using ShowerQ.Models.Entities;
using ShowerQ.Models.Entities.Validators;

namespace ShowerQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "OAuth")]
    [Authorize(AuthenticationSchemes = "OAuth")]
    public class DormitoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DormitoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Dormitories
        [HttpGet]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<IEnumerable<object>>> GetDormitories()
        {
            return _context.Dormitories
                .Select(
                dormitory => new
                {
                    id = dormitory.Id,
                    address = dormitory.Address,
                    universityId = dormitory.UniversityId
                }).ToArray();
        }

        // GET: api/Dormitories/5
        [HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = "OAuth", Roles = "DormitoryAdministrator")]
        [Authorize(Roles = "SystemAdministrator, DormitoryAdministrator")]
        public async Task<ActionResult<object>> GetDormitory(int id)
        {
            var dormitory = await _context.Dormitories.FindAsync(id);

            if (dormitory == null)
            {
                return NotFound();
            }

            var tenants = dormitory.Tenants.Select(tenant => tenant.Id).ToArray();

            var administrators = dormitory.Administrators.Select(administrator => administrator.Id).ToArray();

            return new { 
                id = id,
                address = dormitory.Address,
                universityId = dormitory.UniversityId,
                currentScheduleId = dormitory.CurrentScheduleId,
                tenants = tenants,
                administrators = administrators
            };
        }

        // PUT: api/Dormitories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        //[Authorize(AuthenticationSchemes = "OAuth", Roles = "DormitoryAdministrator")]
        [Authorize(Roles = "SystemAdministrator, DormitoryAdministrator")]
        public async Task<IActionResult> PutDormitory(int id, Dormitory dormitory)
        {
            if (id != dormitory.Id)
            {
                return BadRequest();
            }

            var claim = User.Claims.FirstOrDefault(cl => cl.Type.Equals("DormitoryId"));

            if(claim is default(Claim))
            {
                return StatusCode(500, new { error = "DormitoryId of current user not found." });
            }

            var dormitoryId = Convert.ToInt32(claim.Value);

            if(dormitoryId != id)
            {
                return Unauthorized();
            }

            DormitoryValidator validator = new(_context);

            var result = validator.Validate(dormitory);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            _context.Entry(dormitory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DormitoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Dormitories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<object>> PostDormitory(Dormitory dormitory)
        {
            var schedule = CreateNewSchedule();

            dormitory.CurrentScheduleId = schedule.Id;
            dormitory.CurrentSchedule = schedule;

            DormitoryValidator validator = new(_context);

            var result = validator.Validate(dormitory);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            _context.Dormitories.Add(dormitory);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { id = dormitory.Id });
        }

        private Schedule CreateNewSchedule()
        {
            var schedule = new Schedule();
            schedule.TenantsPerInterval = 1;
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
            return schedule;
        }

        // DELETE: api/Dormitories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "SystemAdministrator")]
        public async Task<ActionResult<Dormitory>> DeleteDormitory(int id)
        {
            var dormitory = await _context.Dormitories.FindAsync(id);

            if (dormitory == null)
            {
                return NotFound();
            }

            _context.Dormitories.Remove(dormitory);

            var schedule = _context.Schedules.FirstOrDefault(s => s.Id.Equals(dormitory.CurrentScheduleId));

            if(schedule is not null)
            {
                _context.Schedules.Remove(schedule);
            }
            
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool DormitoryExists(int id)
        {
            return _context.Dormitories.Any(e => e.Id == id);
        }
    }
}
