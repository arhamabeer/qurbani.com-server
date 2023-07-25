using static Qurabani.com_Server.Responses.SwaggerResponse;
using Qurabani.com_Server.Helpers;
using Microsoft.EntityFrameworkCore;
using Qurabani.com_Server.Models.DTOs;
using Qurabani.com_Server.Models;

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
        //[Auth]
        [HttpPost()]
        public async Task<IActionResult> AddAnimal([FromBody] AnimalDTO animalDTO)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (string.IsNullOrEmpty(animalDTO.partPrice) || intHelper.IntergerIsNullOrEmpty(animalDTO.type) || intHelper.IntergerIsNullOrEmpty(animalDTO.number))
            {
                response.ResponseCode = (int)HttpStatusCode.BadRequest;
                response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                response.ErrorMessage = "Type, Part, Number or Part-Price should not be empty";
                return BadRequest(response);
            };

            if (await _context.AnimalDetails.AnyAsync(e => e.AnimalId == animalDTO.type && e.Number == animalDTO.number))
            {
                response.ResponseCode = (int)HttpStatusCode.BadRequest;
                response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                response.ErrorMessage = "Number is already assigned to another Animal";
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
                return Ok(response);
            }
            response.ResponseCode = (int)HttpStatusCode.InternalServerError;
            response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
            response.ErrorMessage = "Error in saving the new data in the database";
            return Forbid();
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
        //[Auth]
        [HttpPost]
        public async Task<IActionResult> ConfirmDealing([FromBody] DealingDTO dealingDTO)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (string.IsNullOrEmpty(dealingDTO.Address) ||
                    string.IsNullOrEmpty(dealingDTO.Name) ||
                    string.IsNullOrEmpty(dealingDTO.Contact) ||
                    string.IsNullOrEmpty(dealingDTO.EmergencyContact) ||
                    string.IsNullOrEmpty(dealingDTO.Description) ||
                    string.IsNullOrEmpty(dealingDTO.Nic) ||
                    intHelper.IntergerIsNullOrEmpty(dealingDTO.AdId) ||
                    intHelper.IntergerIsNullOrEmpty(dealingDTO.PartId) ||
                    intHelper.IntergerIsNullOrEmpty(dealingDTO.QurbaniDay))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "Every Field is Mandatory and cannot be empty";
                    return BadRequest(response);
                }

                var cond1 = await _context.AnimalParts.FirstOrDefaultAsync(e => e.PartId == dealingDTO.PartId);
                var cond2 = await _context.AnimalDetails.FirstOrDefaultAsync(e => e.Adid == dealingDTO.AdId);
                if (cond1 == null || cond2 == null)
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "Invalid Animal or Part";
                    return BadRequest(response);
                }
                if (await _context.People.AnyAsync(e => e.Nic == dealingDTO.Nic))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "NIC already assigned to an other Animal";
                    return BadRequest(response);
                }
                if (await _context.Dealings.AnyAsync(e => e.Adid == dealingDTO.AdId && e.PartId == dealingDTO.PartId))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "This Animal Part is already assigned to another Person";
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
                        return Ok(response);
                    }
                    response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                    response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                    response.ErrorMessage = "Error in saving the new data in the database";
                    return Forbid();
                }
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                response.ErrorMessage = "Error in saving the new data in the database";
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
        [Auth]
        [HttpGet]
        public async Task<IActionResult> GetDealOfPerson(string nic)
        {
            ApiResponse<DealingDTO> response = new ApiResponse<DealingDTO>();
            try
            {
                if (string.IsNullOrEmpty(nic))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "NIC cannot be empty";
                    return BadRequest(response);
                }
                var person_data = await _context.People.FirstOrDefaultAsync(e => e.Nic == nic);
                if (person_data == null)
                {
                    response.ResponseCode = (int)HttpStatusCode.NotFound;
                    response.ResponseMessage = HttpStatusCode.NotFound.ToString();
                    response.ErrorMessage = "Cannot Find Any Person associated with given NIC number.";
                    return NotFound(response);
                }
                var deal_data = await _context.Dealings.FirstOrDefaultAsync(e => e.PersonId == person_data.PersonId);
                if (deal_data == null)
                {
                    response.ResponseCode = (int)HttpStatusCode.NotFound;
                    response.ResponseMessage = HttpStatusCode.NotFound.ToString();
                    response.ErrorMessage = "No Deal is associated with given NIC number.";
                    return NotFound(response);
                }
                var response_data = new DealingDTO
                {
                    Address = person_data.Address,
                    Contact = person_data.Contact,
                    EmergencyContact = person_data.EmergencyContact,
                    Name = person_data.Name,
                    Nic = nic,
                    QurbaniDay = (int)deal_data.QurbaniDay,
                    Description = deal_data.Description,
                    PartId = (int)deal_data.DealId,
                    AdId = (int)deal_data.Adid,
                    DealId = deal_data.DealId,
                    PickedUp = deal_data.PickedUp,
                    PersonId = person_data.PersonId
                };
                response.ResponseCode = (int)HttpStatusCode.OK;
                response.ResponseMessage = HttpStatusCode.OK.ToString();
                response.Description = "Data Fetched Successfully.";
                response.Data = response_data;
                return Ok(response);
            }
            catch (Exception ex)
            {
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
        [Auth]
        [HttpPost]
        public async Task<IActionResult> IssueDealToPerson(int dealId, int personId)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (intHelper.IntergerIsNullOrEmpty(dealId) || intHelper.IntergerIsNullOrEmpty(dealId))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "Invalid Deal ID or Person ID";
                    return BadRequest(response);
                }
                var deal = await _context.Dealings.FirstOrDefaultAsync(x => x.DealId == dealId && x.PersonId == personId);
                if (deal == null)
                {
                    response.ResponseCode = (int)HttpStatusCode.NotFound;
                    response.ResponseMessage = HttpStatusCode.NotFound.ToString();
                    response.ErrorMessage = "Cannot Find Any Deal associated with given IDs.";
                    return NotFound(response);
                }
                deal.PickedUp = true;
                var res = await _context.SaveChangesAsync();
                if (res > 0)
                {
                    response.ResponseCode = (int)HttpStatusCode.OK;
                    response.ResponseMessage = HttpStatusCode.OK.ToString();
                    response.Data = "Data Updated Successfully.";
                    return Ok(response);
                }
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                response.ErrorMessage = "Server Error during the execution. Try Again";
                return Forbid();
            }
            catch (Exception ex)
            {
                {
                    response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                    response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                    response.ErrorMessage = "Server Error during the execution. Try Again";
                    return Forbid();
                }
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
        //[Auth]
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                response.ErrorMessage = "Server Error during the execution. Try Again";
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
        //[Auth]
        [HttpGet]
        public async Task<IActionResult> GetAnimalRegisteration()
        {
            ApiResponse<List<Animal>> response = new ApiResponse<List<Animal>>();
            try
            {
                List<Animal> animal = _context.Animals.ToList();

                response.ResponseCode = (int)HttpStatusCode.OK;
                response.ResponseMessage = HttpStatusCode.OK.ToString();
                response.Data = animal;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                response.ErrorMessage = "Server Error during the execution. Try Again";
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
        //[Auth]
        [HttpGet]
        public async Task<IActionResult> GetAnimalNumberAvailableForRegisteration(int AnimalId)
        {
            ApiResponse<List<int>> response = new ApiResponse<List<int>>();
            try
            {
                if (intHelper.IntergerIsNullOrEmpty(AnimalId))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "Invalid Animal ID provided";
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
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = HttpStatusCode.InternalServerError.ToString();
                response.ErrorMessage = "Server Error during the execution. Try Again";
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
        //[Auth]
        [HttpGet]
        public async Task<IActionResult> GetAnimalNumberAvailableForDealing(int AnimalId)
        {
            ApiResponse<List<DealAndPartDTO>> response = new ApiResponse<List<DealAndPartDTO>>();
            try
            {
                if (intHelper.IntergerIsNullOrEmpty(AnimalId))
                {
                    response.ResponseCode = (int)HttpStatusCode.BadRequest;
                    response.ResponseMessage = HttpStatusCode.BadRequest.ToString();
                    response.ErrorMessage = "Invalid Animal ID provided";
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
                        if (deal_data.Count > 0)
                        {
                            List<int> allParts = Enumerable.Range(1, (int)total_parts).ToList();
                            var loop_part_data = deal_data.Select(item => item.PartId).ToList();
                            allParts.RemoveAll(x => loop_part_data.Contains(x));
                            DealAndPartDTO loop_data = new DealAndPartDTO
                            {
                                AdId = Adid,
                                Number = animal_number,
                                Parts = allParts
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
								Parts = allParts
							};

							part_data.Add(loop_data);
						}
                    }
                    response.ResponseCode = (int)HttpStatusCode.OK;
                    response.ResponseMessage = HttpStatusCode.OK.ToString();
                    response.Data = part_data;
                    return Ok(response);
                }
                else
                {
                    response.ResponseCode = (int)HttpStatusCode.Continue;
                    response.ResponseMessage = HttpStatusCode.Continue.ToString();
                    response.Data = part_data;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.ResponseCode = (int)HttpStatusCode.InternalServerError;
                response.ResponseMessage = ex.Message;
                response.ErrorMessage = "Server Error during the execution. Try Again";
                return Forbid();

            }
        }

    }
}
