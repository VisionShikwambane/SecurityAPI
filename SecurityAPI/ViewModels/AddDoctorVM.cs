using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.ViewModels
{
    public class AddDoctorVM
    {

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string  DocType { get; set; } = string.Empty;
    }
}
