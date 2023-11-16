using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.ViewModels
{
    public class ConfirmEmailVM
    {

        [Required]
        public string ConfirmEmailOPT { get; set; } = string.Empty;
    }
}
