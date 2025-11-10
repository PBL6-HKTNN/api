using Codemy.Payment.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Payment.Application.DTOs
{
    public class UpdatePaymentRequest
    {
        [Required]
        public required Guid PaymentId { get; set; }
        [Required]
        public required OrderStatus status { get; set; }
    }
}
