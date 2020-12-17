using System.Collections.Generic;
using System.Linq;
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
    [Authorize(AuthenticationSchemes = "OAuth", Roles = "SystemAdministrator")]
    public class UniversitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UniversitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Universities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUniversities()
        {
            return _context.Universities
                .Select(
                university => new
                {
                    id = university.Id,
                    name = university.Name,
                    cityId = university.CityId,
                    dormitories = university.Dormitories.Select(d => d.Id)
                }).ToArray();
        }

        // GET: api/Universities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUniversity(int id)
        {
            var university = await _context.Universities.FindAsync(id);

            if (university == null)
            {
                return NotFound();
            }

            return new
            {
                id = university.Id,
                name = university.Name,
                cityId = university.CityId,
                dormitories = university.Dormitories.Select(d => d.Id)
            };
        }

        // PUT: api/Universities/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUniversity(int id, University university)
        {
            if (id != university.Id)
            {
                return BadRequest();
            }

            UniversityValidator validator = new(_context);

            var result = validator.Validate(university);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            _context.Entry(university).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UniversityExists(id))
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

        // POST: api/Universities
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<object>> PostUniversity(University university)
        {
            UniversityValidator validator = new(_context);

            var result = validator.Validate(university);

            if (!result.IsValid)
            {
                return StatusCode(500, new { errors = result.Errors });
            }

            _context.Universities.Add(university);
            await _context.SaveChangesAsync();

            return new
            {
                id = university.Id,
                name = university.Name,
                cityId = university.CityId
            };
        }

        // DELETE: api/Universities/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<University>> DeleteUniversity(int id)
        {
            var university = await _context.Universities.FindAsync(id);
            if (university == null)
            {
                return NotFound();
            }

            _context.Universities.Remove(university);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool UniversityExists(int id)
        {
            return _context.Universities.Any(e => e.Id == id);
        }
    }
}
