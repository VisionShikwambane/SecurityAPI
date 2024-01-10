using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class Slot
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlotID { get; set; }
        public string SlotDescription { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        [ForeignKey("Patient")]
        public int? PatientID { get; set; }

        [ForeignKey("Doctor")]
        public int? DoctorID { get; set; }



        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }

        

    }
}
