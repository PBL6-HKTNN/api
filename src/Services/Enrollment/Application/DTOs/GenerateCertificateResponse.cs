namespace Codemy.Enrollment.Application.DTOs
{
    public class GenerateCertificateResponse
    {
        public Guid? CertificateId { get; set; }
        public string CertificateUrl { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; } = null;
    }

}