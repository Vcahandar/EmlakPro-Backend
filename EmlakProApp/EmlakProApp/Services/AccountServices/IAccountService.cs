using AutoMapper;
using EmlakProApp.DTOs.AccountDTOs;
using EmlakProApp.ReturnTypes.Account;
using EmlakProApp.ReturnTypes.Device;
using Microsoft.AspNetCore.Mvc;

namespace EmlakProApp.Services.AccountServices
{
	public interface IAccountService
	{
		Task<RegisterReturnType> UserRegister([FromForm] RegisterDto registerDto, IMapper mapper);
		Task<LoginReturnType> UserLogin(LoginDto loginDto);
		Task<GoogleResponseReturnType> GoogleResponse();
		Task<GetUserReturnType> GetOne(string emailOrUserName, IMapper mapper);
		DeviceReturnType DetectDevice(HttpContext context);
		Task<string> GenerateAndSendOtpAsync(string email, string userId);

	}
}
