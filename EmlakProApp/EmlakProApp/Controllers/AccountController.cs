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
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			try
			{
				var result = await _accountService.UserRegister(registerDto, _mapper);
				if (result.User == null)
				{
					return BadRequest(new { Message = result.Message });
				}

				await _accountService.GenerateAndSendOtpAsync(registerDto.Email, result.User.Id);

				return Ok(new { Message = "OTP kodu e-mail ünvanınıza göndərildi. Təsdiqlədikdən sonra login edə bilərsiniz." });
			}
			catch (Exception)
			{
				return StatusCode(500, "Gözlənilməz xəta baş verdi. Zəhmət olmasa, sonra yenidən cəhd edin.");
			}
		}


		[HttpPost("Login")]
		public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return NotFound("İstifadəçi tapılmadı.");

			// Email təsdiqlənməyibsə, girişə icazə verilmir
			if (!user.EmailConfirmed)
			{
				return BadRequest("E-mail təsdiqlənməyib. Zəhmət olmasa, OTP kodu ilə təsdiq edin.");
			}

			var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
			if (!result.Succeeded) return Unauthorized("Yanlış email və ya şifrə.");

			return Ok("Uğurla giriş etdiniz.");
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
			if (email == null) return BadRequest();
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return NotFound();
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.Action(nameof(ResetPassword), "Account", new { Email = email, Token = token }, Request.Scheme, Request.Host.ToString());
			_emailService.ConfirmEmail(email, "Reset Password", url, $"<a href={url}>Click Here to start reseting Password</a>");
			return Ok();
		}


		[HttpGet("ResetPassword")]
		public async Task<IActionResult> ResetPassword([Required, EmailAddress] string email, [Required] string token)
		{
			AppUser user = await _userManager.FindByEmailAsync(email);
			if (user == null) return BadRequest();
			bool checkToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
			if (!checkToken) return BadRequest("Link artiq istifade olunub!!");
			return Ok(new ResetPasswordDto { Email = email, Token = token });
		}



		[HttpPost("ResetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			if (resetPasswordDto == null) return BadRequest();
			AppUser user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
			if (user == null) return NotFound();
			await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
			await _userManager.UpdateSecurityStampAsync(user);
			return Ok();
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

