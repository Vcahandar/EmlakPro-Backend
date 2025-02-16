using System.ComponentModel.DataAnnotations;

namespace EmlakProApp.DTOs.AccountDTOs
{
	public class LoginDto
	{
		[Required]
		public string EmailOrUsername { get; set; }

		[Required]
		public string Password { get; set; }
	}
}
