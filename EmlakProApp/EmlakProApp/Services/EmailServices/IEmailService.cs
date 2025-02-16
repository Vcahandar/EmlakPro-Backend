namespace EmlakProApp.Services.EmailServices
{
	public interface IEmailService
	{
		void ConfirmEmail(string address, string subject, string url, string body = null);
		void SendOtpEmail(string address, string subject, string otpCode);
	}
}
