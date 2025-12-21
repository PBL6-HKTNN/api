namespace Codemy.Enrollment.Application.DTOs
{
    public class CheckStatusResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Guid? CertificateId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
