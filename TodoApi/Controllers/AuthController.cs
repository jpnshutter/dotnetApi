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
            new Claim(ClaimTypes.Role, user.Role) // Ensure this is added
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        Audience = _configuration["Jwt:Audience"], // Ensure audience is set correctly
        Issuer = _configuration["Jwt:Issuer"], // Ensure issuer is set correctly
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
        public IActionResult GetUser([FromBody] User user){
            var result = _context.User
                .Where(x => x.Username == user.Username && x.Password == user.Password)
                .Select(x => new { x.Username, x.Password, x.Role })
                .FirstOrDefault();

            if (result != null)
            {
                // Retrieve the actual User object from the database
                var userObj = _context.User.FirstOrDefault(x => x.Username == user.Username && x.Password == user.Password);

                if (userObj != null)
                {
                    // Generate and return JWT token
                    var token = GenerateJwtToken(userObj);
                    return Ok(new { username =userObj.Username,token });
                }
            }
            return NotFound();
        }
        [HttpPost("Singup")]
        [AllowAnonymous]
        public IActionResult Save([FromBody] User user){
            _context.User.Add(user);
            _context.SaveChanges(); //for save state
            return Ok(user);
        }
        

        
    }
}