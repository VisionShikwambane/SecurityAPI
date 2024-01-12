using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAPI.DataModels;
using SecurityAPI.DBContext;
using SecurityAPI.Repositories;
using SecurityAPI.Services;
using SecurityAPI.ViewModels;

namespace SecurityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {


        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IRepository _repository;

        public PatientController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService service, AppDbContext appDbContext, IRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.emailService = service;
            _appDbContext = appDbContext;
            _repository = repository;
        }


        [HttpPost]
        [Route("MakeBooking")]
        public async Task<ActionResult> MakeBooking(Booking booking)
        {
            try
            {
                 var user = await _userManager.GetUserAsync(User);
                 var patient = await _appDbContext.Patients.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

                 if(patient == null)
                 {
                     return NotFound("Patient Not Found");
                 }

                var ExistingBooking = _appDbContext.Bookings.Where(e => e.PatientID == patient.PatientID && e.BookingTypeID == booking.BookingTypeID).FirstOrDefault();

                if(ExistingBooking != null)
                {
                    return BadRequest("You already made this type of booking");
                }
                else
                {

                    var bk = new Booking
                    {
                        BookingTypeID = booking.BookingTypeID,
                        Date = booking.Date,
                        PatientID = patient.PatientID,
                        IsExpired = false
                    };

                    _repository.Add(bk);
                    await _repository.SaveChangesAsync();

                    return Ok("Booking Created Successfully");
                }


            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }




        [HttpDelete]
        [Route("DeleteBooking/{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
        {
            try
            {
                var bk = await _repository.GetByIdAsync<Booking>(id);

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


        [HttpGet]
        [Route("GetAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var results = _appDbContext.Bookings.Where(a => a.IsExpired == false);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }





    }
}
