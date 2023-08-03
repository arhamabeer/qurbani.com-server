
using Qurabani.com_Server.Helpers;
using Qurabani.com_Server.Models.DTOs;
using static Qurabani.com_Server.Responses.SwaggerResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Qurabani.com_Server.Controllers
{

	[ApiController]
	[ApiVersion("1.0")]
	[Route("[controller]/[action]")]
	[SwaggerTag("This is <b>Qurbani Auth Controller</b>Get all actions belonging to the Authentication.")]
	public class AuthController : ControllerBase
	{
		private readonly QurbaniContext _context;
		private readonly IntIsNullOrEmpty intHelper;
		private readonly Salt salt;
		private readonly Pepper pepper;
		private readonly Hasher hash;
		public AuthController(QurbaniContext context)
		{
			_context = context;
			intHelper = new IntIsNullOrEmpty();
			salt = new Salt();
			pepper = new Pepper();
			hash = new Hasher();
		}

		// Register
		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
		[Produces("application/json", "application/xml")]
		[Consumes("application/json", "application/xml")]
		[SwaggerOperation(
			Summary = "Get all initial product list",
			Description = "This function returns all products in MongoDB format")]
		//[Auth]
		[HttpPost()]
		public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
		{
			ApiResponse<string> response = new ApiResponse<string>();
			try
			{
				if (string.IsNullOrEmpty(registerDTO.Email) || string.IsNullOrEmpty(registerDTO.Name) || string.IsNullOrEmpty(registerDTO.Password))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Type, Part, Number or Part-Price should not be empty";
					return BadRequest(response);
				}

				string salted = salt.GenerateSalt(128);
				string peppered = pepper.GetMyPrivateConstant();
				string hashedPassword = hash.HashPassword(registerDTO.Password, salted, peppered);

				var adminData = new AuthAdmin
				{
					Salt = salted,
					Email = registerDTO.Email,
					Password = hashedPassword,
					Name = registerDTO.Name
				};

				var res = await _context.AuthAdmins.AddAsync(adminData);
				var resDeal = await _context.SaveChangesAsync();

				if (resDeal > 0)
				{
					response.ResponseCode = (int)HttpStatusCode.OK;
					response.ResponseMessage = HttpStatusCode.OK.ToString();
					response.Data = "New user has been registered successfully.";
					return Ok(response);
				}
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				return Forbid();
			}

			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				return Forbid();
			}
		}
	}

}
