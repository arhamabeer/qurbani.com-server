using System;
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
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration.GetSection("Secret").GetSection("Key").Value); // Use the same secret key used in Startup.cs

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
			new Claim(ClaimTypes.Name, userId)
				}),
				Expires = DateTime.UtcNow.AddMinutes(30), // Set the expiration time for the token
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
