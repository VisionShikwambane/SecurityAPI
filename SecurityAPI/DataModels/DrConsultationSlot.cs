using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class DrConsultationSlot
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SlotID { get; set; }

        public DateTime SlotDate { get; set; }

        public bool IsActive { get; set; }

        public bool? IsConfirmed { get; set; }

        [ForeignKey("Patient")]
        public int? PatientID { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }

        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }

        

    }
}
