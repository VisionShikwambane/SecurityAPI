using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecurityAPI.DataModels;
using SecurityAPI.DBContext;
using SecurityAPI.Models;
using SecurityAPI.Repositories;
using SecurityAPI.Services;
using SecurityAPI.ViewModels;
using System.Data;
using System.Text.RegularExpressions;

namespace SecurityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IRepository _repository;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService service, AppDbContext appDbContext, IRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.emailService = service;
            _appDbContext = appDbContext;
            _repository = repository;
        }


        [HttpPost]
        [Route("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] AddDoctorVM model)
        {


            string emailRegex = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$";

            if (!Regex.IsMatch(model.Email, emailRegex))
            {
                return BadRequest("Invalid email");
            }
            var user = await _userManager.FindByIdAsync(model.Email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Title = model.Title,
                    FirstName = model.Name,
                    LastName = model.Surname,
                    EmailConfirmed = false
                };

                string randomPassword = GenerateRandomPassword();

                var result = await _userManager.CreateAsync(user, randomPassword);

                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "Doctor");

                    var doctor = new Doctor
                    {
                        UserID = user.Id,
                        DoctorType = model.DocType
  
                    };

                    _appDbContext.Add(doctor);
                    _appDbContext.SaveChanges();

                    if(doctor!= null)
                    {
                       await SendWelcomeEmail(user.Email, randomPassword);
                    }


                    // await SendConfirmEmailOTP(user.Email, "2234");
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
        [Route("AddliveAnnoucement")]
        public async Task<ActionResult> AddliveAnnoucement(LiveAnnouncement liveAnnouncement)
        {
            try
            {
                _repository.Add(liveAnnouncement);
                await _repository.SaveChangesAsync();
                return Ok(liveAnnouncement);

            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }


        [HttpGet("GetliveAnnoucemenById/{id}")]
        public async Task<IActionResult> GetliveAnnoucementById(int id)
        {
            try
            {
                var la = await _repository.GetByIdAsync<LiveAnnouncement>(id);
                if (la == null)
                {
                    return NotFound();
                }
                return Ok(la);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving internal department");
            }
        }



        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllliveAnnoucements()
        {
            try
            {
                var results = await _repository.GetAllAsync<LiveAnnouncement>();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }



        [HttpDelete]
        [Route("DeleteliveAnnouncement/{id}")]
        public async Task<ActionResult> DeleteliveAnnouncement(int id)
        {
            try
            {
                var la = await _repository.GetByIdAsync<LiveAnnouncement>(id);

                if (la == null)
                    return NotFound();

                _repository.Delete(la);
                await _repository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPut("UpdateLiveAnnouncement/{id}")]
        public async Task<IActionResult> UpdateLiveAnnouncement(int id, [FromBody] LiveAnnouncement la)
        {
            try
            {
                //var existingCategory = await _repository.GetByIdAsync<Category>(id);
                var existingla = _appDbContext.LiveAnnouncements.Where(a => a.LiveAnnouncementID == id).FirstOrDefault();

                if (existingla == null)
                {
                    return NotFound();
                }
                _appDbContext.Attach(existingla);
                existingla.AnnouncementsDetails = la.AnnouncementsDetails;
                await _appDbContext.SaveChangesAsync();
                return Ok(la);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating internal department");
            }
        }



        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string just = "0123456789!@#$%^&*()_+=-";

            // You can adjust the length of the password as needed
            int length = 6;

            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }




        private async Task<IActionResult> SendWelcomeEmail(string receiver, string password)
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = receiver;
                mailrequest.Subject = "Welcome to MCBS App";
                mailrequest.Body = GetHtmlcontent(password);
                await emailService.SendEmailAsync(mailrequest);

                return Ok();

            }
            catch (Exception ex)
            {
                throw;
            }
        }






        private string GetHtmlcontent(string password)
        {
            string response = "<p>You have been successfully registered to the MCBS App</p>";
            response += "<p>To access your account, use the Auto generated password with your email below<p>";
            response += $"<h2>Password: {password}</h2>";
            return response;
        }




        [HttpPost]
        [Route("AddBookingType")]
        public async Task<ActionResult> AddBookingType(BookingType bk)
        {
            try
            {
                _repository.Add(bk);
                await _repository.SaveChangesAsync();
                return Ok(bk);

            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }

        [HttpDelete]
        [Route("DeleteBookingTypey/{id}")]
        public async Task<ActionResult> DeleteBookingType(int id)
        {
            try
            {
                var bk = await _repository.GetByIdAsync<BookingType>(id);

                if (bk == null)
                    return NotFound();

                _repository.Delete(bk);
                await _repository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPut("UpdateBookingType/{id}")]
        public async Task<IActionResult> UpdateBookingType(int id, [FromBody] BookingType bk)
        {
            try
            {
                //var existingCategory = await _repository.GetByIdAsync<Category>(id);
                var existingBk = _appDbContext.BookingTypes.Where(a => a.TypeID == id).FirstOrDefault();

                if (existingBk == null)
                {
                    return NotFound();
                }
                _appDbContext.Attach(existingBk);
                existingBk.Description = bk.Description;
                await _appDbContext.SaveChangesAsync();
                return Ok(bk);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating internal department");
            }
        }


        [HttpGet]
        [Route("GetAllBookingType")]
        public async Task<IActionResult> GetAllBookingTyoes()
        {
            try
            {
                var results = await _repository.GetAllAsync<BookingType>();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }



    }
}
