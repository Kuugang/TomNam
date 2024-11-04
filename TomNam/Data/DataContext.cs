using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TomNam.Models;

namespace TomNam.Data
{
    public class DataContext : IdentityDbContext<User>
        //should use custom Identity User
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
             
        }
    }
}