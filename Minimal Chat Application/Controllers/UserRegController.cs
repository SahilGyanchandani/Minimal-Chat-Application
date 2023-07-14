using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal_Chat_Application.Models;
using Minimal_Chat_Application.ParameterModels;
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
        public async Task<ActionResult> AddUser([FromBody] UserReg reg)
        {
            if(await _Context.UserRegistrations.AnyAsync(ur=> ur.Email==reg.Email))
            {
                return Conflict(new { error = "Email is Already Registred" });
            }
            var userRegistration = new UserRegistration
            {
                Name = reg.Name,
                Email = reg.Email,
                Password = PasswordHash.HashPassword(reg.Password),
                RegistrationDate = DateTime.Now.Date,
                PhoneNumber=reg.PhoneNumber,
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
