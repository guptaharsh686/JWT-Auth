using FormulaOneApp.Data;
using FormulaOneApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneApp.Controllers
{
    // all the routs to this controller will be authenticated through jwt bearer scheme
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [ApiController] // To make api behavior and functionalities available to us out of the box
    [Route("api/[controller]")] // api/teams dynamically configure route according to controller name
    //[Route("api/teams")] hard coding the route whatever be the name of the controller
    public class TeamsController : ControllerBase // Lightweight controller without view support
    {

        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        //[Route("GetBestTeam")] hard codeded endpoint name so api/Team/GetBestTeam
        // current api/Team
        public async Task<IActionResult> Get()
        {
            var teams = await _context.Teams.ToListAsync();

            return Ok(teams);
        }

        [HttpGet(template:"{id:int}")] // This http get is expecting an id parameter of type int
        public async Task<IActionResult> Get(int id) 
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == id);
            if(team == null)
            {
                return BadRequest("Bad Id");
            }
            return Ok(team);
        }


        [HttpPost]
        public async Task<IActionResult> Post(Team team)
        {
            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get",routeValues:team.Id,team);
            //For add item we return 201 created with a route to access that item
        }

        //http put for full fledged update 
        //http patch for single property change or partial update of the object

        [HttpPatch]
        public async Task<IActionResult> Patch(int id, string country)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(team => team.Id == id);

            if(team == null)
            {
                return BadRequest("Invalid Id");
            }

            team.Country = country;
            await _context.SaveChangesAsync();
            return NoContent();
            //With patch we return a 204 with no content

        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) 
        {
            var team = await  _context.Teams.FirstOrDefaultAsync(team => team.Id == id);

            if (team == null)
            {
                return BadRequest("Invalid Id");
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
            //With delete we return a 204 with no content

        }
    }
}
