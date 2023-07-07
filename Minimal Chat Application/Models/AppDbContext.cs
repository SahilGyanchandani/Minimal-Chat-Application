using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Services;
using Minimal_Chat_Application.ParameterModels;

namespace Minimal_Chat_Application.Models
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 

        }
        public DbSet<UserRegistration> UserRegistrations { get; set; }
        public DbSet<Message> Messages { get; set; }


    }
}
