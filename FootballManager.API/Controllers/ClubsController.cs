using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FootballManager.API.DTOs.Command;
using FootballManager.API.DTOs.Response;
using FootballManager.Data;
using FootballManager.Data.Models;
using FootballManager.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Annotations;

namespace FootballManager.API.Controllers
{
    /// <summary>
    ///  A controller for menaging clubs in database.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController : Controller
    {
        private readonly FootballManagerContext _context;
        private readonly IClubRepository _repository;
        private readonly ILogger<ClubsController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        ///  Initializes a new istance of the <see cref="ClubsController"/> class.
        /// </summary>
        /// <param name="_context">Context instance</param>
        /// <param name="_logger">Logger instance</param>
        /// <param name="_mapper">Mapper instance</param>
        public ClubsController(FootballManagerContext context, IClubRepository repository, ILogger<ClubsController> logger, IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a club by club's name for a requesting user.
        /// </summary>
        /// <response code="200">If a club exists returns OK.</response>
        /// <response code="400">If clubName claim is missing in the token return BadRequest.</response>
        /// <response code="404">If a club doesn't exist a response returns NotFound.</response>
        /// <param name="clubName">Name of a specific club to be retrieved.</param>
        /// <returns>Specific club for a requesting user.</returns>
        [HttpGet("{clubName}")]
        [SwaggerOperation(OperationId = "getSpecificClub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ClubResponse>>> GetSpecificClub(string clubName)
        {
            try
            {
                var searchedClub = await _repository.GetSpecificClub(clubName);
                var clubResponse = _mapper.Map<ClubResponse>(searchedClub);
                _logger.LogInformation($"Club has been downloaded.");
                return Ok(clubResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download club.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Gets clubs for a requesting user.
        /// </summary>
        /// <response code="200">If a list exists returns OK.</response>
        /// <response code="400">If failed to download clubs list in the token return BadRequest.</response>
        /// <response code="404">If a clubs don't exist a response returns NotFound.</response>
        /// <returns>Clubs list for requesting user.</returns>
        [HttpGet]
        [SwaggerOperation(OperationId = "getAllClubs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClubResponse>> GetAllClubs()
        {
            try
            {
                var clubs = await _repository.GetAllClubs();
                var clubsResponse = new List<ClubResponse>();

                foreach (var c in clubs)
                {
                    var clubResponse = _mapper.Map<ClubResponse>(c);
                    clubsResponse.Add(clubResponse);
                }

                _logger.LogInformation($"Clubs have been downloaded.");
                return Ok(clubsResponse);
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Failed to download clubs list.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Update specific club by club's name in a database for a requesting user.
        /// </summary>
        /// <response code="200">If a club was updated a response returns OK.</response>
        /// <response code="400">If a club wasn't updated a response returns BadRequest.</response>
        /// <response code="404">If a club doesn't exist a response returns NotFound.</response>
        /// <param name="clubName">Name of a specific club to be updated.</param>
        /// <param name="command">Values to update a specific club.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPut("clubName, command")]
        [SwaggerOperation(OperationId = "updateExistingClub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Club>> PutExistingClub(string clubName, ClubCommand command)
        {
            try
            {
                var updatedClub = _mapper.Map<Club>(command);
                await _repository.PutExistingClub(clubName, updatedClub);
                _logger.LogInformation($"Club has been updated.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Club hasn't been updated.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Add specific club to a database for a requesting user.
        /// </summary>
        /// <response code="200">If a club was created a response returns OK.</response>
        /// <response code="400">If a club wasn't added a response returns BadRequest.</response>
        /// <param name="command">New entry to be created.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpPost("command")]
        [SwaggerOperation(OperationId = "addNewClub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Club>> PostNewClub(ClubCommand command)
        {
            try
            {
                var newClub = _mapper.Map<Club>(command);
                await _repository.PostNewClub(newClub);
                _logger.LogInformation($"Club has been added.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, "Club hasn't been added.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }

        /// <summary>
        /// Remove specific club by club's name from a database for a requesting user.
        /// </summary>
        /// <response code="200">If a club exists a response returns OK.</response>
        /// <response code="400">If a club wasn't deleted a response returns BadRequest.</response>
        /// /// <response code="404">If a club doesn't exist a response returns NotFound.</response>
        /// <param name="clubName">Club's name to be deleted.</param>
        /// <returns>Status code describing result of the action.</returns>
        [HttpDelete("{clubName}")]
        [SwaggerOperation(OperationId = "removeSpecificClub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Club>> DeleteSpecificClub(string clubName)
        {
            try
            {
                await _repository.DeleteSpecificClub(clubName);
                _logger.LogInformation($"{clubName} has been deleted.");
                return Ok();
            }
            catch (System.Exception ext)
            {
                _logger.LogError(ext, $"{clubName} hasn't been deleted.");
                // TODO return error object with proper error code.
                return BadRequest();
            }
        }
    }
}