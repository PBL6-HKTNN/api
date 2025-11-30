

using Codemy.Identity.Application.DTOs.Request;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IRequestService
    {
        Task CreateRequestAsync(CreateRequestDTO createRequestDTO);
        Task<RequestResponse> DeleteRequestAsync(Guid requestId);
        Task GetRequestByIdAsync(Guid requestId);
        Task GetRequestsAsync();
        Task UpdateRequestAsync(Guid requestId, UpdateRequestDTO updateRequestDTO);
    }

    public class RequestResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
