using System.Security.Claims;
using Codemy.BuildingBlocks.Core;
using Codemy.CoursesProto;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities;
using Codemy.Enrollment.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Codemy.Enrollment.Application.Services
{
    public class RoadmapService : IRoadmapService
    {
        private readonly IRepository<Roadmap> _roadmapRepo;
        private readonly IRepository<RoadmapItem> _roadmapItemRepo;
        private readonly IRepository<UserRoadmap> _userRoadmapRepo;
        private readonly IRepository<Enrollments> _enrollmentRepo;
        private readonly CoursesService.CoursesServiceClient _courseClient;
        private readonly IUnitOfWork _unitOfWork;
        public RoadmapService(
            IRepository<Roadmap> roadmapRepo,
            IRepository<RoadmapItem> roadmapItemRepo,
            IRepository<UserRoadmap> userRoadmapRepo,
            IRepository<Enrollments> enrollmentRepo,
            CoursesService.CoursesServiceClient courseClient,
            IUnitOfWork unitOfWork)
        {
            _roadmapRepo = roadmapRepo;
            _roadmapItemRepo = roadmapItemRepo;
            _userRoadmapRepo = userRoadmapRepo;
            _enrollmentRepo = enrollmentRepo;
            _courseClient = courseClient;
            _unitOfWork = unitOfWork;
        }
        
        public async Task<CreateRoadmapResponse> CreateAsync(CreateRoadmapRequest request, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            // 1. create roadmap
            var roadmap = new Roadmap
            {
                ownerId = userId,
                title = request.Title,
                description = request.Description
            };

            await _roadmapRepo.AddAsync(roadmap);

            // 2. create items
            int order = 1;
            foreach (var courseId in request.CourseIds)
            {
                var courseResponse = await _courseClient.GetCourseByIdAsync(
                    new GetCourseByIdRequest
                    {
                        CourseId = courseId.ToString()
                    }
                );

                if (!courseResponse.Exists)
                {
                    throw new KeyNotFoundException("Course not found: " + courseId);
                }

                await _roadmapItemRepo.AddAsync(new RoadmapItem
                {
                    roadmapId = roadmap.Id,
                    courseId = courseId,
                    order = order++
                });
            }

            // 3. auto join owner
            await _userRoadmapRepo.AddAsync(new UserRoadmap
            {
                userId = userId,
                roadmapId = roadmap.Id,
                enrolledAt = DateTime.UtcNow,
                progress = 0
            });
            await _unitOfWork.SaveChangesAsync();

            return new CreateRoadmapResponse
            {
                RoadmapId = roadmap.Id,
                Success = true,
                Message = "Roadmap created successfully"
            };
        }

        public async Task<JoinRoadmapResponse> JoinAsync(Guid roadmapId, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            // Check roadmap exists
            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null)
                throw new KeyNotFoundException("Roadmap not found");

            // check existed
            var existed = await _userRoadmapRepo
                .FindAsync(x => x.roadmapId == roadmapId && x.userId == userId);

            if (existed.Any())
                throw new InvalidOperationException("User already joined roadmap");

            var userRoadmap = new UserRoadmap
            {
                userId = userId,
                roadmapId = roadmapId,
                enrolledAt = DateTime.UtcNow,
                progress = 0
            };

            await _userRoadmapRepo.AddAsync(userRoadmap);

            await _unitOfWork.SaveChangesAsync();

            return new JoinRoadmapResponse
            {
                UserRoadmap = userRoadmap,
                Message = "Joined roadmap successfully",
                Success = true
            };
        }


        public async Task<GetMyRoadmapsResponse> GetMyAsync(ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            // get roadmaps where user is owner or joined
            // left join roadmaps with user_roadmaps
            var query =
                from r in _roadmapRepo.Query().Where(x => !x.IsDeleted)
                join ur in _userRoadmapRepo.Query().Where(x => !x.IsDeleted)
                    on r.Id equals ur.roadmapId into userRoadmaps
                from ur in userRoadmaps
                    .Where(x => x.userId == userId)
                    .DefaultIfEmpty()
                where ur != null || r.ownerId == userId
                select new RoadmapDto
                {
                    RoadmapId = r.Id,
                    Title = r.title,
                    Description = r.description,
                    Progress = ur != null ? ur.progress : 0
                };

            return new GetMyRoadmapsResponse
            {
                Roadmaps = await query.ToListAsync(),
                Success = true,
                Message = "Fetched my roadmaps successfully"
            };
        }

        public async Task<GetRoadmapDetailResponse> GetDetailAsync(Guid roadmapId, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null)
                throw new KeyNotFoundException("Roadmap not found");

            var items = await _roadmapItemRepo.Query()
                .Where(i => i.roadmapId == roadmapId && !i.IsDeleted)
                .OrderBy(i => i.order)
                .ToListAsync();


            var userRoadmap = await _userRoadmapRepo.Query()
                .FirstOrDefaultAsync(x => x.roadmapId == roadmapId && x.userId == userId);

            var roadmapDetail = new RoadmapDetailDto
            {
                RoadmapId = roadmap.Id,
                Title = roadmap.title,
                Description = roadmap.description,
                IsOwner = roadmap.ownerId == userId,
                IsJoined = userRoadmap != null,
                Progress = userRoadmap?.progress ?? 0,
                CourseIds = items.Select(i => i.courseId).ToList()
            };

            return new GetRoadmapDetailResponse
            {
                Roadmap = roadmapDetail,
                Success = true,
                Message = "Fetched roadmap detail successfully"
            };
        }

        public async Task<SyncRoadmapProgressResponse> SyncProgressAsync(Guid roadmapId, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var userRoadmap = await _userRoadmapRepo.Query()
                .FirstOrDefaultAsync(x => x.roadmapId == roadmapId && x.userId == userId);

            if (userRoadmap == null)
                throw new KeyNotFoundException("User not joined roadmap");

            // get course list
            var courseIds = await _roadmapItemRepo.Query()
                .Where(i => i.roadmapId == roadmapId && !i.IsDeleted)
                .Select(i => i.courseId)
                .ToListAsync();

            if (!courseIds.Any())
                return new SyncRoadmapProgressResponse
                {
                    Progress = 0,
                    IsCompleted = false,
                    Success = true,
                    Message = "No courses in roadmap"
                };

            // count completed enrollments
            var completedCount = await _enrollmentRepo.Query()
                .Where(e =>
                    e.studentId == userId &&
                    e.progressStatus == ProgressStatus.Completed &&
                    courseIds.Contains(e.courseId))
                .CountAsync();

            var progress = (decimal)completedCount / courseIds.Count * 100;

            userRoadmap.progress = Math.Round(progress, 2);

            if (progress == 100 && userRoadmap.completedAt == null)
                userRoadmap.completedAt = DateTime.UtcNow;

            _userRoadmapRepo.Update(userRoadmap);
            await _unitOfWork.SaveChangesAsync();
            return new SyncRoadmapProgressResponse
            {
                Progress = userRoadmap.progress,
                IsCompleted = userRoadmap.progress == 100,
                Success = true,
                Message = "Synchronized roadmap progress successfully"
            };
        }

        public async Task<UpdateRoadmapResponse> UpdateAsync(
            Guid roadmapId,
            UpdateRoadmapRequest request,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null || roadmap.ownerId != userId)
                return new UpdateRoadmapResponse
                {
                    Success = false,
                    Message = "Roadmap not found or access denied"
                };

            roadmap.title = request.Title;
            roadmap.description = request.Description;

            _roadmapRepo.Update(roadmap);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateRoadmapResponse
            {
                Success = true,
                Message = "Roadmap updated successfully",
                RoadmapId = roadmap.Id,
                Title = roadmap.title,
                Description = roadmap.description
            };
        }

        public async Task<AddCourseToRoadmapResponse> AddCourseAsync(
            Guid roadmapId,
            AddCourseToRoadmapRequest request,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null || roadmap.ownerId != userId)
                return new AddCourseToRoadmapResponse
                {
                    Success = false,
                    Message = "Access denied"
                };

            var existed = await _roadmapItemRepo.FindAsync(
                x => x.roadmapId == roadmapId &&
                    x.courseId == request.CourseId &&
                    !x.IsDeleted
            );


            if (existed.Any())
                return new AddCourseToRoadmapResponse
                {
                    Success = false,
                    Message = "Course already exists in roadmap"
                };

            var maxOrder = _roadmapItemRepo.Query()
                .Where(x => x.roadmapId == roadmapId)
                .Select(x => (int?)x.order)
                .Max() ?? 0;

            var item = new RoadmapItem
            {
                roadmapId = roadmapId,
                courseId = request.CourseId,
                order = maxOrder + 1
            };

            await _roadmapItemRepo.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();

            return new AddCourseToRoadmapResponse
            {
                Success = true,
                Message = "Course added to roadmap",
                RoadmapId = roadmapId,
                CourseId = request.CourseId,
                Order = item.order
            };
        }
        public async Task<ReorderRoadmapCoursesResponse> ReorderCoursesAsync(
            Guid roadmapId,
            ReorderRoadmapCoursesRequest request,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null || roadmap.ownerId != userId)
                return new ReorderRoadmapCoursesResponse
                {
                    Success = false,
                    Message = "Access denied"
                };

            var items = await _roadmapItemRepo.FindAsync(
                x => x.roadmapId == roadmapId && !x.IsDeleted
            );

            for (int i = 0; i < request.CourseIds.Count; i++)
            {
                var item = items.FirstOrDefault(x => x.courseId == request.CourseIds[i]);
                if (item != null)
                    item.order = i + 1;
            }

            await _unitOfWork.SaveChangesAsync();

            return new ReorderRoadmapCoursesResponse
            {
                Success = true,
                Message = "Courses reordered successfully",
                RoadmapId = roadmapId
            };
        }

        public async Task<RemoveCourseFromRoadmapResponse> RemoveCourseAsync(
            Guid roadmapId,
            Guid courseId,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null || roadmap.ownerId != userId)
                return new RemoveCourseFromRoadmapResponse
                {
                    Success = false,
                    Message = "Access denied"
                };

            var item = await _roadmapItemRepo.Query()
                .FirstOrDefaultAsync(x =>
                    x.roadmapId == roadmapId &&
                    x.courseId == courseId &&
                    !x.IsDeleted);

            if (item == null)
                return new RemoveCourseFromRoadmapResponse
                {
                    Success = false,
                    Message = "Course not found in roadmap"
                };

            var removedOrder = item.order;

            _roadmapItemRepo.Delete(item);

            var itemsToReorder = await _roadmapItemRepo.Query()
                .Where(x =>
                    x.roadmapId == roadmapId &&
                    x.order > removedOrder &&
                    !x.IsDeleted)
                .ToListAsync();

            foreach (var i in itemsToReorder)
            {
                i.order -= 1;
            }

            await _unitOfWork.SaveChangesAsync();

            return new RemoveCourseFromRoadmapResponse
            {
                Success = true,
                Message = "Course removed and roadmap reordered successfully",
                RoadmapId = roadmapId,
                CourseId = courseId
            };
        }

        public async Task<DeleteRoadmapResponse> DeleteAsync(
            Guid roadmapId,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var roadmap = await _roadmapRepo.GetByIdAsync(roadmapId);
            if (roadmap == null || roadmap.ownerId != userId)
                return new DeleteRoadmapResponse
                {
                    Success = false,
                    Message = "Access denied"
                };

            var items = await _roadmapItemRepo.FindAsync(x => x.roadmapId == roadmapId);
            foreach (var item in items)
                _roadmapItemRepo.Delete(item);

            var users = await _userRoadmapRepo.FindAsync(x => x.roadmapId == roadmapId);
            foreach (var ur in users)
                _userRoadmapRepo.Delete(ur);

            _roadmapRepo.Delete(roadmap);
            await _unitOfWork.SaveChangesAsync();

            return new DeleteRoadmapResponse
            {
                Success = true,
                Message = "Roadmap deleted successfully",
                RoadmapId = roadmapId
            };
        }
    }
}