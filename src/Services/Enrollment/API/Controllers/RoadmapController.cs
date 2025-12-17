using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Enrollment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RoadmapController : ControllerBase
    {
        private readonly IRoadmapService _roadmapService;

        public RoadmapController(IRoadmapService roadmapService)
        {
            _roadmapService = roadmapService;
        }

        [HttpPost]
        [RequireAction("ROADMAP_CREATE")]
        public async Task<IActionResult> Create(
            [FromBody] CreateRoadmapRequest request)
        {
            try
            {
                var response = await _roadmapService.CreateAsync(request, User);
                if (response.Success)
                    return this.OkResponse(response);
                else
                    return this.BadRequestResponse(response.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpGet("my")]
        [RequireAction("ROADMAP_READ")]
        public async Task<IActionResult> GetMyRoadmaps()
        {
            try
            {
                var result = await _roadmapService.GetMyAsync(User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpPost("{id}/join")]
        [RequireAction("ROADMAP_JOIN")]
        public async Task<IActionResult> Join(Guid id)
        {
            try
            {
                var result = await _roadmapService.JoinAsync(id, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }   

        [HttpGet("{id}")]
        [RequireAction("ROADMAP_READ")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var result = await _roadmapService.GetDetailAsync(id, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpPost("{id}/sync-progress")]
        [RequireAction("ROADMAP_UPDATE")]
        public async Task<IActionResult> SyncProgress(Guid id)
        {
            try
            {
                var result = await _roadmapService.SyncProgressAsync(id, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [RequireAction("ROADMAP_UPDATE")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateRoadmapRequest request)
        {
            try
            {
                var result = await _roadmapService.UpdateAsync(id, request, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpPost("{id}/courses")]
        [RequireAction("ROADMAP_UPDATE")]
        public async Task<IActionResult> AddCourse(
            Guid id,
            [FromBody] AddCourseToRoadmapRequest request)
        {
            try
            {
                var result = await _roadmapService.AddCourseAsync(id, request, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpPut("{id}/courses/reorder")]
        [RequireAction("ROADMAP_UPDATE")]
        public async Task<IActionResult> ReorderCourses(
            Guid id,
            [FromBody] ReorderRoadmapCoursesRequest request)
        {
            try
            {
                var result = await _roadmapService.ReorderCoursesAsync(id, request, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpDelete("{id}/courses/{courseId}")]
        [RequireAction("ROADMAP_UPDATE")]
        public async Task<IActionResult> RemoveCourse(
            Guid id,
            Guid courseId)
        {
            try
            {
                var result = await _roadmapService.RemoveCourseAsync(id, courseId, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [RequireAction("ROADMAP_DELETE")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _roadmapService.DeleteAsync(id, User);
                if (result.Success)
                    return this.OkResponse(result);
                else
                    return this.BadRequestResponse(result.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFoundResponse(ex.Message);
            }
            catch (Exception ex)
            {
                return this.InternalServerErrorResponse(ex.Message);
            }
        }
    }
}
