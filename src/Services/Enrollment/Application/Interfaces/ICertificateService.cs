using System.Security.Claims;
using Codemy.Enrollment.Application.DTOs;

namespace Codemy.Enrollment.Application.Interfaces
{
    public interface ICertificateService
    {
        Task<GenerateCertificateResponse> GenerateCertificateAsync(Guid enrollmentId, ClaimsPrincipal user);
        Task<GetCertificateResponse> GetMyCertificatesAsync(ClaimsPrincipal user);
        Task<CheckStatusResponse> CheckCertificateStatusAsync(Guid enrollmentId, ClaimsPrincipal user);
        Task<DownloadCertificateResponse> DownloadCertificateAsync(Guid certificateId, ClaimsPrincipal user);
    }
}