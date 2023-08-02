namespace Qurabani.com_Server.Helpers
{
	public class Salt
	{
		public string GenerateSalt(int length)
		{
			const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			var saltChars = new char[length];

			for (int i = 0; i < length; i++)
			{
				saltChars[i] = validChars[random.Next(validChars.Length)];
			}

			return new string(saltChars);
		}
	}
}
