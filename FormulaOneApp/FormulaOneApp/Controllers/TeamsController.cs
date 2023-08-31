using FormulaOneApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormulaOneApp.Controllers
{
    [ApiController] // To make api behavior and functionalities available to us out of the box
    [Route("api/[controller]")] // api/teams dynamically configure route according to controller name
    //[Route("api/teams")] hard coding the route whatever be the name of the controller
    public class TeamsController : ControllerBase // Lightweight controller without view support
    {

        private List<Teams> teams = new List<Teams>()
        {
            new Teams
            {
                Id = 1,
                Name = "Marcedes",
                Country = "Germany",
                TeamPrinciple = "Toto Wolf"

            },
            new Teams
            {
                Id= 2,
                Name = "Ferrari",
                Country = "Italy",
                TeamPrinciple = "Mattia Bonitto"
            },
            new Teams
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
    }
}
