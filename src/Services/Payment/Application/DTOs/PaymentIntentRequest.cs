using System.ComponentModel.DataAnnotations;

namespace Codemy.Payment.Application.DTOs
{
    public class PaymentIntentRequest
    {
        [Required]
        public required Guid PaymentId { get; set; }
        [Required]
        public required decimal Amount { get; set; }    
    }
}
