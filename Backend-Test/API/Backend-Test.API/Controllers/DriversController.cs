using Microsoft.AspNetCore.Mvc;
using MediatR;
using FluentValidation;
using Backend_Test.Application.Commands;
using Backend_Test.Application.Queries;
using Backend_Test.Domain.Common.Exceptions;
using Backend_Test.Domain.Entities;

namespace Backend_Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DriversController> _logger;

        public DriversController(IMediator mediator, ILogger<DriversController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Get drivers By Pagenation
        /// Filter Optional firstName or lastName or email or phoneNumber
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDrivers(
            [FromQuery] string firstName = null,
            [FromQuery] string lastName = null,
            [FromQuery] string email = null,
            [FromQuery] string phoneNumber = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 10)
        {
            _logger.LogInformation("GetDrivers request received with filters: FirstName={FirstName}, LastName={LastName}, Email={Email}, PhoneNumber={PhoneNumber}, Skip={Skip}, Take={Take}",
                firstName, lastName, email, phoneNumber, skip, take);

            var query = new GetDriversQuery
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Skip = skip,
                Take = take
            };

            var drivers = await _mediator.Send(query);

            return Ok(drivers);
        }

        /// <summary>
        /// Get Driver By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDriver(Guid id)
        {
            _logger.LogInformation("GetDriver request received for Id={DriverId}", id);

            var driver = await _mediator.Send(new GetDriverByIdQuery(id));

            if (driver == null)
            {
                _logger.LogWarning("Driver with Id={DriverId} not found", id);
                return NotFound(new { Message = $"Driver with Id '{id}' not found." });
            }

            return Ok(driver);
        }

        /// <summary>
        /// Create Driver
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromBody] CreateDriverCommand command)
        {
            _logger.LogInformation("CreateDriver request received: Email={Email}", command.Email);

            try
            {
                var driverId = await _mediator.Send(command);
                _logger.LogInformation("Driver successfully created: Id={DriverId}", driverId);

                return CreatedAtAction(nameof(GetDriver), new { id = driverId }, new { Id = driverId });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed when creating driver: {Errors}", ex.Errors);
                return BadRequest(ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating driver.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Update Driver
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateDriver(Guid id, [FromBody] UpdateDriverCommand command)
        {
            _logger.LogInformation("UpdateDriver request received for Id={DriverId}", id);

            if (id != command.Id)
            {
                _logger.LogWarning("Mismatch in Driver ID between route ({RouteId}) and payload ({PayloadId})", id, command.Id);
                return BadRequest("Route ID and command ID mismatch.");
            }

            try
            {
                await _mediator.Send(command);
                _logger.LogInformation("Driver updated successfully: Id={DriverId}", id);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error when updating driver {DriverId}: {Errors}", id, ex.Errors);
                return BadRequest(ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating driver {DriverId}.", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// DELETE Driver
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            _logger.LogInformation("DeleteDriver request received for Id={DriverId}", id);

            try
            {
                await _mediator.Send(new DeleteDriverCommand(id));
                _logger.LogInformation("Driver successfully deleted: Id={DriverId}", id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting driver Id={DriverId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }
        /// <summary>
        /// Generate random 10 Drivers
        /// </summary>
        /// <returns></returns>
        [HttpPost("generate-random")]
        public async Task<IActionResult> GenerateRandomDrivers()
        {
            var command = new GenerateRandomDriversCommand();
            var createdDrivers = await _mediator.Send(command);

            return Ok(createdDrivers);
        }
        /// <summary>
        /// display an alphabetized list of the current users in the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("alphabetized")]
        public async Task<IActionResult> GetAlphabetizedDrivers()
        {
            var drivers = await _mediator.Send(new GetAlphabetizedDriversQuery());
            return Ok(drivers);
        }
        /// <summary>
        /// return the user’s name By Id alphabetized as well
        /// Example return Oliver Johnson alphabetized
        /// Example output: eilOrv hJnnoos
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}/alphabetized-name")]
        public async Task<IActionResult> GetDriverNameAlphabetized(Guid id)
        {
            var result = await _mediator.Send(new GetDriverNameAlphabetizedQuery(id));
            if (result == null)
                return NotFound($"Driver with Id '{id}' not found.");

            return Ok(result);
        }
    }
}
