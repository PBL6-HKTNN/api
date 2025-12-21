namespace Codemy.Enrollment.Application.DTOs
{
    public class DownloadCertificateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid CertificateId { get; set; }
        public string DownloadUrl { get; set; } 
    }
}