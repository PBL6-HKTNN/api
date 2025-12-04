using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.BuildingBlocks.Core.Models
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                var endpoint = context.GetEndpoint();
                var actionAttr = endpoint?.Metadata.GetMetadata<RequireActionAttribute>();

                if (actionAttr != null)
                {
                    var userActions = context.User.Claims
                        .Where(c => c.Type == "permissions")
                        .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        .Select(x => x.Trim())
                        .ToList();

                    if (!userActions.Contains(actionAttr.ActionCode))
                    {

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            status = context.Response.StatusCode,
                            isSuccess = false,
                            error = $"You do not have permission: {actionAttr.ActionCode}"
                        });

                        return;
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    status = context.Response.StatusCode,
                    error = "Internal server error occurred during permission validation.",
                    isSuccess = false,
                });
            }
        }
    }
}
