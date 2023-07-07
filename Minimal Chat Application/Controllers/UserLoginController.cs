using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minimal_Chat_Application.Models;
//using Minimal_Chat_Application.Models.UserRegistration;
using Minimal_Chat_Application.PasswordFunction;
using Org.BouncyCastle.Utilities;

using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace Minimal_Chat_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly AppDbContext _Context;
        private readonly IConfiguration _Configuration;

        public UserLoginController(AppDbContext dbContext, IConfiguration configuration)
        {
            _Context = dbContext;
            _Configuration = configuration;
        }

        //[HttpGet(Name ="GetMe"),Authorize]
        //public ActionResult<string> getme()
        //{
        //    return Ok("Successfull");
        //}
        

        [HttpPost]
        public async Task<IActionResult> userlogin(string Email, string Password)
        {
            if (Email == null && Password == null)
            {
                return BadRequest();
            }
            var user = await _Context.UserRegistrations.FirstOrDefaultAsync(ul => ul.Email == Email);


            if (user == null)
            {
                return NotFound("User Not Found");
            }

            if (!PasswordHash.VerifyPassword(Password, user.Password))
            {
                return BadRequest("Password is Incorrect");
            }

            string token = CreateToken(user);
            return Ok(token);
        }
        private string CreateToken(UserRegistration user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
                 //new Claim("UserId", user.UserID.ToString())
                //new Claim(ClaimTypes.Role,"User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_Configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

            //private string CreateToken(UserRegistration user)
            //{
            //    var jwtTokenHandler = new JwtSecurityTokenHandler();
            //    var key = Encoding.ASCII.GetBytes("zaxcnjsbndjcfhysecb#$54$^^&%$#sdcvnsjkdnczm,xnkcjhasdfkacxnnsjfhcnzxc");
            //    var identity = new ClaimsIdentity(new Claim[]
            //    {

            //          new Claim("UserId", user.UserID.ToString()),
            //           new Claim(ClaimTypes.Name,$"{user.Name}")
            //    });

            //    var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

            //    var tokenDescriptor = new SecurityTokenDescriptor
            //    {
            //        Subject = identity,
            //        Expires = DateTime.Now.AddDays(1),
            //        SigningCredentials = credentials
            //    };
            //    var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            //    return jwtTokenHandler.WriteToken(token);
            //}

        
    }
}

