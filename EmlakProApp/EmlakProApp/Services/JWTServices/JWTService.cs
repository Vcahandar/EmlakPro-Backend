using EmlakProApp.Models.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmlakProApp.Services.JWTServices
{
	public class JWTService: IJWTService
	{
		public string JWTToken(IConfiguration _config, AppUser user, IList<string> roles)
		{

			var key = Encoding.ASCII.GetBytes
			(_config["Jwt:Key"]);
			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti,
				Guid.NewGuid().ToString())
			};
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));



			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(60),
				Issuer = _config["Jwt:Issuer"],
				Audience = _config["Jwt:Audience"],
				SigningCredentials = new SigningCredentials
				(new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256)
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var jwtToken = tokenHandler.WriteToken(token);
			var stringToken = tokenHandler.WriteToken(token);
			return stringToken;
		}

		public string SimpleJWTToken(IConfiguration _config, string userName)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				expires: DateTime.Now.AddMinutes(120),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
