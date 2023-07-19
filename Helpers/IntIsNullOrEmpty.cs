namespace Qurabani.com_Server.Helpers
{
	public class IntIsNullOrEmpty
	{
		
		public bool IntergerIsNullOrEmpty(int integer)
		{
			if (integer == 0 || integer == null)
				return true;
			return false;
		}

	}
}
