using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecurityAPI.DataModels;
using SecurityAPI.Models;
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


        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService service)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.emailService = service;
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
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Client");
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

            if(user.EmailConfirmed == true)
            {
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Passname))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }

                return Unauthorized();

            }

            return BadRequest("Email Not Confirmed");
           
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
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
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
