using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DataModels
{
    public class LiveAnnouncement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LiveAnnouncementID { get; set; }

        [Required]
        public string AnnouncementsDetails { get; set; } = string.Empty;
    }
}
