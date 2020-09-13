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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Annotations;

namespace FootballManager.API.Controllers
{
    /// <summary>
    ///  A controller for menaging stadiums in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumsController : Controller
    {
        private readonly FootballManagerContext _context;
        private readonly IStadiumRepository _repository;
        private readonly ILogger<StadiumsController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Initializes a new istance of the <see cref="StadiumsController"/> class.
        /// </summary>
        /// <param name="_context">Context instance</param>
        /// <param name="_logger">Logger instance</param>
        /// <param name="_mapper">Mapper instance</param>
        public StadiumsController(FootballManagerContext context, IStadiumRepository repository, ILogger<StadiumsController> logger, IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a stadium by name for a requesting user.
        /// </summary>
        /// <response code="200">If a stadium exists returns OK.</response>
        /// <response code="400">If stadium's name claim is missing in the token return BadRequest.</response>
        /// <response code="404">If a stadium doesn't exist a response returns NotFound.</response>
        /// <param name="stadiumName">Name of a specific stadium to be retrieved.</param>
        /// <returns>Specific stadium for a requesting user.</returns>
        [HttpGet("{stadiumName}")]
        [SwaggerOperation(OperationId = "getSpecificStadium")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StadiumResponse>> GetSpecificStadium(string stadiumName)
        {
            try
            {
                var searchedStadium = await _repository.GetSpecificStadium(stadiumName);                
                var stadiumResponseMapping = _mapper.Map<StadiumResponse>(searchedStadium);
                _logger.LogInformation($"Stadium has been downloaded.");
                return Ok(stadiumResponseMapping);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download stadium.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets stadiums for a requesting user.
        /// </summary>
        /// <response code="200">If a list exists returns OK.</response>
        /// <response code="400">If failed to download stadiums list in the token return BadRequest.</response>
        /// <response code="404">If a stadiums don't exist a response returns NotFound.</response>
        /// <returns>Stadiums list for requesting user.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "getAllStadiums")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<StadiumResponse>>> GetAllStadiums()
        {
            try
            {
                var stadiums = await _repository.GetAllStadiums();
                var stadiumsResponseMapping = new List<StadiumResponse>();

                foreach (var s in stadiums)
                {
                    var stadiumResponseMapping = _mapper.Map<StadiumResponse>(s);
                    stadiumsResponseMapping.Add(stadiumResponseMapping);
                }

                _logger.LogInformation($"Stadiums have been downloaded.");
                return Ok(stadiumsResponseMapping);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download stadiums list.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Update specific stadium by name in a database for a requesting user.
        /// </summary>
        /// <response code="200">If a stadium was updated a response returns OK.</response>
        /// <response code="400">If a stadium wasn't updated a response returns BadRequest.</response>
        /// <response code="404">If a stadium doesn't exist a response returns NotFound.</response>
        /// <param name="stadiumName">Name of a specific stadium to be updated.</param>
        /// <param name="command">Values to update a specific stadium.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPut("stadiumName, command")]
        [SwaggerOperation(OperationId = "updateExistingStadium")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Stadium>> PutExistingStadium(string stadiumName, StadiumCommand command)
        {
            try
            {
                var stadiumClub = command.ClubName;
                var updatedStadium = _mapper.Map<Stadium>(command);
                await _repository.PutExistingStadium(stadiumName, stadiumClub, updatedStadium);
                _logger.LogInformation($"Stadium has been updated.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Stadium hasn't been updated.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Add specific stadium to a database for a requesting user.
        /// </summary>
        /// <response code="200">If a stadium was created a response returns OK.</response>
        /// <response code="400">If a stadium wasn't added a response returns BadRequest.</response>
        /// <param name="command">New entry to be created.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPost("command")]
        [SwaggerOperation(OperationId = "addNewStadium")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Stadium>> PostNewStadium(StadiumCommand command)
        {
            try
            {
                var addedStadiumClub = command.ClubName;
                var newStadium = _mapper.Map<Stadium>(command);
                await _repository.PostNewStadium(addedStadiumClub, newStadium);
                _logger.LogInformation($"Stadium has been added.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Stadium hasn't been added.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove specific stadium by name from a database for a requesting user.
        /// </summary>
        /// <response code="200">If a stadium exists a response returns OK.</response>
        /// <response code="400">If a stadium wasn't deleted a response returns BadRequest.</response>
        /// /// <response code="404">If a stadium doesn't exist a response returns NotFound.</response>
        /// <param name="stadiumName">Stadium's surname to be deleted.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpDelete("{stadiumName}")]
        [SwaggerOperation(OperationId = "removeSpecificStadium")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Stadium>> DeleteSpecificStadium(string stadiumName)
        {
            try
            {
                await _repository.DeleteSpecificStadium(stadiumName);
                _logger.LogInformation
                ($"{_context.Stadiums.Where(s => s.StadiumName == stadiumName).Select(s => s.StadiumName)} {stadiumName} has been deleted.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError
                    (ext, $"{_context.Stadiums.Where(s => s.StadiumName == stadiumName).Select(s => s.StadiumName)} {stadiumName} hasn't been deleted.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }
    }
}