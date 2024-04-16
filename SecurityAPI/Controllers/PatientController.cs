using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAPI.DataModels;
using SecurityAPI.DBContext;
using SecurityAPI.Models;
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
                if (ExistingBooking != null)
                {
                    return BadRequest("You already made this type of booking");
                }
                else
                {
                   booking.PatientID = patient.PatientID;
                   _repository.Add(booking);
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
                var user = await _userManager.GetUserAsync(User);
                var patient = await _appDbContext.Patients.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                var results = _appDbContext.Bookings.Where(a => a.IsExpired == false && a.PatientID == patient.PatientID);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }



        [HttpGet]
        [Route("GetPatientProfile")]
        public async Task<IActionResult> GetPatientProfile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var patient = await _appDbContext.Patients.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

                if (patient == null)
                {
                    return NotFound("Patient not found");
                }


                var patientVM = new PatientProfileVM
                {

                    Name = patient.User.FirstName,
                    Surname = patient.User.LastName,
                    Email = patient.User.Email,
                    Interests = patient.Interests,
                    MedicalCondition = patient.MedicalCondition,
                    PhoneNumber = patient.User.PhoneNumber,
                    ProfilePic = patient.User.ProfilePicture,
                    Title = patient.User.Title,
    
                };

                return Ok(patientVM);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }




        [HttpPut]
        [Route("BookConsultationWithDoctor/{SlodID}")]
        public async Task<ActionResult> BookConsultationWithDoctor(int SlodID)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var patient = await _appDbContext.Patients.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

                if (patient == null)
                {
                    return NotFound("Patient Not Found");
                }

                var ChosenSlot = _appDbContext.Slots.Where(s=>s.SlotID == SlodID && s.IsActive == true  ).FirstOrDefault();

                if(ChosenSlot.PatientID != null)
                {
                    return BadRequest("This Slot has already been taken");
                }

                else
                {
                    _appDbContext.Attach(ChosenSlot);
                    ChosenSlot.PatientID = patient.PatientID;
                    _appDbContext.SaveChanges();
                    return Ok("Consulation made, you will be notified when the Doctor Approves the Consultation");
                }

              
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
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






        [HttpPut("CancelConsulationBooking/{id}")]
        public async Task<IActionResult> CancelConsulationBooking (int id)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                var patient = await _appDbContext.Patients.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

                var existingSlt = _appDbContext.Slots.Where(a => a.SlotID == id).FirstOrDefault();

                if (existingSlt == null)
                {
                    return NotFound();
                }
                _appDbContext.Attach(existingSlt);
                existingSlt.PatientID = null;
                await _appDbContext.SaveChangesAsync();
                return Ok("Consultation Cancelled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating internal department");
            }
        }




    }
}
