
namespace Qurabani.com_Server.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class QurbaniController : ControllerBase
	{
		private readonly QurbaniContext _context;

		public QurbaniController(QurbaniContext context)
		{
			_context = context;
		}


		public async Task<IActionResult> AddAnimal()
	}
}
