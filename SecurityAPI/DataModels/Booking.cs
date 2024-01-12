using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class Booking
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingID { get; set; }

      
        public DateTime Date { get; set; }

        public bool IsExpired { get; set; }

        [ForeignKey("PatientID")]
        public int? PatientID { get; set; }

        [ForeignKey("BookingType")]
        public int? BookingTypeID { get; set; }

        public Patient? Patient { get; set; }

        public BookingType? BookingTypes { get; set; }


    }
}
