using System.Security.Cryptography;
using System.Text;

namespace Qurabani.com_Server.Helpers
{
	public class Hasher
	{
		public static string HashPassword(string password, string salt, string pepper)
		{
			using var sha256 = SHA256.Create();

			// Combine the password with the salt and pepper
			string combinedPassword = password + salt + pepper;

			// Compute the hash value of the combined password
			byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));

			// Convert the byte array to a string representation
			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}
	}
}
