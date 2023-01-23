using System.IdentityModel.Tokens.Jwt; //JwtSecurityTokenHandler
using System.Security.Claims; //ClaimsIdentity
using System.Text; //Encoding
using EFCoreSample.MySql.Config;
using EFCoreSample.MySql.Data;
using EFCoreSample.MySql.Models; //AuthResults
using EFCoreSample.MySql.Models.DTOs; //UserRegistrationRequest
using Microsoft.AspNetCore.Identity; //UserManager
using Microsoft.AspNetCore.Mvc; //ControllerBase
using Microsoft.IdentityModel.Tokens; //SecurityTokenDescriptor

namespace EFCoreSample.MySql.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    readonly UserManager<IdentityUser> _userManager;
    readonly IConfiguration _configuration;

    public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRegistrationRequestDto)
    {
        if (ModelState.IsValid)
        {
            var userExists = await _userManager.FindByEmailAsync(userRegistrationRequestDto.Mail);
            if (userExists is not null)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = new List<string>() { "Email already exits." }
                });
            }
            else
            {
                var user = new IdentityUser()
                {
                    Email = userRegistrationRequestDto.Mail,
                    UserName = userRegistrationRequestDto.Name
                };

                var userIsCreated = await _userManager.CreateAsync(user, userRegistrationRequestDto.Password);
                if (userIsCreated.Succeeded)
                {
                    var token = GenerateJwtToken(user);
                    return Ok(new AuthResults()
                    {
                        Result = true,
                        Token = token
                    });
                }
                else
                {
                    return BadRequest(new AuthResults()
                    {
                        Result = false,
                        Errors = new List<string>() { "Server error : New user could not be created." }
                    });
                }
            }
        }
        else
            return BadRequest();
    }

    string GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim ("id",user.Id),
                new Claim (JwtRegisteredClaimNames.Sub, user.Email),
                new Claim (JwtRegisteredClaimNames.Email,user.Email),
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            }),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = jwtTokenHandler.WriteToken(token);
        return serializedToken;
    }
}