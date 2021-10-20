using ActivityHistoryApi.V1.Boundary.Request;
using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/activityHistory")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class ActivityHistoryApiController : BaseController
    {
        private readonly IGetByTargetIdUseCase _getByTargetIdUseCase;
        public ActivityHistoryApiController(IGetByTargetIdUseCase getByTargetIdUseCase)
        {
            _getByTargetIdUseCase = getByTargetIdUseCase;
        }

        /// <summary>
        /// Retrieves activity history for the supplied targetId.
        /// If a pagination token is provided (returned from a previous call where the results set > 10)
        /// then the query will continue from the last record returned in the preivious query.
        /// </summary>
        /// <response code="200">Returns the list of activity hhistroy for the supplied target id.</response>
        /// <response code="400">Invalid Query Parameter.</response>
        /// <response code="404">No activity history found for the supplied targetId</response>
        [ProducesResponseType(typeof(List<ActivityHistoryResponseObject>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public async Task<IActionResult> GetByTargetIdAsync([FromQuery] GetActivityHistoryByTargetIdQuery query)
        {
            var response = await _getByTargetIdUseCase.ExecuteAsync(query).ConfigureAwait(false);
            if ((null == response) || !response.Results.Any()) return NotFound(query.TargetId);

            return Ok(response);
        }
    }
}
