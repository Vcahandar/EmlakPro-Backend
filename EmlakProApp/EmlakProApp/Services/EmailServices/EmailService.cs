using EmlakProApp.Utilities;
using System.Net.Mail;
using System.Net;

namespace EmlakProApp.Services.EmailServices
{
	public class EmailService : IEmailService
	{
		private readonly EmailConfig _config;

		public EmailService(EmailConfig config)
		{
			_config = config;
		}

		public void ConfirmEmail(string address, string subject, string url, string body = null)
		{
			MailMessage message = new();
			message.From = new MailAddress(_config.From);
			if (body is null) body = File.ReadAllText("wwwroot/Templates/VerifyEmail.html");
			body = body.Replace("{{link}}", url);
			message.IsBodyHtml = true;
			message.Body = body;
			message.Subject = subject;
			message.To.Add(address);

			SmtpClient smtpClient = new();
			smtpClient.Port = _config.Port;
			smtpClient.Host = _config.SmtpServer;
			smtpClient.EnableSsl = true;
			smtpClient.Credentials = new NetworkCredential(_config.From, _config.Password);
			smtpClient.Send(message);
		}

		public void SendOtpEmail(string address, string subject, string otpCode)
		{
			MailMessage message = new();
			message.From = new MailAddress(_config.From);
			string body = $"Sizin OTP kodunuz: {otpCode}";
			message.IsBodyHtml = true;
			message.Body = body;
			message.Subject = subject;
			message.To.Add(address);

			SmtpClient smtpClient = new();
			smtpClient.Port = _config.Port;
			smtpClient.Host = _config.SmtpServer;
			smtpClient.EnableSsl = true;
			smtpClient.Credentials = new NetworkCredential(_config.From, _config.Password);
			smtpClient.Send(message);
		}

	}

}
