using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorID { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public string? DoctorType { get; set; }
      
    }
}
