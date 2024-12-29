using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using ClassLibrary1;
using System;

namespace lab11sem3api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly AppDbContext _context; 

        
        public RegionController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Regions>>> GetRegions()
        {
            var regions = await _context.Regions.ToListAsync(); 
            if (regions == null || regions.Count == 0) 
            {
                return NotFound("No regions found."); 
            }
            return Ok(regions); 
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Regions>> GetRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id); 
            if (region == null) 
            {
                return NotFound($"Region with ID {id} not found."); 
            }
            return Ok(region); 
        }

       
        [HttpPost]
        public async Task<ActionResult<Regions>> PostRegion(Regions region)
        {
            if (region == null) 
            {
                return BadRequest("Region data is null."); 
            }

            _context.Regions.Add(region); 
            await _context.SaveChangesAsync(); 
            return CreatedAtAction(nameof(GetRegion), new { id = region.RegionID }, region); 
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegion(int id, Regions region)
        {
            if (id != region.RegionID)
            {
                return BadRequest("ID mismatch."); 
            }

            _context.Entry(region).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); 
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!RegionExists(id)) 
                {
                    return NotFound($"Region with ID {id} not found."); 
                }
                else
                {
                    throw; 
                }
            }

            return NoContent(); 
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id); 
            if (region == null) 
            {
                return NotFound($"Region with ID {id} not found."); 
            }

            _context.Regions.Remove(region); 
            await _context.SaveChangesAsync(); 
            return NoContent();
        }

        
        private bool RegionExists(int id)
        {
            return _context.Regions.Any(e => e.RegionID == id);
        }
    }
}