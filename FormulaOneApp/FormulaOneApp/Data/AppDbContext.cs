using FormulaOneApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormulaOneApp.Data
{
    //IdentityDbContext actually inherits from DbContext and added tables,classes and functions for providing authentication and authorization facilities 
    public class AppDbContext : IdentityDbContext 
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Team> Teams { get; set; }
    }
}
