using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminID { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; } = string.Empty;
        public AppUser? User { get; set; }

    }
}
