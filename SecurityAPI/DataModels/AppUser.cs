using Microsoft.AspNetCore.Identity;

namespace SecurityAPI.DataModels
{
    public class AppUser : IdentityUser
    {


        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public  string ConfirmEmailOPT { get; set; } = string.Empty;

   
    }
}
