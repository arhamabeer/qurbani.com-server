
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Qurabani.com_Server.Helpers;
using Qurabani.com_Server.Models.DTOs;
using System.Security.Claims;
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
		private readonly Salt salt;
		private readonly Pepper pepper;
		private readonly Hasher hash;
		private readonly JwtGenerator _JWT;
		private readonly VerifyPasswords _verifyPasswords;
		public AuthController(QurbaniContext context, JwtGenerator jWT, Salt salt, Pepper pepper, Hasher hasher, VerifyPasswords verifyPasswords)
		{
			_context = context;
			this.salt = salt;
			this.pepper = pepper;
			hash = hasher;
			_verifyPasswords = verifyPasswords;
			_JWT = jWT;
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
					response.ErrorMessage = "Email, Name or Password should not be empty";
					return BadRequest(response);
				}
				var user = await _context.AuthAdmins.FirstOrDefaultAsync(e => e.Email == registerDTO.Email);
				if (user != null)
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Admin with this email is already exists.";
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

		// LOGIN
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
		public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
		{
			ApiResponse<LoginDTO> response = new ApiResponse<LoginDTO>();
			try
			{
				if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Email or Password should not be empty";
					return BadRequest(response);
				}
				var user = await _context.AuthAdmins.FirstOrDefaultAsync(x => x.Email == loginDTO.Email);
				if (user == null)
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "Cannot found any Admin registered with provided email";
					return NotFound(response);
				}

				if (_verifyPasswords.VerifyPassword(loginDTO.Password, user.Salt, pepper.GetMyPrivateConstant(), user.Password))
				{
					var token = _JWT.GenerateJwtToken(user.Email);
					var data = new LoginDTO
					{
						Email = loginDTO.Email,
						Name = user.Name
					};

					response.ResponseCode = (int)HttpStatusCode.OK;
					response.ResponseMessage = HttpStatusCode.OK.ToString();
					response.Description = token;
					response.Data = data;
					return Ok(response);
				}
				else
				{
					response.ResponseCode = (int)HttpStatusCode.Unauthorized;
					response.ResponseMessage = HttpStatusCode.Unauthorized.ToString();
					response.ErrorMessage = "Wrong Credentials, try again.";
					return Unauthorized(response);
				}


			}
			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				return Forbid();
			}
		}

		// GET USER INFO
		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
		[Produces("application/json", "application/xml")]
		[Consumes("application/json", "application/xml")]
		[SwaggerOperation(
			Summary = "Get all initial product list",
			Description = "This function returns all products in MongoDB format")]
		[Authorize]
		[HttpGet()]
		public async Task<IActionResult> GetUserInfo()
		{
			ApiResponse<LoginDTO> response = new ApiResponse<LoginDTO>();
			try
			{
				// Get the claims principal
				var claimsPrincipal = User;

				// Retrieve the email claim from the claims principal
				var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

				if (emailClaim != null)
				{
					var email = emailClaim.Value;
					// Now you have the email address from the JWT token, use it as needed.
					var user = await _context.AuthAdmins.FirstOrDefaultAsync(x => x.Email == email);
					if (user != null)
					{
						var data = new LoginDTO
						{
							Email = email,
							Name = user.Name
						};
						response.ResponseCode = (int)HttpStatusCode.OK;
						response.ResponseMessage = HttpStatusCode.OK.ToString();
						response.Data = data;
						return Ok(response);
					}
					else
					{
						response.ResponseCode = (int)HttpStatusCode.NotFound;
						response.ResponseMessage = HttpStatusCode.NotFound.ToString();
						response.ErrorMessage = "Email claim not found in the token.";
						return NotFound(response);
					}
				}
				else
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "Email claim not found in the token.";
					return NotFound(response);
				}
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
