using Codemy.BuildingBlocks.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.BuildingBlocks.Core
{
    public static class ApiResponseExtensions
    {
        // Success responses
        public static IActionResult OkResponse<T>(this ControllerBase controller, T data, string? message = null)
        {
            var response = ApiResponse<T>.Success(data);
            return controller.Ok(response);
        }

        public static IActionResult CreatedResponse<T>(this ControllerBase controller, T data, string location = "")
        {
            var response = ApiResponse<T>.Success(data, StatusCodes.Status201Created);
            return controller.Created(location, response);
        }

        // Error responses
        public static IActionResult BadRequestResponse(this ControllerBase controller, string message, string? details = null)
        {
            var response = ApiResponse<object>.Failure(message, StatusCodes.Status400BadRequest, details);
            return controller.BadRequest(response);
        }

        public static IActionResult UnauthorizedResponse(this ControllerBase controller, string message = "Unauthorized", string? details = null)
        {
            var response = ApiResponse<object>.Failure(message, StatusCodes.Status401Unauthorized, details);
            return controller.StatusCode(StatusCodes.Status401Unauthorized, response);
        }

        public static IActionResult ForbiddenResponse(this ControllerBase controller, string message = "Forbidden", string? details = null)
        {
            var response = ApiResponse<object>.Failure(message, StatusCodes.Status403Forbidden, details);
            return controller.StatusCode(StatusCodes.Status403Forbidden, response);
        }

        public static IActionResult NotFoundResponse(this ControllerBase controller, string message = "Resource not found", string? details = null)
        {
            var response = ApiResponse<object>.Failure(message, StatusCodes.Status404NotFound, details);
            return controller.NotFound(response);
        }

        public static IActionResult InternalServerErrorResponse(this ControllerBase controller, string message = "Internal server error", string? details = null)
        {
            var response = ApiResponse<object>.Failure(message, StatusCodes.Status500InternalServerError, details);
            return controller.StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        // Validation error response
        public static IActionResult ValidationErrorResponse(this ControllerBase controller, Dictionary<string, string[]> validationErrors)
        {
            var response = new ApiResponse<object>
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Data = null,
                Error = new ErrorResponse
                {
                    Message = "Validation failed",
                    ValidationErrors = validationErrors
                }
            };
            return controller.UnprocessableEntity(response);
        }
    }
}
