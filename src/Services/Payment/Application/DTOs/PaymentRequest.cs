using Codemy.Payment.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Payment.Application.DTOs
{
    public class PaymentRequest
    {
        [Required]
        public required MethodPayment method { get; set; }
        [Required]
        public required List<Guid> CourseIds { get; set; }
    }
}
