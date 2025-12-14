namespace Codemy.Enrollment.Application.DTOs
{
    public class GetCertificateResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CertificateDto>? Certificates { get; set; }
    }
}