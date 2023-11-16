using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.ViewModels
{
    public class RegisterVM
    {

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set;} = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set;}  =string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;


    }
}
