namespace EmlakProApp.Models.Identity
{
	public class OtpCode
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Code { get; set; }
		public DateTime ExpiryTime { get; set; }
	}

}
