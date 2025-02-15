using AutoMapper;
using EmlakProApp.Data;
using EmlakProApp.DTOs.AccountDTOs;
using EmlakProApp.Models.Identity;
using EmlakProApp.ReturnTypes.Account;
using EmlakProApp.ReturnTypes.Device;
using EmlakProApp.Services.EmailServices;
using EmlakProApp.Services.JWTServices;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UAParser;

namespace EmlakProApp.Services.AccountServices
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _config;
		private readonly AppDbContext _context;
		private readonly IJWTService _jwtService;
		private readonly Parser uaParser;
		private readonly IEmailService _emailService;
		private readonly IValidator<RegisterDto> _validator;



		// 


		public AccountService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
							  AppDbContext context, IValidator<RegisterDto> validator, SignInManager<AppUser> signInManager,
							  IConfiguration config, IJWTService jwtService, IEmailService emailService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
			_validator = validator;
			_signInManager = signInManager;
			_config = config;
			_jwtService = jwtService;
			uaParser = Parser.GetDefault();
			_emailService = emailService;
		}

		public async Task<GetUserReturnType> GetOne(string? emailOrUserName, IMapper mapper)
		{
			if (emailOrUserName == null) return new() { Message = "Bad Request", StatusCode = 400 };
			var user = await _userManager.FindByEmailAsync(emailOrUserName);



			if (user == null)
			{
				user = await _userManager.FindByNameAsync(emailOrUserName);
				if (user == null) return new() { Message = "User with ths email or username doesn't exist", StatusCode = 404 };
			}
			var result = new GetUserReturnType()
			{
				User = mapper.Map<ReturnUserDto>(user),
			};
			if (result == null)
			{
				result.StatusCode = 400;
				result.Message = "Xeta bash verdi!";
				return result;
			}
			else
			{
				result.StatusCode = 200;
				result.Message = "Secceded";
				return result;
			}

		}

		public DeviceReturnType DetectDevice(HttpContext context)
		{
			var userAgent = context.Request.Headers["User-Agent"].ToString();

			var clientInfo = uaParser.Parse(userAgent);

			var result = new DeviceReturnType()
			{
				DeviceType = clientInfo.Device.Family.ToLower(),
				DeviceBrand = clientInfo.Device.Brand,
				DeviceModel = clientInfo.Device.Model,
				OperationSystem = clientInfo.OS.ToString(),
				Browser = clientInfo.UA.Family
			};

			return result;
		}

		//Login with Goolge üçün domain, təşkilat email-i və proqramist məlutmatları lazımdır.

		public async Task<GoogleResponseReturnType> GoogleResponse()
		{
			//    var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			//    if (!result.Succeeded)
			//        return new() {Message="Bad Request",StatusCode= 400}; 

			//    var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
			//    {
			//        claim.Issuer,
			//        claim.OriginalIssuer,
			//        claim.Type,
			//        claim.Value
			//    });

			//    var token = _jwtService.SimpleJWTToken(result.Principal.Identity.Name);
			//    if (token == null)
			//        return new() { Message = "Something went wrong", StatusCode = 400 };

			//    else 
			//        return new() { Message = "Succedd", StatusCode = 200, Token = token };

			return null;
		}


		public async Task<LoginReturnType> UserLogin(LoginDto loginDto)
		{
			if (loginDto == null) return new() { Message = "Bad Request", StatusCode = 404 };
			var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUsername);
			if (user == null)
			{
				user = await _userManager.FindByNameAsync(loginDto.EmailOrUsername);
				if (user == null) return new() { Message = "The User not found", StatusCode = 404 };
			}

			var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, true, true);
			if (!result.Succeeded) return new() { Message = result.ToString(), StatusCode = 404 };
			if (result.IsLockedOut) return new() { Message = "Bad Request", StatusCode = 404 };

			var roles = await _userManager.GetRolesAsync(user);

			var stringToken = _jwtService.JWTToken(_config, user, roles);

			return new() { Message = "Signed in successfully", StatusCode = 200, JWTToken = stringToken };
		}

		public async Task<RegisterReturnType> UserRegister(RegisterDto registerDto, IMapper mapper)
		{
			var validationResult = await _validator.ValidateAsync(registerDto);

			if (!validationResult.IsValid)
			{
				return new RegisterReturnType
				{
					Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
					StatusCode = 400
				};
			}

			if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
			{
				return new RegisterReturnType { Message = "Bu email ilə artıq qeydiyyat var!", StatusCode = 400 };
			}

			var user = mapper.Map<AppUser>(registerDto);
			user.UserName = registerDto.Email;
			user.Name ??= "Unknown";

			var result = await _userManager.CreateAsync(user, registerDto.Password);

			if (!result.Succeeded)
			{
				return new RegisterReturnType
				{
					Message = string.Join(", ", result.Errors.Select(e => e.Description)),
					StatusCode = 400
				};
			}

			// Burada yaradılmış user-i də qaytarırıq
			return new RegisterReturnType
			{
				User = user,
				Message = "Qeydiyyat uğurla tamamlandı!",
				StatusCode = 200
			};
		}


		public async Task<string> GenerateAndSendOtpAsync(string email, string userId)
		{
			// OTP kodunu yaradın
			var otpCode = new Random().Next(100000, 999999).ToString();
			var otp = new OtpCode
			{
				UserId = userId,
				Code = otpCode,
				ExpiryTime = DateTime.UtcNow.AddMinutes(5)
			};

			// OTP kodunu verilənlər bazasına əlavə edin
			_context.OtpCodes.Add(otp);
			await _context.SaveChangesAsync();

			// OTP kodunu email vasitəsilə göndərin
			await _emailService.SendOtpEmailAsync(email, "Your OTP Code", otpCode);

			return otpCode;
		}




	}
}
