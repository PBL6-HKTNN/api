using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Identity.Application.DTOs.Request;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
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
        private readonly CoursesService.CoursesServiceClient _courseClient;
        private readonly EmailSender _emailSender;

        public RequestService(
            ILogger<RequestService> logger,
            IRepository<User> userRepository,
            IRepository<Request> requestRepository,
            IRepository<RequestType> requestTypeRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            CoursesService.CoursesServiceClient courseClient,
            EmailSender emailSender)
        {
            _logger = logger;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _requestTypeRepository = requestTypeRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _courseClient = courseClient;
            _emailSender = emailSender;
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
            _logger.LogInformation($"Creating request of type: {requestType.Type} for user: {UserId}");
            Request request = new Request();
            if (requestType.Type == RequestTypeEnum.UpgradeToInstructor)
            {
                _logger.LogInformation("Processing UpgradeToInstructor request.");
                _logger.LogInformation($"Type:{ RequestTypeEnum.UpgradeToInstructor}");
                if (userExists.role != Role.Student)
                {
                    return new RequestResponse
                    {
                        Success = false,
                        Message = "Only Student can request to upgrade to Instructor."
                    };
                }
            }


            if (requestType.Type == RequestTypeEnum.PublicCourseRequest || requestType.Type == RequestTypeEnum.HideCourseRequest)
            {
                // check course có tồn tại
                var courseResponse = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest
                {
                    CourseId = createRequestDTO.courseId.ToString()
                });
                if (!courseResponse.Exists)
                {
                    return new RequestResponse
                    {
                        Success = false,
                        Message = "Course does not exist."
                    };
                }
                // check user có phải instructor và là instructor của course đó ko
                if (userExists.role != Role.Instructor || UserId != Guid.Parse(courseResponse.InstructorId))
                {
                    _logger.LogInformation($"Role: {userExists.role}, InstructorId: {courseResponse.InstructorId}, UserId: {UserId}");
                    _logger.LogWarning($"User {UserId} is not instructor of course {createRequestDTO.courseId}. Instructor Id of course: {courseResponse.InstructorId}");
                    return new RequestResponse
                    {
                        Success = false,
                        Message = "Current user is not instructor of this course."
                    };
                }
                request.CourseId = createRequestDTO.courseId;
            }

            if (requestType.Type == RequestTypeEnum.PublicCourseRequest)
            {
                //check thông tin khóa học
                var resultCheckCourseSpam = _courseClient.AutoCheckCourseAsync(new GetCourseByIdRequest
                {
                    CourseId = createRequestDTO.courseId.ToString()
                });
                if (!resultCheckCourseSpam.Success)
                {
                    return new RequestResponse
                    {
                        Success = false,
                        Message = resultCheckCourseSpam.Message
                    };
                }
            }

            request.UserId = UserId;
            request.Description = createRequestDTO.Description;
            request.RequestTypeId = createRequestDTO.RequestTypeId;
            request.Status = RequestStatus.Reviewing;
            request.CreatedBy = UserId;
            await _requestRepository.AddAsync(request);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Failed to create request.",
                    request = null
                };
            }
            return new RequestResponse
            {
                Success = true,
                Message = "Request created successfully.",
                request = request
            };
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

        public async Task<ListRequestResponse> GetMyRequestsAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ListRequestResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing.",
                    requests = null
                };
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;
            var UserId = Guid.Parse(userIdClaim);

            var requests = await _requestRepository.GetAllAsync(r => r.UserId == UserId && !r.IsDeleted);
            return new ListRequestResponse
            {
                Success = true,
                Message = "User requests retrieved successfully.",
                requests = requests.ToList()
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

        public async Task<ListRequestResponse> GetRequestsResolvedByMe()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ListRequestResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing.",
                    requests = null
                };
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;
            var UserId = Guid.Parse(userIdClaim);

            var requests = await _requestRepository.GetAllAsync(r => r.UpdatedBy == UserId && !r.IsDeleted);
            return new ListRequestResponse
            {
                Success = true,
                Message = "User requests retrieved successfully.",
                requests = requests.ToList()
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

        public async Task<RequestResponse> ResolveRequestAsync(ResolveRequestDTO updateRequestDTO)
        {
            //throw new NotImplementedException();
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing.",
                };
            }
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;
            var UserId = Guid.Parse(userIdClaim);

            var request = await _requestRepository.GetByIdAsync(updateRequestDTO.RequestId);
            if (request == null || request.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Request not found.",
                    request = null
                };
            }
            if (request.Status != RequestStatus.Reviewing)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Request has already been resolved.",
                    request = null
                };

            }
            var requestType = await _requestTypeRepository.GetByIdAsync(request.RequestTypeId);
            if (requestType == null || requestType.IsDeleted)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Invalid request type.",
                    request = null
                };
            }


            if (updateRequestDTO.Status == RequestStatus.Rejected)
            {
                request.Status = updateRequestDTO.Status;
                request.UpdatedBy = UserId;
                request.Response = updateRequestDTO.Response;
            }
            else if (updateRequestDTO.Status == RequestStatus.Approved)
            {
                RequestTypeEnum type = requestType.Type;

                switch (type)
                {
                    case RequestTypeEnum.UpgradeToInstructor:
                        var userUpgrade = await _userRepository.GetByIdAsync(request.UserId);
                        if (userUpgrade != null && !userUpgrade.IsDeleted)
                        {
                            userUpgrade.role = Role.Instructor;
                            _userRepository.Update(userUpgrade);
                            request.Status = updateRequestDTO.Status;
                            request.UpdatedBy = UserId;
                            request.Response = updateRequestDTO.Response;
                        }
                        else
                        {
                            return new RequestResponse
                            {
                                Success = false,
                                Message = "User not found for upgrade.",
                            };
                        }
                        break;
                    case RequestTypeEnum.PublicCourseRequest:
                        var courseResponse = await _courseClient.GetCourseByIdAsync(new GetCourseByIdRequest
                        {
                            CourseId = request.CourseId.ToString()
                        });
                        if (courseResponse.Exists)
                        {
                            //check course is spam or not
                            var checkSpam = _courseClient.AutoCheckCourseAsync(new GetCourseByIdRequest
                            {
                                CourseId = request.CourseId.ToString()
                            });
                            if (!checkSpam.Success)
                            {
                                return new RequestResponse
                                {
                                    Success = false,
                                    Message = "Course failed spam check, cannot be made public.",
                                };
                            }
                            var updateVisibilityResponse = await _courseClient.ModUpdateStatusAsync(new ModChangeCourseStatusRequest
                            {
                                CourseId = request.CourseId.ToString(),
                                Status = "1", // Assuming 1 represents 'Public' status
                                ModeratorId = UserId.ToString()
                            });
                            _logger.LogInformation($"Course visibility update response: {updateVisibilityResponse.CourseId}");
                            if (updateVisibilityResponse.CourseId != null)
                            {
                                request.Status = updateRequestDTO.Status;
                                request.UpdatedBy = UserId;
                                request.Response = updateRequestDTO.Response;
                            }
                            else return new RequestResponse
                            {
                                Success = false,
                                Message = "Failed to update course visibility.",
                            };
                        }
                        else
                        {
                            return new RequestResponse
                            {
                                Success = false,
                                Message = "Course not found for visibility update.",
                            };
                        }
                        break;
                    case RequestTypeEnum.HideCourseRequest:
                        //check course exists
                        //check enrollment -> email thông báo -> set course to private
                        break;
                    case RequestTypeEnum.ReportCourseRequest:
                        // Handle reported course logic if needed
                        break;
                    case RequestTypeEnum.ReportReviewRequest:
                        // Handle reported review logic if needed
                        break;
                }
            }
            var emailTo = await _userRepository.GetByIdAsync(request.UserId);

            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Failed to resolve request.",
                };
            }
            if (request.CourseId.HasValue)
            {
                _logger.LogInformation($"Sending email with course ID {request.CourseId}");
                await _emailSender.InformRequestResolved(
                    user.FindFirst(ClaimTypes.Email)?.Value,
                    emailTo.email,
                    updateRequestDTO.RequestId,
                    requestType.Type.ToString(),
                    request.Status.ToString(),
                    request.Description,
                    request.Response,
                    request.CourseId
                    );
            }
            else
                await _emailSender.InformRequestResolved(
                    user.FindFirst(ClaimTypes.Email)?.Value,
                    emailTo.email,
                    updateRequestDTO.RequestId,
                    requestType.Type.ToString(),
                    request.Status.ToString(),
                    request.Description,
                    request.Response
                    );
        
            return new RequestResponse
            {
                Success = true,
                Message = "Request resolved successfully.",
                request = request
             };
         }

    public async Task<RequestResponse> UpdateRequestAsync(Guid requestId, UpdateRequestDTO updateRequestDTO)
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
            request.Description = updateRequestDTO.Description;
            _requestRepository.Update(request);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new RequestResponse
                {
                    Success = false,
                    Message = "Failed to update request.",
                    request = null
                };
            }
            return new RequestResponse
            {
                Success = true,
                Message = "Request updated successfully.",
                request = request
            };
        }
    }
}
