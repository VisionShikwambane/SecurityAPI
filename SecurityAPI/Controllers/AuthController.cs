using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecurityAPI.DataModels;
using SecurityAPI.DBContext;
using SecurityAPI.Models;
using SecurityAPI.Repositories;
using SecurityAPI.Services;
using SecurityAPI.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SecurityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IRepository _repository;


        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService service, AppDbContext appDbContext, IRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.emailService = service;
            _appDbContext = appDbContext;
            _repository = repository;
        }


        [HttpPost]
        [Route("RegisterUser")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {


            string emailRegex = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$";

            if (!Regex.IsMatch(model.email, emailRegex))
            {
                return BadRequest("Invalid email");
            }
            var user = await _userManager.FindByIdAsync(model.email);

            if(user == null)
            {
                user = new AppUser
                {
                    UserName = model.email,
                    Email = model.email,
                    PhoneNumber = model.PhoneNumber,
                    Title = model.Title,
                    FirstName = model.Name,
                    LastName = model.Surname,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var patient = new Patient
                    {
                        UserID = user.Id
                    };

                    _repository.Add(patient);
                    await _repository.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(user, "Patient");
                    await SendConfirmEmailOTP(user.Email, "2234");

                }
                if (result.Errors.Any()) return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }

            else
            {
                return Forbid("Account already exists.");
            }

            return Ok();

        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                try
                {
                    var token = GenerateJWTToken((AppUser)user);
                    return Ok(new { token });
                }
                catch (Exception ex)
                {
                    // Log the specific exception details for troubleshooting
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the JWT token.");
                }
            }

            return Unauthorized("Invalid credentials");


        }

      
        private async Task<string> GenerateJWTToken(AppUser user)
        {
            var claims = await GetAllValidClaims(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = credentials,
                Issuer = _configuration["Tokens:Issuer"],
                Audience = _configuration["Tokens:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }






        private async Task<List<Claim>> GetAllValidClaims(AppUser user)
        {
            var _options = new IdentityOptions();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),

            };

            //Getting claims that we have assigned to the user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //Get user role and add it to claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }







        [HttpPost]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailVM model)
        {

            return Ok();

        }







        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Tokens:Issuer"],
                audience: _configuration["Tokens:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }





        private async Task<IActionResult> SendConfirmEmailOTP(string receiver, string code)
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = receiver;
                mailrequest.Subject = "Once Off OPT ";
                mailrequest.Body = GetHtmlcontent(code);
                await emailService.SendEmailAsync(mailrequest);

                return Ok();

            }
            catch (Exception ex)
            {
                throw;
            }
        }






        private string GetHtmlcontent(string code)
        {
            string response = "<p>The code expires in 5 minutes</p>";
            response += $"<h2>{code}</h2>";
            return response;
        }


    }
}
