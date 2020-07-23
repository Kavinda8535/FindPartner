using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FindPartner.API.Data;
using FindPartner.API.Dtos;
using FindPartner.API.Model;
//using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
//using System.Web;

namespace FindPartner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserforRegisterDto userforRegisterDto)
        {
            //Validate request
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            userforRegisterDto.UserName = userforRegisterDto.UserName.ToLower();
            if(await _repository.UserExists(userforRegisterDto.UserName))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                UserName = userforRegisterDto.UserName
            };

            var createdUser = await _repository.Register(userToCreate, userforRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userforLoginDto)
        {
            var userFroRepo = await _repository.Login(userforLoginDto.Username.ToLower(), userforLoginDto.Password);

            if(userFroRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFroRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFroRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
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