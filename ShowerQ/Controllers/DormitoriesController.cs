using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShowerQ.Models;
using ShowerQ.Models.Entities;

namespace ShowerQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DormitoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DormitoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Dormitories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dormitory>>> GetDormitories()
        {
            return await _context.Dormitories.ToListAsync();
        }

        // GET: api/Dormitories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dormitory>> GetDormitory(int id)
        {
            var dormitory = await _context.Dormitories.FindAsync(id);

            if (dormitory == null)
            {
                return NotFound();
            }

            return dormitory;
        }

        // PUT: api/Dormitories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDormitory(int id, Dormitory dormitory)
        {
            if (id != dormitory.Id)
            {
                return BadRequest();
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

            return NoContent();
        }

        // POST: api/Dormitories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Dormitory>> PostDormitory(Dormitory dormitory)
        {
            var schedule = CreateNewSchedule();

            dormitory.CurrentScheduleId = schedule.Id;
            dormitory.CurrentSchedule = schedule;

            _context.Dormitories.Add(dormitory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDormitory", new { id = dormitory.Id }, dormitory);
        }

        private Schedule CreateNewSchedule()
        {
            var schedule = new Schedule();
            schedule.TenantsPerInterval = 1;
            return _context.Schedules.Add(schedule).Entity;
        }

        // DELETE: api/Dormitories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dormitory>> DeleteDormitory(int id)
        {
            var dormitory = await _context.Dormitories.FindAsync(id);
            if (dormitory == null)
            {
                return NotFound();
            }

            _context.Dormitories.Remove(dormitory);
            await _context.SaveChangesAsync();

            return dormitory;
        }

        private bool DormitoryExists(int id)
        {
            return _context.Dormitories.Any(e => e.Id == id);
        }
    }
}
