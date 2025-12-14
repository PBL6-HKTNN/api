namespace Codemy.Enrollment.Application.DTOs
{
    public class CertificateDto
    {
        public Guid CertificateId { get; set; }
        public Guid CourseId { get; set; }
        public string CertificateUrl { get; set; } = string.Empty;
        public DateTime CompletionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}