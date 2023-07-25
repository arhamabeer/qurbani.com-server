namespace Qurabani.com_Server.Custom_Attributes
{
	public class SwaggerAttributes : Attribute
	{
		[AttributeUsage(AttributeTargets.Class)]
		public class SwaggerExcludeAttribute : Attribute
		{
		}
	}
}
