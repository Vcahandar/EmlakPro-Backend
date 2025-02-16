 using AutoMapper;
using EmlakProApp.Data;
using EmlakProApp.DTOs.AccountDTOs;
using EmlakProApp.Models.Identity;
using EmlakProApp.Services.AccountServices;
using EmlakProApp.Services.EmailServices;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace EmlakProApp.Controllers
{
	[EnableCors("AllowAll")]  // CORS icazəsi verildi
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IAccountService _accountService;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;


		public AccountController(AppDbContext context, IAccountService accountService, IMapper mapper, IEmailService emailService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_context = context;
			_accountService = accountService;
			_mapper = mapper;
			_emailService = emailService;
			_userManager = userManager;
			_signInManager = signInManager;
		}
		[HttpGet("GetOne")]
		public async Task<IActionResult> GetOne(string emailOrUsername)
		{
			var result = await _accountService.GetOne(emailOrUsername, _mapper);
			return StatusCode(result.StatusCode, result);

		}





		[HttpPost("Register")]
		public async Task<IActionResult> Register([FromBody] string email)
		{
			try
			{
				var result = await _accountService.UserRegister(email);
				if (result.User == null)
				{
					return BadRequest(new { Message = result.Message });
				}

				return Ok(new { Message = "OTP kodunuz email ünvanınıza göndərildi. O kodu daxil edərək giriş edin." });
			}
			catch (Exception)
			{
				return StatusCode(500, "Sistemde problem var. Zəhmət olmasa, bir az sonra yenidən cəhd edin.");
			}
		}



		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
		{
			var result = await _accountService.UserLogin(loginDto);

			if (result.StatusCode == 200)
			{
				Response.Cookies.Append("jwt", result.JWTToken, new CookieOptions
				{
					HttpOnly = true,
					Secure = true, 
					SameSite = SameSiteMode.Strict,
					Expires = DateTime.UtcNow.AddHours(1)
				});
			}

			return StatusCode(result.StatusCode, new { result.Message, result.StatusCode });
		}



		//[HttpGet("VerifyEmail")]
		//public async Task<IActionResult> VerifyEmail(string email, string token)
		//{
		//	AppUser user = await _userManager.FindByEmailAsync(email);
		//	if (user == null) return NotFound();
		//	if (user.EmailConfirmed)
		//	{
		//		return Ok(new { result = "success" });
		//	}
		//	await _userManager.ConfirmEmailAsync(user, token);
		//	await _signInManager.SignInAsync(user, true);
		//	return Ok(new { Email = email, Token = token });

		//}



		[HttpPost("ForgotPassword")]
		public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
		{
			var result = await _accountService.ForgotPassword(email);
			return StatusCode(result.StatusCode, result.Message);
		}




		//[HttpGet("signin-google")]
		//public IActionResult SignInWithGoogle()
		//{
		//	var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
		//	return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		//}


		//[HttpGet("google-response")]
		//public async Task<IActionResult> GoogleResponse()
		//{
		//	var result = await _accountService.GoogleResponse();

		//	return Ok();
		//}


		[HttpGet("Detect-Device")]
		public IActionResult DetectDevice()
		{
			var result = _accountService.DetectDevice(HttpContext);
			return Ok(result);
		}


		[HttpPost("VerifyOtp")]
		public async Task<IActionResult> VerifyOtp([FromForm] string email, [FromForm] string otpCode)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return NotFound("İstifadəçi tapılmadı.");

			var otp = await _context.OtpCodes.FirstOrDefaultAsync(o => o.UserId == user.Id && o.Code == otpCode);
			if (otp == null || otp.ExpiryTime < DateTime.UtcNow)
			{
				return BadRequest("Yanlış və ya müddəti bitmiş OTP kodu.");
			}

			// İstifadəçinin emailini təsdiqləyirik
			user.EmailConfirmed = true;
			await _userManager.UpdateAsync(user);

			// OTP kodunu silirik
			_context.OtpCodes.Remove(otp);
			await _context.SaveChangesAsync();

			return Ok("Qeydiyyat uğurla tamamlandı. Artıq login ola bilərsiniz.");
		}


	}
}

