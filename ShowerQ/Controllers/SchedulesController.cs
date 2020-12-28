using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowerQ.Models;
using ShowerQ.Models.Entities;
using ShowerQ.Models.Entities.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ShowerQ.Controllers
{
    /// <summary>
    /// POST and DELETE requests are not processing here
    /// because schedule create and delete actions executes in DormitoriesController.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "OAuth", Roles = "DormitoryAdministrator")]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<SchedulesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> Get()
        {
            return _context.Schedules;
        }

        // GET api/<SchedulesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return schedule;
        }

        // PUT api/<SchedulesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return BadRequest();
            }

            ScheduleValidator validator = new();

            var result = validator.Validate(schedule);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            var intervals = _context.Intervals.Where(i => i.ScheduleId.Equals(schedule.Id)).ToArray();

            _context.Intervals.RemoveRange(intervals);

            _context.Intervals.AddRange(schedule.Intervals);

            schedule.Intervals = null;

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

            return Ok();
        }
    }
}
