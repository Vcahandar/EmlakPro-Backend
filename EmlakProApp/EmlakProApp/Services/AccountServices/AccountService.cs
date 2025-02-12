using AutoMapper;
using EmlakProApp.Data;
using EmlakProApp.DTOs.AccountDTOs;
using EmlakProApp.Models.Identity;
using EmlakProApp.ReturnTypes.Account;
using EmlakProApp.ReturnTypes.Device;
using EmlakProApp.Services.EmailServices;
using EmlakProApp.Services.JWTServices;
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



		// 


		public AccountService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
							  AppDbContext context, SignInManager<AppUser> signInManager,
							  IConfiguration config, IJWTService jwtService, IEmailService emailService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
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
			if (registerDto == null)
				return new() { Message = "Bad Request", StatusCode = 400 };

			var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUser != null)
				return new() { Message = "Bu email ilə artıq qeydiyyat var!", StatusCode = 409 };

			var user = mapper.Map<AppUser>(registerDto);
			user.UserName = registerDto.Email;  // UserName-i Email kimi təyin edirik
			user.Name ??= "Unknown";  // Əgər `Name` NULL-dursa, default dəyər ver

			var result = await _userManager.CreateAsync(user, registerDto.Password);
			if (!result.Succeeded)
			{
				return new()
				{
					Message = string.Join(", ", result.Errors.Select(e => e.Description)),
					StatusCode = 400
				};
			}

			return new() { Message = "Qeydiyyat uğurlu oldu!", StatusCode = 201, User = user };
		}



	}
}
