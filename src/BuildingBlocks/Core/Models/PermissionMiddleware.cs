using Microsoft.AspNetCore.Http;

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
            var endpoint = context.GetEndpoint();
            var actionAttr = endpoint?.Metadata.GetMetadata<RequireActionAttribute>();

            if (actionAttr != null)
            {
                var userActions = context.User.Claims
                    .Where(c => c.Type == "permissions")
                    .SelectMany(c => c.Value.Split(','))
                    .Select(x => x.Trim())
                    .ToList();

                if (!userActions.Contains(actionAttr.ActionCode))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("You do not have permission.");
                    return;
                }
            }

            await _next(context);
        }
    }

}
