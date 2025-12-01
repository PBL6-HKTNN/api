using Codemy.BuildingBlocks.Core;
using Codemy.Identity.Application.DTOs.Request;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Codemy.Identity.Application.Services
{
    internal class RequestService : IRequestService
    {
        private readonly ILogger<RequestService> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<RequestType> _requestTypeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public RequestService(
            ILogger<RequestService> logger,
            IRepository<User> userRepository,
            IRepository<Request> requestRepository,
            IRepository<RequestType> requestTypeRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _requestTypeRepository = requestTypeRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse> CreateRequestAsync(CreateRequestDTO createRequestDTO)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            var userExists = await _userRepository.GetByIdAsync(UserId);
            if (userExists == null || userExists.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var requestType = await _requestTypeRepository.GetByIdAsync(createRequestDTO.RequestTypeId);
            if (requestType == null || requestType.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Invalid request type."
                };
            }
            Request request = new Request();
            if (requestType.Type == RequestTypeEnum.UpgradeToInstructor)
            {
                if (userExists.role != Role.Student)
                {
                    return new RequestResponse
                    {
                        Success = false,
                        Message = "Only Student can request to upgrade to Instructor."
                    };
                }
            }
            request.UserId = UserId;
            request.Description = createRequestDTO.Description;
            request.RequestTypeId = createRequestDTO.RequestTypeId;
            request.CreatedBy = UserId;

            if (requestType.Type == RequestTypeEnum.PublicCourseRequest || requestType.Type == RequestTypeEnum.HideCourseRequest)
            {
                // check course có tồn tại
                // check user có phải instructor và là instructor của course đó ko
                if (userExists.role != Role.Instructor || )
                {
                    return new RequestResponse
                    {
                        Success = false,
                        Message = "Only Student can request to upgrade to Instructor."
                    };
                }
            }

            
        }

        public async Task<RequestResponse> DeleteRequestAsync(Guid requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Request not found.",
                    request = null
                };
            }
            request.IsDeleted = true;
            await _requestRepository.UpdateAsync(request);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Failed to delete request.",
                    request = null
                };
            }
            return new RequestResponse
            {
                Success = true,
                Message = "Request deleted successfully.",
                request = request
            };
        }

        public async Task<RequestResponse> GetRequestByIdAsync(Guid requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Request not found.",
                    request = null
                };
            }
            return new RequestResponse
            {
                Success = true,
                Message = "Request retrieved successfully.",
                request = request
            };
        }

        public async Task<ListRequestResponse> GetRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            var requestFiltered = requests.Where(r => !r.IsDeleted).ToList();
            return new ListRequestResponse
            {
                Success = true,
                Message = "Request types retrieved successfully.",
                requests = requestFiltered
            };
        }

        public async Task<ListRequestTypeResponse> GetRequestTypesAsync()
        {
            var requestTypes = await _requestTypeRepository.GetAllAsync();
            var requestTypeFiltered = requestTypes.Where(rt => !rt.IsDeleted).ToList();
            return new ListRequestTypeResponse
            {
                Success = true,
                Message = "Request types retrieved successfully.",
                types = requestTypeFiltered
            };
        }

        public Task<RequestResponse> UpdateRequestAsync(Guid requestId, UpdateRequestDTO updateRequestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
