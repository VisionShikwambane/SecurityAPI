﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class Patient
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientID { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; } = string.Empty;
        public AppUser? User { get; set; }

    }
}
