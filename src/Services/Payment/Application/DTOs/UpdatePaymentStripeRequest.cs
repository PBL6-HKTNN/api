using Codemy.Payment.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Payment.Application.DTOs
{
    public class UpdatePaymentStripeRequest
    {
        [Required]
        public required Guid PaymentId { get; set; }
        [Required]
        public required Guid UserId { get; set; }
        [Required]
        public required OrderStatus status { get; set; }
    }
}
