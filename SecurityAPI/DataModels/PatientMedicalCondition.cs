using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SecurityAPI.DataModels
{
    public class PatientMedicalCondition
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientMedicalConditionID { get; set; }

        public string MedicalConditionDescription { get; set; } = string.Empty;


    }
}
