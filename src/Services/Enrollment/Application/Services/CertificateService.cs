using Codemy.BuildingBlocks.Core;
using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Domain.Entities;
using Codemy.Enrollment.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Codemy.FilesProto;
using Codemy.CoursesProto;
using Codemy.IdentityProto;
using Google.Protobuf;
using Grpc.Core;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.RegularExpressions;

namespace Codemy.Enrollment.Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly IRepository<Enrollments> _enrollRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CertificateService> _logger;
        private readonly FileUploader.FileUploaderClient _fileUploaderClient;
        private readonly CoursesService.CoursesServiceClient _coursesClient;
        private readonly IdentityService.IdentityServiceClient _identityClient;

        public CertificateService(
            IRepository<Enrollments> enrollRepo,
            IUnitOfWork uow,
            ILogger<CertificateService> logger,
            FileUploader.FileUploaderClient fileUploaderClient,
            CoursesService.CoursesServiceClient coursesClient,
            IdentityService.IdentityServiceClient identityClient)   
        {
            _enrollRepo = enrollRepo;
            _uow = uow;
            _logger = logger;
            _fileUploaderClient = fileUploaderClient;
            _coursesClient = coursesClient;
            _identityClient = identityClient;
        }

        public async Task<GenerateCertificateResponse> GenerateCertificateAsync(Guid enrollmentId, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var enrollment = await _enrollRepo
                .Query()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.studentId == userId);

            if (enrollment == null)
                throw new InvalidOperationException("Enrollment not found");

            if (enrollment.progressStatus != ProgressStatus.Completed)
                throw new InvalidOperationException("Course has not been completed");

            if (!string.IsNullOrEmpty(enrollment.certificateUrl))
                throw new InvalidOperationException("Certificate already generated");

            var student = await _identityClient.GetUserByIdAsync(
                new GetUserByIdRequest
                {
                    UserId = enrollment.studentId.ToString()
                }
            );

            var course = await _coursesClient.GetCourseByIdAsync(
                new GetCourseByIdRequest
                {
                    CourseId = enrollment.courseId.ToString()
                }
            );

            // Generate real PDF bytes
            var pdfBytes = GenerateCertificatePdfBytes(
                studentName: student.Name,
                courseName: course.Title,
                completionDate: enrollment.completionDate ?? DateTime.UtcNow);

            var fileName = $"certificate_{enrollment.Id}.pdf";
            var contentType = "document";

            using var call = _fileUploaderClient.UploadFile();

            // Metadata chunk
            await call.RequestStream.WriteAsync(new UploadFileRequest
            {
                FileName = fileName,
                Type = contentType,
                ChunkData = ByteString.Empty
            });

            // Send chunks
            const int chunkSize = 64 * 1024;
            for (int offset = 0; offset < pdfBytes.Length;)
            {
                int len = Math.Min(chunkSize, pdfBytes.Length - offset);

                await call.RequestStream.WriteAsync(new UploadFileRequest
                {
                    ChunkData = ByteString.CopyFrom(pdfBytes, offset, len)
                });

                offset += len;
            }

            await call.RequestStream.CompleteAsync();
            var response = await call.ResponseAsync;

            if (response == null || string.IsNullOrEmpty(response.FileUrl))
                throw new InvalidOperationException("Certificate upload failed");

            enrollment.certificateUrl = response.FileUrl;
            enrollment.certificateExpiryDate = DateTime.UtcNow.AddYears(1);
            enrollment.certificatePublicId = response.PublicId;

            _enrollRepo.Update(enrollment);
            await _uow.SaveChangesAsync();

            return new GenerateCertificateResponse
            {
                Success = true,
                Message = "Certificate generated successfully",
                CertificateId = enrollment.Id,
                CertificateUrl = enrollment.certificateUrl,
                PublicId = enrollment.certificatePublicId!,
                ExpiryDate = enrollment.certificateExpiryDate.Value
            };
        }

        private byte[] GenerateCertificatePdfBytes(
            string studentName,
            string courseName,
            DateTime completionDate)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Border(5).BorderColor(Colors.Grey.Lighten3)
                        .Padding(5)
                        .Border(2).BorderColor(Colors.Blue.Darken2)
                        .Padding(30)
                        .ExtendVertical()
                        .AlignMiddle()
                        .Column(col =>
                        {
                            // ================= HEADER =================
                            col.Item().AlignCenter().Column(header =>
                            {
                                header.Item()
                                    .AlignCenter()
                                    .Text("CODEMY ACADEMY")
                                    .FontSize(12)
                                    .SemiBold()
                                    .FontColor(Colors.Grey.Medium)
                                    .LetterSpacing(0.1f);

                                header.Item().PaddingTop(10)
                                    .AlignCenter()
                                    .Text("CERTIFICATE")
                                    .FontSize(48)
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken3)
                                    .FontFamily(Fonts.TimesNewRoman);

                                header.Item()
                                    .AlignCenter()
                                    .Text("OF COMPLETION")
                                    .FontSize(18)
                                    .Medium()
                                    .FontColor(Colors.Blue.Darken3)
                                    .LetterSpacing(0.2f);
                            });

                            // ================= BODY =================
                            col.Item().PaddingTop(25).AlignCenter().Column(body =>
                            {
                                body.Item()
                                    .AlignCenter()
                                    .Text("This certificate is proudly presented to")
                                    .FontSize(14)
                                    .Italic()
                                    .FontColor(Colors.Grey.Darken2);

                                body.Item().PaddingVertical(15)
                                    .AlignCenter()
                                    .Text(studentName.ToUpper())
                                    .FontSize(32)
                                    .Bold()
                                    .FontColor(Colors.Black)
                                    .FontFamily(Fonts.Calibri);

                                body.Item().AlignCenter()
                                    .Width(320)
                                    .LineHorizontal(1)
                                    .LineColor(Colors.Grey.Medium);

                                body.Item().PaddingTop(15)
                                    .AlignCenter()
                                    .Text("For successfully completing the course")
                                    .FontSize(14)
                                    .FontColor(Colors.Grey.Darken2);

                                body.Item().PaddingVertical(10)
                                    .AlignCenter()
                                    .Text(courseName)
                                    .FontSize(24)
                                    .SemiBold()
                                    .FontColor(Colors.Blue.Darken2);
                            });

                            // ================= FOOTER =================
                            col.Item().PaddingTop(35).Row(row =>
                            {
                                // Date
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item()
                                        .AlignCenter()
                                        .Text(completionDate.ToString("MMMM dd, yyyy"))
                                        .FontSize(14);

                                    c.Item().PaddingTop(5)
                                        .AlignCenter()
                                        .Width(150)
                                        .LineHorizontal(1);

                                    c.Item().PaddingTop(5)
                                        .AlignCenter()
                                        .Text("Date")
                                        .FontSize(10)
                                        .FontColor(Colors.Grey.Darken1);
                                });

                                // Medal
                                row.RelativeItem()
                                    .AlignCenter()
                                    .Text("🏅")
                                    .FontSize(40);

                                // Signature
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item()
                                        .AlignCenter()
                                        .Text("Codemy Director")
                                        .FontFamily(Fonts.Calibri)
                                        .Italic()
                                        .FontSize(16);

                                    c.Item().PaddingTop(5)
                                        .AlignCenter()
                                        .Width(150)
                                        .LineHorizontal(1);

                                    c.Item().PaddingTop(5)
                                        .AlignCenter()
                                        .Text("Signature")
                                        .FontSize(10)
                                        .FontColor(Colors.Grey.Darken1);
                                });
                            });

                            // ================= FOOT NOTE =================
                            col.Item().PaddingTop(25)
                                .AlignCenter()
                                .Text($"Certificate ID: {Guid.NewGuid().ToString()[..8].ToUpper()} | Codemy Academy © {DateTime.Now.Year}")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Lighten1);
                        });
                });
            });

            return document.GeneratePdf();
        }


        public async Task<GetCertificateResponse> GetMyCertificatesAsync(ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var certs = await _enrollRepo
                .Query()
                .Where(e => e.studentId == userId && !string.IsNullOrEmpty(e.certificateUrl))
                .Select(e => new CertificateDto
                {
                    CertificateId = e.Id,
                    CourseId = e.courseId,
                    CertificateUrl = e.certificateUrl!,
                    CompletionDate = e.completionDate ?? DateTime.MinValue,
                    ExpiryDate = e.certificateExpiryDate ?? DateTime.MinValue
                })
                .ToListAsync();

            _logger.LogInformation("Found {Count} certificates for user {UserId}", certs.Count, userId);

            return new GetCertificateResponse
            {
                Success = true,
                Message = "Certificates retrieved successfully",
                Certificates = certs
            };
        }

        public async Task<CheckStatusResponse> CheckCertificateStatusAsync(Guid enrollmentId, ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var enrollment = await _enrollRepo
                .Query()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.studentId == userId);

            if (enrollment == null)
                throw new InvalidOperationException("Enrollment not found");

            if (enrollment.progressStatus != ProgressStatus.Completed)
                return new CheckStatusResponse { Success = false, Message = "Course has not been completed", Status = "NotCompleted" };

            if (string.IsNullOrEmpty(enrollment.certificateUrl))
                return new CheckStatusResponse { Success = false, Message = "Certificate not generated", Status = "NotGenerated" };

            if (enrollment.certificateExpiryDate.HasValue && enrollment.certificateExpiryDate.Value < DateTime.UtcNow)
                return new CheckStatusResponse { Success = false, Message = "Certificate expired", Status = "Expired" };

            return new CheckStatusResponse
            {
                Success = true,
                Message = "Certificate is available",
                Status = "Available",
                CertificateId = enrollment.Id,
                CertificateUrl = enrollment.certificateUrl,
                ExpiryDate = enrollment.certificateExpiryDate
            };
        }

        public async Task<DownloadCertificateResponse> DownloadCertificateAsync(
            Guid enrollmentId,
            ClaimsPrincipal user)
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

            var enrollment = await _enrollRepo
                .Query()
                .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.studentId == userId);

            if (enrollment == null)
                throw new InvalidOperationException("Certificate not found");

            if (string.IsNullOrEmpty(enrollment.certificatePublicId))
                throw new InvalidOperationException("Certificate not generated");

            if (!enrollment.certificateExpiryDate.HasValue)
                throw new InvalidOperationException("Certificate has no expiry date");

            if (enrollment.certificateExpiryDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Certificate expired");

            string version = "";
            var match = Regex.Match(enrollment.certificateUrl, @"\/v(\d+)\/");
            if (match.Success)
            {
                version = match.Groups[1].Value; 
            }

            var grpcResponse = await _fileUploaderClient.DownloadFileAsync(
                new DownloadFileRequest
                {
                    PublicId = enrollment.certificatePublicId,
                    Version = version
                });

            return new DownloadCertificateResponse
            {
                Success = true,
                Message = "Certificate ready for download",
                CertificateId = enrollment.Id,
                DownloadUrl = grpcResponse.DownloadUrl
            };
        }
    }
}
