using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FootballManager.API.DTOs.Command;
using FootballManager.API.DTOs.Response;
using FootballManager.Data;
using FootballManager.Data.Models;
using FootballManager.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Annotations;

namespace FootballManager.API.Controllers
{
    /// <summary>
    ///  A controller for menaging footballers in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FootballersController : Controller
    {
        private readonly FootballManagerContext _context;
        private readonly IFootballerRepository _repository;
        private readonly ILogger<FootballersController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Initializes a new istance of the <see cref="FootballersController"/> class.
        /// </summary>
        /// <param name="_context">Context instance</param>
        /// <param name="_logger">Logger instance</param>
        /// <param name="_mapper">Mapper instance</param>
        public FootballersController(FootballManagerContext context, IFootballerRepository repository, ILogger<FootballersController> logger, IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a footballer by surname for a requesting user.
        /// </summary>
        /// <response code="200">If a footballer exists returns OK.</response>
        /// <response code="400">If surname claim is missing in the token return BadRequest.</response>
        /// <response code="404">If a footballer doesn't exist a response returns NotFound.</response>
        /// <param name="footballerSurname">Surname of a specific footballer to be retrieved.</param>
        /// <returns>Specific footballer for a requesting user.</returns>
        [HttpGet("{surname}")]
        [SwaggerOperation(OperationId = "getSpecificFootballer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FootballerResponse>> GetSpecificFootballer(string footballerSurname)
        {
            try
            {
                var searchedFootballer = await _repository.GetSpecificFootballer(footballerSurname);                
                var footballerResponse = _mapper.Map<FootballerResponse>(searchedFootballer);
                _logger.LogInformation($"Footballer has been downloaded.");
                return Ok(footballerResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download footballer.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets footballers for a requesting user.
        /// </summary>
        /// <response code="200">If a list exists returns OK.</response>
        /// <response code="400">If failed to download footballers list in the token return BadRequest.</response>
        /// <response code="404">If a footballers don't exist a response returns NotFound.</response>
        /// <returns>Footballers list for requesting user.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "getAllFootballers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FootballerResponse>>> GetAllFootballers()
        {
            try
            {
                var footballers = await _repository.GetAllFootballers();
                var footballersResponse = new List<FootballerResponse>();

                foreach (var f in footballers)
                {
                    var footballerResponse = _mapper.Map<FootballerResponse>(f);
                    footballersResponse.Add(footballerResponse);
                }

                _logger.LogInformation($"Footballers have been downloaded.");
                return Ok(footballersResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download footballers list.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Update specific footballer by surname in a database for a requesting user.
        /// </summary>
        /// <response code="200">If a footballer was updated a response returns OK.</response>
        /// <response code="400">If a footballer wasn't updated a response returns BadRequest.</response>
        /// <response code="404">If a footballer doesn't exist a response returns NotFound.</response>
        /// <param name="footballerSurname">Name of a specific footballer to be updated.</param>
        /// <param name="command">Values to update a specific footballer.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPut("surname, command")]
        [SwaggerOperation(OperationId = "updateExistingFootballer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Footballer>> PutExistingFootballer(string footballerSurname, FootballerCommand command)
        {
            try
            {
                var footballerCurrentClub = command.CurrentClub;
                var updatedFootballer = _mapper.Map<Footballer>(command);
                await _repository.PutExistingFootballer(footballerSurname, footballerCurrentClub, updatedFootballer);
                _logger.LogInformation($"Footballer has been updated.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Footballer hasn't been updated.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Add specific footballer to a database for a requesting user.
        /// </summary>
        /// <response code="200">If a footballer was created a response returns OK.</response>
        /// <response code="400">If a footballer wasn't added a response returns BadRequest.</response>
        /// <param name="command">New entry to be created.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPost("command")]
        [SwaggerOperation(OperationId = "addNewFootballer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Footballer>> PostNewFootballer(FootballerCommand command)
        {
            try
            {
                var addedFootballerCurrentClub = command.CurrentClub;
                var newFootballer = _mapper.Map<Footballer>(command);
                await _repository.PostNewFootballer(addedFootballerCurrentClub, newFootballer);
                _logger.LogInformation($"Footballer has been added.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Footballer hasn't been added.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove specific footballer by surname from a database for a requesting user.
        /// </summary>
        /// <response code="200">If a footballer exists a response returns OK.</response>
        /// <response code="400">If a footballer wasn't deleted a response returns BadRequest.</response>
        /// /// <response code="404">If a footballer doesn't exist a response returns NotFound.</response>
        /// <param name="footballerSurname">Footballer's surname to be deleted.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpDelete("{surname}")]
        [SwaggerOperation(OperationId = "removeSpecificCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Footballer>> DeleteSpecificFootballer(string footballerSurname)
        {
            try
            {
                await _repository.DeleteSpecificFootballer(footballerSurname);
                _logger.LogInformation
                ($"{_context.Footballers.Where(c => c.Surname == footballerSurname).Select(c => c.Name)} {footballerSurname} has been deleted.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError
                    (ext, $"{_context.Footballers.Where(c => c.Surname == footballerSurname).Select(c => c.Name)} {footballerSurname} hasn't been deleted.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }
    }
}