using JwtWebApiTutorial.Models;
using JwtWebApiTutorial.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtWebApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        public static User user = new User();
        private IConfiguration _config;
        private readonly IUserService userService;

        public AuthController(IConfiguration configuration,IUserService userService)
        {
            _config = configuration;
            this.userService = userService;
        }
        //[HttpGet,Authorize]
        //public ActionResult<Object> GetMe()
        //{
        //    //getting info from http context
        //    var username = User.Identity.Name;
        //    var username2 = User.FindFirstValue(ClaimTypes.Name);
        //    var role = User.FindFirstValue(ClaimTypes.Role);
        //    return Ok(new {username,username2,role});
        //} shortcut way

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            //getting info from http context which is alreader available beacause of ControllerBase
            //this only works with authorize attribute
            var username = userService.getMyName();
            //var username2 = User.FindFirstValue(ClaimTypes.Name);
            //var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(username);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            CreatePasswordHash(request.Password,out byte[] passHash,out byte[] passSalt);

            user.Username = request.Username;
            user.PasswordHash = passHash;
            user.PasswordSalt = passSalt;

            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if(user.Username != request.Username) 
            {
                return BadRequest("User not found");
            }

            if (!verifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Invalid Password");
            }

            string token = CreateToken(user);


            var refreshToken = generateRefreshToken();

            //set as a http only cookie
            setRefreshToken(refreshToken);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> GetNewJWTTokenWithRefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token");
            }
            else if(user.TokenExpires < DateTime.UtcNow)
            {
                return Unauthorized("Token Expired");
            }

            string token = CreateToken(user);

            var newRefreshToken = generateRefreshToken();
            setRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private void setRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires,
            };
            Response.Cookies.Append("refreshToken",refreshToken.Token, cookieOptions);

            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpires = refreshToken.Expires;

        }

        private RefreshToken generateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };

            return refreshToken;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,"Admin"),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("Appsetting:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool verifyPasswordHash(string password, byte[] passHash, byte[] passSalt)
        {
            using(var hmac = new HMACSHA512(passSalt)) 
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passHash);
            }
        }

        private void CreatePasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
