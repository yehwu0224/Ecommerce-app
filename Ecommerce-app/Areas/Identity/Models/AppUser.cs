using Microsoft.AspNetCore.Identity;

namespace Ecommerce_app.Areas.Identity.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; }
        public string? Gender { get; set; }

    }

}
