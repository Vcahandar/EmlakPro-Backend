using EmlakProApp.DTOs.AccountDTOs;

namespace EmlakProApp.ReturnTypes.Account
{
	public class GetUserReturnType
	{
		public string Message { get; set; }
		public int StatusCode { get; set; }
		public ReturnUserDto? User { get; set; }
	}
}
