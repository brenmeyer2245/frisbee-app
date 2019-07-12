using Microsoft.EntityFrameworkCore;
using UFL.API.Models;

namespace UFL.API.DB

{
    public class UFLDbContext : DbContext
    {
        public UFLDbContext(DbContextOptions<UFLDbContext> options) : base(options){

        }
       public DbSet<Team> Teams {get; set;}
       public DbSet<User> Users {get; set;}
    }
}
