using EmlakProApp.Models.Identity;

namespace EmlakProApp.Services.JWTServices
{
	public interface IJWTService
	{
		string JWTToken(IConfiguration _config, AppUser user, IList<string> roles);
		string SimpleJWTToken(IConfiguration _config, string param);
	}
}
