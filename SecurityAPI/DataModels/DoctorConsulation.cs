using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class DoctorConsulation
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorConsulationID { get; set; }

        public string Slot { get; set; } = string.Empty;

        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public Patient? Patient { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorID { get; set; }
        public Patient? Doctor { get; set; }

    }
}
