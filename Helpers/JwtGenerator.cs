using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Qurabani.com_Server.Helpers
{
	public class JwtGenerator
	{
		private readonly IConfiguration _configuration;

		public JwtGenerator(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string GenerateJwtToken(string userId)
		{
			// Create claims
			var claims = new List<Claim>();

			claims.Add(new Claim(ClaimTypes.Email, userId));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Secret").GetSection("Key").Value));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_configuration.GetSection("Secret").GetSection("Issuer").Value,
				_configuration.GetSection("Secret").GetSection("Audience").Value,
				claims,
				expires: DateTime.Now.AddMinutes(1),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);



			//var tokenHandler = new JwtSecurityTokenHandler();
			//var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Secret").GetSection("Key").Value); // Use the same secret key used in Startup.cs

			//var tokenDescriptor = new SecurityTokenDescriptor
			//{
			//	Subject = new ClaimsIdentity(new Claim[]
			//	{
			//new Claim(ClaimTypes.Name, userId)
			//	}),
			//	Expires = DateTime.UtcNow.AddMinutes(300), // Set the expiration time for the token
			//	SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			//};

			//var token = tokenHandler.CreateToken(tokenDescriptor);
			//return tokenHandler.WriteToken(token);
		}
	}
}
