

using Codemy.Identity.Application.DTOs.Request;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IRequestService
    {
        Task<RequestResponse> CreateRequestAsync(CreateRequestDTO createRequestDTO);
        Task<RequestResponse> DeleteRequestAsync(Guid requestId);
        Task<ListRequestResponse> GetMyRequestsAsync();
        Task<RequestResponse> GetRequestByIdAsync(Guid requestId);
        Task<ListRequestResponse> GetRequestsAsync();
        Task<ListRequestResponse> GetRequestsResolvedByMe();
        Task<ListRequestTypeResponse> GetRequestTypesAsync();
        Task<RequestResponse> ResolveRequestAsync(Guid requestId, ResolveRequestDTO updateRequestDTO);
        Task<RequestResponse> UpdateRequestAsync(Guid requestId, UpdateRequestDTO updateRequestDTO);
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
