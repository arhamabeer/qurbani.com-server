using static Qurabani.com_Server.Responses.SwaggerResponse;
using Qurabani.com_Server.Helpers;
using Microsoft.EntityFrameworkCore;
using Qurabani.com_Server.Models.DTOs;

namespace Qurabani.com_Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]/[action]")]
    [SwaggerTag("This is <b>Qurbani Controller</b>Fetch Update and Create SQL database on bases of MongoDB format")]
    public class QurbaniController : ControllerBase
    {
        private readonly QurbaniContext _context;
		private readonly IntIsNullOrEmpty intHelper;


		public QurbaniController(QurbaniContext context)
        {
            _context = context;
			intHelper = new IntIsNullOrEmpty();
		}


		// ADD ANIMAL
		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
        [Produces("application/json", "application/xml")]
        [Consumes("application/json", "application/xml")]
        [SwaggerOperation(
            Summary = "Get all initial product list",
            Description = "This function returns all products in MongoDB format")]
        [Auth]
        [HttpPost("{type=int}/{number=int}/{partPrice=string}")]
        public async Task<IActionResult> AddAnimal(int type, int number, string partPrice, string desc = null)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (string.IsNullOrEmpty(partPrice) || intHelper.IntergerIsNullOrEmpty(type) || intHelper.IntergerIsNullOrEmpty(number))
            {
				response.ResponseCode = (int)HttpStatusCode.BadRequest;
				response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
				response.ErrorMessage = "Type, Part, Number or Part-Price should not be empty";
				return BadRequest(response);
			};

			if (await _context.AnimalDetails.AnyAsync(e => e.AnimalId == type && e.Number == number))
			{
				response.ResponseCode = (int)HttpStatusCode.BadRequest;
				response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
				response.ErrorMessage = "Number is already assigned to another Animal";
				return BadRequest(response);
			}

            var data = new AnimalDetail
            {
                AnimalId = type,
                Number = number,
                PartSellPrice = decimal.Parse(partPrice),
                Description = desc,
                Memo = null
            };
			await _context.AnimalDetails.AddAsync(data);
			var res = await _context.SaveChangesAsync();
			if (res > 0)
			{
				response.ResponseCode = (int)HttpStatusCode.OK;
				response.ResponseMessage = HttpStatusCode.OK.ToString();
				response.Data = "New Animal has been registered in the Database";
				return Ok(response);
			}
			response.ResponseCode = (int)HttpStatusCode.InternalServerError;
			response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
			response.ErrorMessage = "Error in saving the new data in the database";
			return Forbid();
		}

		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
		[Produces("application/json", "application/xml")]
		[Consumes("application/json", "application/xml")]
		[SwaggerOperation(
			Summary = "Get all initial product list",
			Description = "This function returns all products in MongoDB format")]
		[Auth]
		[HttpPost]
		public async Task<IActionResult> ConfirmDealing([FromBody] DealingDTO dealingDTO)
		{
			return null;
		}
	}
    
}
