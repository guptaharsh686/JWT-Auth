using FormulaOneApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormulaOneApp.Controllers
{
    [ApiController] // To make api behavior and functionalities available to us out of the box
    [Route("api/[controller]")] // api/teams dynamically configure route according to controller name
    //[Route("api/teams")] hard coding the route whatever be the name of the controller
    public class TeamsController : ControllerBase // Lightweight controller without view support
    {

        private static List<Team> teams = new List<Team>()
        {
            new Team
            {
                Id = 1,
                Name = "Marcedes",
                Country = "Germany",
                TeamPrinciple = "Toto Wolf"

            },
            new Team
            {
                Id= 2,
                Name = "Ferrari",
                Country = "Italy",
                TeamPrinciple = "Mattia Bonitto"
            },
            new Team
            {
                Id = 3,
                Name = "Alfa Romeo",
                Country = "Swiss",
                TeamPrinciple = "Fredric Vasseur"
            }
        };

        [HttpGet]
        //[Route("GetBestTeam")] hard codeded endpoint name so api/Team/GetBestTeam
        // current api/Team
        public IActionResult Get()
        {
            return Ok(teams);
        }

        [HttpGet(template:"{id:int}")] // This http get is expecting an id parameter of type int
        public IActionResult Get(int id) 
        {
            var team = teams.FirstOrDefault(x => x.Id == id);
            if(team == null)
            {
                return BadRequest("Bad Id");
            }
            return Ok(team);
        }


        [HttpPost]
        public IActionResult Post(Team team)
        {
            teams.Add(team);

            return CreatedAtAction("Get",routeValues:team.Id,team);
            //For add item we return 201 created with a route to access that item
        }

        //http put for full fledged update 
        //http patch for single property change or partial update of the object

        [HttpPatch]
        public IActionResult Patch(int id, string country)
        {
            var team = teams.FirstOrDefault(team => team.Id == id);

            if(team == null)
            {
                return BadRequest("Invalid Id");
            }

            team.Country = country;
            return NoContent();
            //With patch we return a 204 with no content

        }

        [HttpDelete]
        public IActionResult Delete(int id) 
        {
            var team = teams.FirstOrDefault(team => team.Id == id);

            if (team == null)
            {
                return BadRequest("Invalid Id");
            }

            teams.Remove(team);

            return NoContent();
            //With delete we return a 204 with no content

        }
    }
}
