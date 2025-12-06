

using Codemy.Identity.Application.DTOs.Request;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IRequestService
    {
        Task<RequestResponse> CreateRequestAsync(CreateRequestDTO createRequestDTO);
        Task<RequestResponse> DeleteRequestAsync(Guid requestId);
        Task<AllDetailResponse> GetAllDetailRequestsAsync();
        Task<ListRequestResponse> GetMyRequestsAsync();
        Task<RequestResponse> GetRequestByIdAsync(Guid requestId);
        Task<ListRequestResponse> GetRequestsAsync();
        Task<ListRequestResponse> GetRequestsResolvedByMe();
        Task<ListRequestTypeResponse> GetRequestTypesAsync();
        Task<RequestResponse> ResolveRequestAsync(ResolveRequestDTO updateRequestDTO);
        Task<RequestResponse> UpdateRequestAsync(Guid requestId, UpdateRequestDTO updateRequestDTO);
    }

    public class AllDetailResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public AllDetailDto Data { get; set; }
    }

    public class AllDetailDto
    {
        public int TotalPendingRequest { get; set; }
        public int TotalResolvedRequest { get; set; }
        public List<Request> UpgradeRequest { get; set; }
        public List<Request> PublicCourserequest { get; set; }
        public List<Request> HideCourserequest { get; set; }
        public List<Request> ReportCourserequest { get; set; }
        public List<Request> ReportReviewrequest { get; set; }
    }

    public class ListRequestTypeResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<RequestType>? types { get; set; }
    }

    public class RequestResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Request? request { get; set; }
    }

    public class ListRequestResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Request>? requests { get; set; }
    }
}
