namespace EmlakProApp.DTOs.AccountDTOs
{
	public class RegisterDto
	{
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
		public string Password { get; set; }
		public string RepeatPassword { get; set; }
	}
}
