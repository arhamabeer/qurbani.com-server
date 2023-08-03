using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace Qurabani.com_Server.Helpers
{
	public class VerifyPasswords
	{
		public bool VerifyPassword(string userProvidedPassword, string storedSalt, string pepper, string storedHashedPassword)
		{
			// Combine the user-provided password with the stored salt and pepper
			string combinedPassword = userProvidedPassword + storedSalt + pepper;

			// Hash the combined password using SHA-256
			using var sha256 = SHA256.Create();
			byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));

			// Convert the byte array to a lowercase hexadecimal string representation
			string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

			// Compare the generated hash with the stored hashed password
			return hashedPassword == storedHashedPassword;
		}

	}
}
