using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAPI.DataModels;
using SecurityAPI.DBContext;
using SecurityAPI.Repositories;
using SecurityAPI.Services;
using SecurityAPI.ViewModels;
using System.Linq.Expressions;

namespace SecurityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService emailService;
        private readonly AppDbContext _appDbContext;
        private readonly IRepository _repository;

        public DoctorController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService service, AppDbContext appDbContext, IRepository repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            this.emailService = service;
            _appDbContext = appDbContext;
            _repository = repository;
        }


        [HttpPost]
        [Route("AddSlot")]
        public async Task<ActionResult> AddSlot(AddSlotVM slot)
        {
            try
            {
               // var user = await _userManager.GetUserAsync(User);
               // var doctor = await _appDbContext.Doctors.Include(e => e.User).FirstOrDefaultAsync(e => e.User!.Id == user.Id);

               /* if(doctor == null)
                {
                    return NotFound("");
                }*/

                var slot1 = new Slot
                {
                    
                    SlotDescription = slot.SlotDescription,
                    IsActive = true,
                    // DoctorID = doctor.DoctorID,
                   
                    
                    
                };

                _repository.Add(slot1);
                await _repository.SaveChangesAsync();
                return Ok("Slot Created Successfully");

            }
            catch (Exception)
            {

                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }


        [HttpDelete]
        [Route("DeleteSlot/{id}")]
        public async Task<ActionResult> DeleteSlot(int id)
        {
            try
            {
                var slot = await _repository.GetByIdAsync<Slot>(id);

                if (slot == null)
                    return NotFound();

                _repository.Delete(slot) ;
                await _repository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPut("UpdateSlot/{id}")]
        public async Task<IActionResult> UpdateSlot(int id, [FromBody] AddSlotVM Slt)
        {
            try
            {
                //var existingCategory = await _repository.GetByIdAsync<Category>(id);
                var existingSlt = _appDbContext.Slots.Where(a => a.SlotID == id).FirstOrDefault();

                if (existingSlt == null)
                {
                    return NotFound();
                }
                _appDbContext.Attach(existingSlt);
                existingSlt.SlotDescription = Slt.SlotDescription;
                await _appDbContext.SaveChangesAsync();
                return Ok(Slt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error updating internal department");
            }
        }



        [HttpGet]
        [Route("GetAllSlots")]
        public async Task<IActionResult> GetAllSlots()
        {
            try
            {
                var results =  _appDbContext.Slots.Where(a => a.IsActive == true);
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }



    }
}
