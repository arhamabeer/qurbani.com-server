
namespace Qurabani.com_Server.Responses
{
	public class SwaggerResponse
	{
		[SwaggerAttributes]
		public class ApiResponse<T> where T : class
		{

			public int ResponseCode { get; set; }
			public string ResponseMessage { get; set; }
			public T? Data { get; set; } = null;
			public string? Description { get; set; }
			public string? ErrorMessage { get; set; }

		}
	}
}
