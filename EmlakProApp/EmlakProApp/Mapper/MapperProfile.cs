using AutoMapper;
using EmlakProApp.DTOs.AccountDTOs;
using EmlakProApp.Models.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmlakProApp.Mapper
{
	public class MapperProfile: Profile
	{
		public MapperProfile()
		{
			CreateMap<RegisterDto, AppUser>()
	.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)); 

			CreateMap<AppUser, ReturnUserDto>();
				
		}
	}
}
