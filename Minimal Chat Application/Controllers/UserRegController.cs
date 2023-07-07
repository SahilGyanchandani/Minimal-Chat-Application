using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Chat_Application.Models;
using Minimal_Chat_Application.PasswordFunction;
using Minimal_Chat_Application.Response_Models;

namespace Minimal_Chat_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegController : ControllerBase
    {
        private readonly AppDbContext _Context;

        public UserRegController(AppDbContext dbContext)
        {
            _Context = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRegistration>>> GetUser()
        {
            if (_Context.UserRegistrations == null)
            {
                return NotFound();
            }
            return await _Context.UserRegistrations.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRegistration>> GetUserById(int id)
        {
            var user = await _Context.UserRegistrations.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
        [HttpPost]
        public async Task<ActionResult> AddUser(string Name, string Email,string Password, string PhoneNumber)
        {
            if(await _Context.UserRegistrations.AnyAsync(ur=> ur.Email==Email))
            {
                return Conflict(new { error = "Email is Already Registred" });
            }
            var userRegistration = new UserRegistration
            {
                Name = Name,
                Email = Email,
                Password = PasswordHash.HashPassword(Password),
                RegistrationDate = DateTime.Now.Date,
                PhoneNumber=PhoneNumber,
                IsActive=true
            };

            _Context.UserRegistrations.Add(userRegistration);
            await _Context.SaveChangesAsync();

            var response = new UserAddResponse
            {
                UserId = userRegistration.UserID,
                Name = userRegistration.Name,
                Email = userRegistration.Email
            };
            return Ok(response);
           
        }

       
        
                
        
    }
}
