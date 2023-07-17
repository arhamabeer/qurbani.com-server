using static Qurabani.com_Server.Responses.SwaggerResponse;

namespace Qurabani.com_Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]/[action]")]
    [SwaggerTag("This is <b>Qurbani Controller</b>Fetch Update and Create SQL database on bases of MongoDB format")]
    public class QurbaniController : ControllerBase
    {
        private readonly QurbaniContext _context;

        public QurbaniController(QurbaniContext context)
        {
            _context = context;
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
        [HttpGet("{type=int}/{part=int}/{number=int}/{partPrice=string}")]
        public async Task<IActionResult> AddAnimal(int type, int part, int number, string partPrice, string desc = "")
        {
            ApiResponse<string> response = new ApiResponse<string>();
            return null;
        }
    }
}
