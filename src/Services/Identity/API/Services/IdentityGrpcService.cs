using Codemy.Identity.Application.Interfaces;
using Codemy.IdentityProto; 
using Grpc.Core; 
namespace Codemy.Identity.API.Services
{
    public class IdentityGrpcService : IdentityService.IdentityServiceBase
    {
        private readonly IAuthenticationService _authenticationService;

        public IdentityGrpcService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
        {
            var result = await _authenticationService.GetUserById(request.UserId);
            if (result == null)
            {
                Console.WriteLine($"User {request.UserId} không tồn tại");
                return new GetUserByIdResponse
                {
                    Exists = false
                };
            }

            Console.WriteLine($"User found: {result.name} ({result.email})");

            return new GetUserByIdResponse
            {
                Exists = true,
                UserId = result.Id.ToString(),
                Email = result.email,
                Name = result.name
            };
        }
    }
}