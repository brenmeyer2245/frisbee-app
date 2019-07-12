using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using UFL.API.Services;
using UFL.API.Models;
using UFL.API.Models.DTOs;

namespace UFL.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration config, IAuthService _authService){
            _config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO user){
                user.Username = user.Username.ToLower();

                if (await _authService.UserExists(user.Username)){
                    return BadRequest("UserAlready Exists");
                }

                var userToCreate = new User {
                    Username = user.Username,
                };

                var createdUsers = await _authService.Register(userToCreate, user.Password);

                return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO submittedUser){
                 var user = await _authService.Login(submittedUser.Username.ToLower(), submittedUser.Password);

                 if (null == user) return Unauthorized();

                 var claims = new[]{
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                     new Claim(ClaimTypes.Name, user.Username)
                 };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings: Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new {
                    token = tokenHandler.WriteToken(token)
                });
        }



        }
}
