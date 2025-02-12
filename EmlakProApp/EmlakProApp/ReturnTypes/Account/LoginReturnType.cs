namespace EmlakProApp.ReturnTypes.Account
{
	public class LoginReturnType
	{
		public int StatusCode { get; set; }
		public string Message { get; set; }
		public string? JWTToken { get; set; }
	}
}
