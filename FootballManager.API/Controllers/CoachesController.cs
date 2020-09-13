using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FootballManager.API.DTOs.Command;
using FootballManager.API.DTOs.Response;
using FootballManager.API.DTOs.Transfer;
using FootballManager.Data;
using FootballManager.Data.Models;
using FootballManager.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Annotations;

namespace FootballManager.API.Controllers
{
    /// <summary>
    ///  A controller for menaging coaches in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoachesController : Controller
    {
        private readonly FootballManagerContext _context;
        private readonly ICoachRepository _repository;
        private readonly ILogger<CoachesController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Initializes a new istance of the <see cref="CoachesController"/> class.
        /// </summary>
        /// <param name="_context">Context instance</param>
        /// <param name="_logger">Logger instance</param>
        /// <param name="_mapper">Mapper instance</param>
        public CoachesController(FootballManagerContext context, ICoachRepository repository, ILogger<CoachesController> logger, IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a coach by surname for a requesting user.
        /// </summary>
        /// <response code="200">If a coach exists returns OK.</response>
        /// <response code="400">If surname claim is missing in the token return BadRequest.</response>
        /// <response code="404">If a coach doesn't exist a response returns NotFound.</response>
        /// <param name="coachSurname">Surname of a specific coach to be retrieved.</param>
        /// <returns>Specific coach for a requesting user.</returns>
        [HttpGet("{surname}")]
        [SwaggerOperation(OperationId = "getSpecificCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CoachResponse>> GetSpecificCoach(string coachSurname)
        {
            try
            {
                var searchedCoach = await _repository.GetSpecificCoach(coachSurname);
                var coachResponse = _mapper.Map<CoachResponse>(searchedCoach);
                _logger.LogInformation($"Coach has been downloaded.");
                return Ok(coachResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download coach.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets coaches for a requesting user.
        /// </summary>
        /// <response code="200">If a list exists returns OK.</response>
        /// <response code="400">If failed to download coaches list in the token return BadRequest.</response>
        /// <response code="404">If a coaches don't exist a response returns NotFound.</response>
        /// <returns>Coaches list for requesting user.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "getAllCoaches")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CoachResponse>>> GetAllCoaches()
        {
            try
            {
                var coaches = await _repository.GetAllCoaches();
                var coachesResponse = coaches.Select(c => _mapper.Map<CoachResponse>(c));

                _logger.LogInformation($"Coaches have been downloaded.");
                return Ok(coachesResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download coaches list.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Update specific coach by surname in a database for a requesting user.
        /// </summary>
        /// <response code="200">If a coach was updated a response returns OK.</response>
        /// <response code="400">If a coach wasn't updated a response returns BadRequest.</response>
        /// <response code="404">If a coach doesn't exist a response returns NotFound.</response>
        /// <param name="coachsurname">Name of a specific club to be updated.</param>
        /// <param name="command">Values to update a specific coach.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPut("surname, command")]
        [SwaggerOperation(OperationId = "updateExistingCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Coach>> PutExistingCoach(string coachsurname, CoachCommand command)
        {
            try
            {
                var coachCurrentClub = command.CurrentClub;
                var updatedCoach = _mapper.Map<Coach>(command);
                await _repository.PutExistingCoach(coachsurname, coachCurrentClub, updatedCoach);
                _logger.LogInformation($"Coach has been updated.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Coach hasn't been updated.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Add specific coach to a database for a requesting user.
        /// </summary>
        /// <response code="200">If a coach was created a response returns OK.</response>
        /// <response code="400">If a coach wasn't added a response returns BadRequest.</response>
        /// <param name="command">New entry to be created.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPost("command")]
        [SwaggerOperation(OperationId = "addNewCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Coach>> PostNewCoach(CoachCommand command)
        {
            try
            {
                var addedCoachCurrentClub = command.CurrentClub;
                var newCoach = _mapper.Map<Coach>(command);
                await _repository.PostNewCoach(addedCoachCurrentClub, newCoach);
                _logger.LogInformation($"Coach has been added.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Coach hasn't been added.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove specific coach by surname from a database for a requesting user.
        /// </summary>
        /// <response code="200">If a coach exists a response returns OK.</response>
        /// <response code="400">If a coach wasn't deleted a response returns BadRequest.</response>
        /// /// <response code="404">If a coach doesn't exist a response returns NotFound.</response>
        /// <param name="coachSurname">Coach's surname to be deleted.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpDelete("{surname}")]
        [SwaggerOperation(OperationId = "removeSpecificCoach")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Coach>> DeleteSpecificCoach(string coachSurname)
        {
            try
            {
                await _repository.DeleteSpecificCoach(coachSurname);
                _logger.LogInformation
                ($"{_context.Coaches.Where(c => c.Surname == coachSurname).Select(c => c.Name)} {coachSurname} has been deleted.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError
                    (ext, $"{_context.Coaches.Where(c => c.Surname == coachSurname).Select(c => c.Name)} {coachSurname} hasn't been deleted.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }
    }
}