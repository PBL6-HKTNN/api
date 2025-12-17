using System.Security.Claims;
using Codemy.Enrollment.Application.DTOs;

namespace Codemy.Enrollment.Application.Interfaces
{
    public interface IRoadmapService
    {
        Task<CreateRoadmapResponse> CreateAsync(CreateRoadmapRequest request, ClaimsPrincipal user);
        Task<GetMyRoadmapsResponse> GetMyAsync(ClaimsPrincipal user);
        Task<JoinRoadmapResponse> JoinAsync(Guid roadmapId, ClaimsPrincipal user);
        Task<GetRoadmapDetailResponse> GetDetailAsync(Guid roadmapId, ClaimsPrincipal user);
        Task<SyncRoadmapProgressResponse> SyncProgressAsync(Guid roadmapId, ClaimsPrincipal user);
        Task<UpdateRoadmapResponse> UpdateAsync(Guid roadmapId, UpdateRoadmapRequest request, ClaimsPrincipal user);

        Task<AddCourseToRoadmapResponse> AddCourseAsync(Guid roadmapId, AddCourseToRoadmapRequest request, ClaimsPrincipal user);
        Task<ReorderRoadmapCoursesResponse> ReorderCoursesAsync(Guid roadmapId, ReorderRoadmapCoursesRequest request, ClaimsPrincipal user);
        Task<RemoveCourseFromRoadmapResponse> RemoveCourseAsync(Guid roadmapId, Guid courseId, ClaimsPrincipal user);
        Task<DeleteRoadmapResponse> DeleteAsync(Guid roadmapId, ClaimsPrincipal user);

    }
}