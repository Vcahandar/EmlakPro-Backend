using EmlakProApp.Models.Identity;

namespace EmlakProApp.ReturnTypes.Account
{
	public class RegisterReturnType
	{
		public AppUser? User { get; set; }
		public string Message { get; set; }
		public int StatusCode { get; set; }
	}
}
