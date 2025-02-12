using Microsoft.AspNetCore.Identity;

namespace EmlakProApp.Models.Identity
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; } 
		public bool IsRemember { get; set; }

	}
}
