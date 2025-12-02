using Codemy.BuildingBlocks.Core.Models;
using Codemy.BuildingBlocks.Core;
using Codemy.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Codemy.Identity.Application.DTOs.Request;

namespace Codemy.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestController> _logger;
        public RequestController(
            IRequestService requestService,
            ILogger<RequestController> logger)
        {
            _requestService = requestService;
            _logger = logger;
        }

        [HttpGet]
        [RequireAction("REQUEST_READ")]
        public async Task<IActionResult> GetRequests()
        {
            try
            {
                var result = await _requestService.GetRequestsAsync();
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve requests.",
                        "Request retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving requests.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("/my-request")]
        [RequireAction("REQUEST_READ")]
        public async Task<IActionResult> GetMyRequests()
        {
            try
            {
                var result = await _requestService.GetMyRequestsAsync();
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve requests.",
                        "Request retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving requests.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("/request-resolved-by-me")]
        [RequireAction("REQUEST_READ")]
        public async Task<IActionResult> GetRequestsResolvedByMe()
        {
            try
            {
                var result = await _requestService.GetRequestsResolvedByMe();
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve requests.",
                        "Request retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving requests.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("/request-type")]
        [RequireAction("REQUEST_READ")]
        public async Task<IActionResult> GetRequestTypes()
        {
            try
            {
                var result = await _requestService.GetRequestTypesAsync();
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve request types.",
                        "Request retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving request types.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request type retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("get/{requestId}")]
        [RequireAction("REQUEST_READ")]
        public async Task<IActionResult> GetRequestById(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                return this.BadRequestResponse(
                    "Invalid request ID provided.",
                    "The request ID cannot be empty."
                );
            }
            try
            {
                var result = await _requestService.GetRequestByIdAsync(requestId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve request.",
                        "Request retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving request.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request retrieval",
                    ex.Message
                );
            }
        }

        [HttpPost("create")]
        [RequireAction("REQUEST_CREATE")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequestDTO createRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _requestService.CreateRequestAsync(createRequestDTO);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create request.",
                        "Request creation failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating request.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request creation",
                    ex.Message
                );
            }
        }

        [HttpPut("update/{requestId}")]
        [RequireAction("REQUEST_UPDATE")]
        public async Task<IActionResult> UpdateRequest(Guid requestId, [FromBody] UpdateRequestDTO updateRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _requestService.UpdateRequestAsync(requestId, updateRequestDTO);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update request.",
                        "Request update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating request.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request update",
                    ex.Message
                );
            }
        }

        [HttpDelete("delete/{requestId}")]
        [RequireAction("REQUEST_DELETE")]
        public async Task<IActionResult> DeleteRequest(Guid requestId)
        {
            try
            {
                var result = await _requestService.DeleteRequestAsync(requestId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete request.",
                        "Request deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse("Request deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting request.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request deletion",
                    ex.Message
                );
            }
        }

        [HttpPost("resolve/{requestId}")]
        [RequireAction("REQUEST_RESOLVE")]
        public async Task<IActionResult> ResolveRequest(Guid requestId, [FromBody] ResolveRequestDTO updateRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _requestService.ResolveRequestAsync(requestId, updateRequestDTO);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to resolve request.",
                        "Request deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse("Request resolved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolve request.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request resolve",
                    ex.Message
                );
            }
        }
    }
}
