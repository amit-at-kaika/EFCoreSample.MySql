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
using System.Security.Cryptography; //RNGCryptoServiceProvider

namespace EFCoreSample.MySql.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    readonly UserManager<IdentityUser> _userManager;
    readonly IConfiguration _configuration;
    readonly ApiDbContext _context;
    readonly TokenValidationParameters _tokenValidationParameters;

    public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration, ApiDbContext context, TokenValidationParameters tokenValidationParameters)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost]
    [Route("Request")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequest userRegistrationRequestDto)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(userRegistrationRequestDto.Mail);
            if (existingUser is not null)
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
                    return Ok(GenerateJwtToken(user));
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
        {
            return BadRequest(new AuthResults()
            {
                Result = false,
                Errors = new List<string>() { "Invalid Payload." }
            });
        }
    }

    [HttpPost]
    [Route("Login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequestDto)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _userManager.FindByEmailAsync(userLoginRequestDto.Mail);
            if (existingUser is null)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = new List<string>() { "Invalid Payload." }
                });
            }

            var validLogin = await _userManager.CheckPasswordAsync(existingUser, userLoginRequestDto.Password);
            if (!validLogin)
            {
                return BadRequest(new AuthResults()
                {
                    Result = false,
                    Errors = new List<string>() { "Invalid credentials." }
                });
            }

            return Ok(GenerateJwtToken(existingUser));
        }
        else
        {
            return BadRequest(new AuthResults()
            {
                Result = false,
                Errors = new List<string>() { "Invalid Payload." }
            });
        }
    }

    async Task<AuthResults> GenerateJwtToken(IdentityUser user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);
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
            Expires = DateTime.Now.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTime").Value!)),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = jwtTokenHandler.WriteToken(token);
        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            Token = GenerateRandomString(32), //TODO: Generate a refresh token
            AddedOn = DateTime.UtcNow,
            ExpiringOn = DateTime.UtcNow.AddMonths(6),
            IsRevoked = false,
            IsUsed = false,
            UserId = user.Id
        };

        CancellationToken cancellationToken = new CancellationToken();
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var retVal = new AuthResults()
        {
            Result = true,
            Token = serializedToken,
            RefreshToken = refreshToken.Token
        };

        return retVal;
    }

    string GenerateRandomString(int length)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
    }
}