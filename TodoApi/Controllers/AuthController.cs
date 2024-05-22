using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi.DataAccess;
using TodoApi.Models;
namespace TodoApi.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase{
        private readonly AuthContext _context;
        private readonly IConfiguration _configuration;
        
        
        public AuthController(AuthContext context, IConfiguration configuration){
            _context =context;
            _configuration = configuration;
        }


        private string GenerateJwtToken(User user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role) 
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        Audience = _configuration["Jwt:Audience"], 
        Issuer = _configuration["Jwt:Issuer"], 
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

        [HttpPost("Signin")]
        [AllowAnonymous]
        public IActionResult GetUser([FromBody] Signin user){
            
                var userObj = _context.User.FirstOrDefault(x => x.Username == user.Username);
                if (userObj != null && BCrypt.Net.BCrypt.Verify(user.Password, userObj.Password))
                {
                    var token = GenerateJwtToken(userObj);
                    return Ok(new { username =userObj.Username,role=userObj.Role,token });
                }
            
            return NotFound();
        }
        [HttpPost("Singup")]
        [AllowAnonymous]
        public IActionResult Save([FromBody] Signup user){
            var newUser = new User
            {
                Username = user.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Role = user.Role
            };
            _context.User.Add(newUser);
            _context.SaveChanges(); //for save state
            return Ok(newUser);
        }
        

        
    }
}