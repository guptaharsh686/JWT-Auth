using FormulaOneApp.Configurations;
using FormulaOneApp.Models;
using FormulaOneApp.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormulaOneApp.Controllers
{
    [Route(template: "api/[controller]")] // api/authentication
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //IdentityUSer is the default user class of Identity provided by asp.net core
        // Ad uw inherit out appDbContex from IdentityDbContext and we add AppDbContext to the DI 
        // All the Identity Related classes like UserMAnager are alredy added to the DI
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthenticationController(
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            //Initializing through DI
            _jwtConfig = optionsMonitor.CurrentValue;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterationRequestDto requestDto)
        {
            //Validate incomming request
            if(ModelState.IsValid) 
            {
                //We need to check if the email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDto.Email);
                if(user_exist != null)
                {
                    return BadRequest(new AuthResult
                    {
                        Result = false,
                        Errors = new List<string>
                        {
                            "Email already exist"
                        }
                    });
                }

                //Create a user
                var new_user = new IdentityUser()
                {
                    Email = requestDto.Email,
                    UserName = requestDto.Email,
                };

                var is_created = await _userManager.CreateAsync(new_user,requestDto.Password);

                if(is_created.Succeeded)
                {
                    //Generate Tokens
                    var token = GenerateToken(new_user);

                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }
                else
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Server Error"
                        }
                    });
                }

            }

            return BadRequest("Invalid Credentials");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            if (ModelState.IsValid)
            {
                //check if user exist
                var existing_user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
                if (existing_user == null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid PAyload"
                        }
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequestDto.Password);

                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Invalid credentials"
                        }
                    });
                }

                var jwtToken = GenerateToken(existing_user);

                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });

            }

            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string>()
                {
                    "Invalid PAyload"
                }
            });
        }


        private string GenerateToken(IdentityUser user) 
        {
            //Create a token handler responsible for generating a token
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Get the Security key
            //Converting key to array of bytes for encryption and decryption because we cannot use string directly for that
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            //Create a token descriptor
            //Allow us to define all the configuration ehat we need to put inside token
            //payload part of JWT Token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                //adding list of claims (info) which the token payload will contain
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString())
                }),
                //must keep some short time to expire
                Expires = DateTime.Now.AddHours(1), // adding long time for dev purposes
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256),

            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            //convert security token to actual text
            var jwtToken = jwtTokenHandler.WriteToken(token);


            return jwtToken;
        }

    }
}
