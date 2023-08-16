using static Qurabani.com_Server.Responses.SwaggerResponse;
using Qurabani.com_Server.Helpers;
using Microsoft.EntityFrameworkCore;
using Qurabani.com_Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

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
		private readonly ILogger<QurbaniController> _logger;



		public QurbaniController(QurbaniContext context, ILogger<QurbaniController> logger)
		{
			_context = context;
			intHelper = new IntIsNullOrEmpty();
			_logger = logger;
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
		[Authorize]
		[HttpPost()]
		public async Task<IActionResult> AddAnimal([FromBody] AnimalDTO animalDTO)
		{
			ApiResponse<string> response = new ApiResponse<string>();
			_logger.LogInformation("ADD-ANIMAL API HIT...");
			try
			{
				if (string.IsNullOrEmpty(animalDTO.partPrice) || intHelper.IntergerIsNullOrEmpty(animalDTO.type) || intHelper.IntergerIsNullOrEmpty(animalDTO.number))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Type, Part, Number or Part-Price should not be empty";
					_logger.LogWarning($"ADD-ANIMAL API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				};

				if (await _context.AnimalDetails.AnyAsync(e => e.AnimalId == animalDTO.type && e.Number == animalDTO.number))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Number is already assigned to another Animal";
					_logger.LogWarning($"ADD-ANIMAL API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}

				var data = new AnimalDetail
				{
					AnimalId = animalDTO.type,
					Number = animalDTO.number,
					PartSellPrice = decimal.Parse(animalDTO.partPrice),
					Description = animalDTO.desc,
					Memo = null
				};
				await _context.AnimalDetails.AddAsync(data);
				var res = await _context.SaveChangesAsync();
				if (res > 0)
				{
					response.ResponseCode = (int)HttpStatusCode.OK;
					response.ResponseMessage = HttpStatusCode.OK.ToString();
					response.Data = "New Animal has been registered in the Database";
					_logger.LogInformation($"ADD-ANIMAL API OK... {response.ResponseMessage}");
					return Ok(response);
				}
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Error in saving the new data in the database";
				_logger.LogWarning($"ADD-ANIMAL API NULL EXCEPTION... {response.ErrorMessage}");
				return Forbid();
			}
			catch (Exception ex)
			{
				_logger.LogError($"ADD-ANIMAL API CATCH... {ex.Message}");
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Error in the server, Try Again.";
				return Forbid();
			}
		}


		// ADD DEAL
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
		[HttpPost]
		public async Task<IActionResult> ConfirmDealing([FromBody] DealingDTO dealingDTO)
		{
			ApiResponse<string> response = new ApiResponse<string>();
			_logger.LogInformation("CONFIRM-DEALING API HIT...");
			try
			{
				if (string.IsNullOrEmpty(dealingDTO.Address) ||
					string.IsNullOrEmpty(dealingDTO.Name) ||
					string.IsNullOrEmpty(dealingDTO.Contact) ||
					string.IsNullOrEmpty(dealingDTO.EmergencyContact) ||
					string.IsNullOrEmpty(dealingDTO.Nic) ||
					intHelper.IntergerIsNullOrEmpty(dealingDTO.AdId) ||
					intHelper.IntergerIsNullOrEmpty(dealingDTO.PartId) ||
					intHelper.IntergerIsNullOrEmpty(dealingDTO.QurbaniDay))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Every Field is Mandatory and cannot be empty";
					_logger.LogWarning($"CONFIRM-DEALING API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}

				var cond1 = await _context.AnimalParts.FirstOrDefaultAsync(e => e.PartId == dealingDTO.PartId);
				var cond2 = await _context.AnimalDetails.FirstOrDefaultAsync(e => e.Adid == dealingDTO.AdId);
				if (cond1 == null || cond2 == null)
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Invalid Animal or Part";
					_logger.LogWarning($"CONFIRM-DEALING API INVALID EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}
				if (await _context.People.AnyAsync(e => e.Nic == dealingDTO.Nic))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "NIC already assigned to an other Animal";
					_logger.LogWarning($"CONFIRM-DEALING API ALREADY ASSIGNED EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}
				if (await _context.Dealings.AnyAsync(e => e.Adid == dealingDTO.AdId && e.PartId == dealingDTO.PartId))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "This Animal Part is already assigned to another Person";
					_logger.LogWarning($"CONFIRM-DEALING API ALREADY ASSIGNED EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}

				var personData = new Person
				{
					Address = dealingDTO.Address,
					Contact = dealingDTO.Contact,
					EmergencyContact = dealingDTO.EmergencyContact,
					Name = dealingDTO.Name,
					Nic = dealingDTO.Nic,
					Memo = null
				};
				var addPerson = await _context.People.AddAsync(personData);
				var resPerson = await _context.SaveChangesAsync();
				var PersonId = personData.PersonId;
				if (resPerson > 0)
				{
					var dealData = new Dealing
					{
						PersonId = PersonId,
						Adid = dealingDTO.AdId,
						PartId = dealingDTO.PartId,
						QurbaniDay = dealingDTO.QurbaniDay,
						Description = dealingDTO.Description,
						IsConfirm = true,
						PickedUp = false,
						Memo = null
					};
					var addDeal = await _context.Dealings.AddAsync(dealData);
					var resDeal = await _context.SaveChangesAsync();
					if (resDeal > 0)
					{
						response.ResponseCode = (int)HttpStatusCode.OK;
						response.ResponseMessage = HttpStatusCode.OK.ToString();
						response.Data = "New Deal has been registered in the Database";
						_logger.LogInformation($"CONFIRM-DEALING API OK... {response.ResponseMessage}");
						return Ok(response);
					}
					response.ResponseCode = (int)HttpStatusCode.InternalServerError;
					response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
					response.ErrorMessage = "Error in saving the new data in the database";
					_logger.LogWarning($"CONFIRM-DEALING SERVER EXCEPTION... {response.ErrorMessage}");
					return Forbid();
				}
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Error in saving the new data in the database";
				_logger.LogWarning($"CONFIRM-DEALING SERVER EXCEPTION... {response.ErrorMessage}");
				return Forbid();

			}
			catch (Exception ex)
			{
				_logger.LogError($"CONFIRM-DEALING API CATCH... {ex.Message}");
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				return Forbid();
			}
		}

		// ISSUE DEAL
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
		[HttpPost]
		public async Task<IActionResult> IssueDealToPerson([FromBody] IssueDealRequestDTO issueDealRequestDTO)
		{
			ApiResponse<string> response = new ApiResponse<string>();
			_logger.LogInformation($"ISSUE-DEAL API HIT...");
			try
			{
				if (intHelper.IntergerIsNullOrEmpty(issueDealRequestDTO.dealId) || intHelper.IntergerIsNullOrEmpty(issueDealRequestDTO.dealId))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Invalid Deal ID or Person ID";
					_logger.LogWarning($"ISSUE-DEAL API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}
				var deal = await _context.Dealings.FirstOrDefaultAsync(x => x.DealId == issueDealRequestDTO.dealId && x.PersonId == issueDealRequestDTO.personId);
				if (deal == null)
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "Cannot Find Any Deal associated with given IDs.";
					_logger.LogWarning($"ISSUE-DEAL API NOT-FOUND EXCEPTION... {response.ErrorMessage}");
					return NotFound(response);
				}
				deal.PickedUp = true;
				var res = await _context.SaveChangesAsync();
				if (res > 0)
				{
					response.ResponseCode = (int)HttpStatusCode.OK;
					response.ResponseMessage = HttpStatusCode.OK.ToString();
					response.Data = "Data Updated Successfully.";
					_logger.LogInformation($"ISSUE-DEAL API OK... {response.ResponseMessage}");
					return Ok(response);
				}
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				_logger.LogWarning($"ISSUE-DEAL API SERVER EXCEPTION... {response.ErrorMessage}");
				return Forbid();
			}
			catch (Exception ex)
			{
				{
					_logger.LogError($"ISSUE-DEAL API CATCH... {ex.Message}");
					response.ResponseCode = (int)HttpStatusCode.InternalServerError;
					response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
					response.ErrorMessage = "Server Error during the execution. Try Again";
					return Forbid();
				}
			}
		}


		// GET DEAL
		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<DealingDTO>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
		[Produces("application/json", "application/xml")]
		[Consumes("application/json", "application/xml")]
		[SwaggerOperation(
			Summary = "Get all initial product list",
			Description = "This function returns all products in MongoDB format")]
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetDealOfPerson(string nic)
		{
			ApiResponse<DealingDTO> response = new ApiResponse<DealingDTO>();
			_logger.LogInformation("GET-DEAL API HIT...");
			try
			{
				if (string.IsNullOrEmpty(nic))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "NIC cannot be empty";
					_logger.LogWarning($"GET-DEAL API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}
				var person_data = await _context.People.FirstOrDefaultAsync(e => e.Nic == nic);
				if (person_data == null)
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "Cannot Find Any Person associated with given NIC number.";
					_logger.LogWarning($"GET-DEAL API NOT-FOUND EXCEPTION... {response.ErrorMessage}");
					return NotFound(response);
				}
				var deal_data = await _context.Dealings.FirstOrDefaultAsync(e => e.PersonId == person_data.PersonId);
				if (deal_data == null)
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "No Deal is associated with given NIC number.";
					_logger.LogWarning($"GET-DEAL API NOT-FOUND EXCEPTION... {response.ErrorMessage}");
					return NotFound(response);
				}
				var animal_detail = await _context.AnimalDetails.FirstOrDefaultAsync(e => e.Adid == deal_data.Adid);
				if (animal_detail == null)
				{
					response.ResponseCode = (int)HttpStatusCode.NotFound;
					response.ResponseMessage = HttpStatusCode.NotFound.ToString();
					response.ErrorMessage = "No Animal data found that associated with given NIC number.";
					_logger.LogWarning($"GET-DEAL API NOT-FOUND EXCEPTION... {response.ErrorMessage}");
					return NotFound(response);
				}
				var animalType = await _context.Animals.FirstOrDefaultAsync(e => e.AnimalId == animal_detail.AnimalId);
				if (animal_detail.PartFinalPrice == null)
					animal_detail.PartFinalPrice = animal_detail.PartSellPrice;
				var response_data = new DealingDTO
				{
					Address = person_data.Address,
					Contact = person_data.Contact,
					EmergencyContact = person_data.EmergencyContact,
					Name = person_data.Name,
					Nic = nic,
					QurbaniDay = (int)deal_data.QurbaniDay,
					Description = deal_data.Description,
					PartId = (int)deal_data.PartId,
					AdId = (int)deal_data.Adid,
					DealId = deal_data.DealId,
					PickedUp = deal_data.PickedUp,
					PersonId = person_data.PersonId,
					Number = (int)animal_detail.Number,
					Price = (int)animal_detail.PartSellPrice,
					FinalPrice = (int)animal_detail.PartFinalPrice,
					AnimalType = animalType.Type
				};
				response.ResponseCode = (int)HttpStatusCode.OK;
				response.ResponseMessage = HttpStatusCode.OK.ToString();
				response.Description = "Data Fetched Successfully.";
				response.Data = response_data;
				_logger.LogInformation($"GET-DEAL API OK... {response.ResponseMessage}");
				return Ok(response);
			}
			catch (Exception ex)
			{
				_logger.LogError($"GET-DEAL API CATCH... {ex.Message}");
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				return Forbid();
			}
		}


		// GET ALL ANIMALS
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
		[HttpGet]
		public async Task<IActionResult> GetAllAnimals()
		{
			AnimalCountDTO res_data = new AnimalCountDTO
			{
				cows = 0,
				goats = 0,
				sheeps = 0,
				camels = 0
			};
			ApiResponse<AnimalCountDTO> response = new ApiResponse<AnimalCountDTO>();
			_logger.LogInformation("GET-ALL-ANIMALS API HIT...");
			try
			{
				var animals = await _context.AnimalDetails.ToListAsync();
				var cows = animals.Where(x => x.AnimalId == 1).ToList().OrderByDescending(x => x.Number).FirstOrDefault();
				var goats = animals.Where(x => x.AnimalId == 2).ToList().OrderByDescending(x => x.Number).FirstOrDefault();
				var sheeps = animals.Where(x => x.AnimalId == 3).ToList().OrderByDescending(x => x.Number).FirstOrDefault();
				var camels = animals.Where(x => x.AnimalId == 4).ToList().OrderByDescending(x => x.Number).FirstOrDefault();


				if (cows != null)
					res_data.cows = (int)cows.Number;
				if (goats != null)
					res_data.goats = (int)goats.Number;
				if (sheeps != null)
					res_data.sheeps = (int)sheeps.Number;
				if (camels != null)
					res_data.camels = (int)camels.Number;

				response.ResponseCode = (int)HttpStatusCode.OK;
				response.ResponseMessage = HttpStatusCode.OK.ToString();
				response.Data = res_data;
				_logger.LogInformation($"GET-ALL-ANIMALS API OK... {response.ResponseMessage}");
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				_logger.LogError($"GET-ALL-ANIMALS API CATCH... {ex.Message}");
				return Forbid();

			}
		}


		// SHARE INPUT DATA FOR REGISTER ANIMAL
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
		[HttpGet]
		public async Task<IActionResult> GetAnimalRegisteration()
		{
			ApiResponse<List<Animal>> response = new ApiResponse<List<Animal>>();
			_logger.LogInformation("GET-ANIMAL-REG API HIT...");
			try
			{
				List<Animal> animal = _context.Animals.ToList();

				response.ResponseCode = (int)HttpStatusCode.OK;
				response.ResponseMessage = HttpStatusCode.OK.ToString();
				response.Data = animal;
				_logger.LogInformation($"GET-ANIMAL-REG API OK... {response.ResponseMessage}");
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				_logger.LogError($"GET-ANIMAL-REG API CATCH... {ex.Message}");
				return Forbid();

			}
		}


		// SHARE NUMBERS AVAILABLE DATA FOR SELECTED ANIMAL
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
		[HttpGet]
		public async Task<IActionResult> GetAnimalNumberAvailableForRegisteration(int AnimalId)
		{
			ApiResponse<List<int>> response = new ApiResponse<List<int>>();
			_logger.LogInformation("GET-ANIMAL-AVAILABLE-FOR-REG API HIT...");
			try
			{
				if (intHelper.IntergerIsNullOrEmpty(AnimalId))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Invalid Animal ID provided";
					_logger.LogError($"GET-ANIMAL-AVAILABLE-FOR-REG API NULL EXCEPTION... {response.ResponseMessage}");
					return BadRequest(response);
				}
				List<AnimalDetail> details = _context.AnimalDetails.Where(e => e.AnimalId == AnimalId).ToList();

				List<int> existingNumbersList = details.Select(item => (int)item.Number).ToList();

				// Generate a list of numbers from 1 to 100
				List<int> allNumbers = Enumerable.Range(1, 200).ToList();

				// Find numbers that are not in the existingNumbersList
				List<int> notExistingNumbers = allNumbers.Except(existingNumbersList).ToList();

				// Create a list of dictionaries for the not existing numbers
				List<int> resultList = notExistingNumbers.Select(num => num).ToList();

				response.ResponseCode = (int)HttpStatusCode.OK;
				response.ResponseMessage = HttpStatusCode.OK.ToString();
				response.Data = resultList;
				_logger.LogInformation($"GET-ANIMAL-AVAILABLE-FOR-REG API OK... {response.ResponseMessage}");
				return Ok(response);
			}
			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
				response.ErrorMessage = "Server Error during the execution. Try Again";
				_logger.LogError($"GET-ANIMAL-AVAILABLE-FOR-REG API CATCH... {ex.Message}");	
				return Forbid();

			}
		}

		// SHARE NUMBERS AVAILABLE FOR DEALING
		[SwaggerResponse((int)HttpStatusCode.OK, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
		[SwaggerResponse((int)HttpStatusCode.Continue, Description = "Products are found and ready to diliver", Type = typeof(ApiResponse<string>))]
		[SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "User is not authorized to access this url", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.NotFound, Description = "No product Found", Type = typeof(ApiResponse<>))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Server has failed to read data", Type = typeof(ApiResponse<>))]
		[Produces("application/json", "application/xml")]
		[Consumes("application/json", "application/xml")]
		[SwaggerOperation(
			Summary = "Get all initial product list",
			Description = "This function returns all products in MongoDB format")]
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAnimalNumberAvailableForDealing(int AnimalId)
		{
			ApiResponse<List<DealAndPartDTO>> response = new ApiResponse<List<DealAndPartDTO>>();
			_logger.LogInformation("GET-ANIMAL-AVAILABLE-FOR-DEAL API HIT...");
			try
			{
				if (intHelper.IntergerIsNullOrEmpty(AnimalId))
				{
					response.ResponseCode = (int)HttpStatusCode.BadRequest;
					response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
					response.ErrorMessage = "Invalid Animal ID provided";
					_logger.LogWarning($"GET-ANIMAL-AVAILABLE-FOR-DEAL API NULL EXCEPTION... {response.ErrorMessage}");
					return BadRequest(response);
				}
				List<AnimalDetail> details = _context.AnimalDetails.Where(e => e.AnimalId == AnimalId).ToList();
				List<Dealing> dealings = new List<Dealing>();
				List<int> existingAdIdList = details.Select(item => (int)item.Adid).ToList();
				var total_parts = _context.Animals.Where(x => x.AnimalId == AnimalId).Select(i => i.Parts).ToList()[0];
				List<DealAndPartDTO> part_data = new List<DealAndPartDTO>();
				if (existingAdIdList.Count > 0)
				{
					foreach (var Adid in existingAdIdList)
					{
						var deal_data = _context.Dealings.Where(e => e.Adid == Adid).ToList();
						var animal_number = _context.AnimalDetails.Where(e => e.Adid == Adid).ToList()[0].Number;
						var price = _context.AnimalDetails.Where(e => e.Adid == Adid).ToList()[0].PartSellPrice;
						if (deal_data.Count > 0)
						{
							List<int> allParts = Enumerable.Range(1, (int)total_parts).ToList();
							var loop_part_data = deal_data.Select(item => item.PartId).ToList();
							allParts.RemoveAll(x => loop_part_data.Contains(x));
							if (allParts.Count == 0)
								continue;
							DealAndPartDTO loop_data = new DealAndPartDTO
							{
								AdId = Adid,
								Number = animal_number,
								Parts = allParts,
								Price = (int?)price
							};

							part_data.Add(loop_data);
						}
						else
						{
							List<int> allParts = Enumerable.Range(1, (int)total_parts).ToList();
							DealAndPartDTO loop_data = new DealAndPartDTO
							{
								AdId = Adid,
								Number = animal_number,
								Parts = allParts,
								Price = (int?)price
							};

							part_data.Add(loop_data);
						}
					}
					response.ResponseCode = (int)HttpStatusCode.OK;
					response.ResponseMessage = HttpStatusCode.OK.ToString();
					response.Data = part_data;
					_logger.LogInformation($"GET-ANIMAL-AVAILABLE-FOR-DEAL API OK... {response.ResponseMessage}");
					return Ok(response);
				}
				else
				{
					response.ResponseCode = (int)HttpStatusCode.Continue;
					response.ResponseMessage = HttpStatusCode.Continue.ToString();
					response.Data = part_data;
					_logger.LogInformation($"GET-ANIMAL-AVAILABLE-FOR-DEAL API OK... {response.ResponseMessage}");
					return Ok(response);
				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = (int)HttpStatusCode.InternalServerError;
				response.ResponseMessage = ex.Message;
				response.ErrorMessage = "Server Error during the execution. Try Again";
				_logger.LogError($"GET-ANIMAL-AVAILABLE-FOR-DEAL API CATCH... {ex.Message}");
				return Forbid();

			}
		}

	}
}
