using Codemy.Payment.Application.DTOs;
using Codemy.Payment.Domain.Entities;

namespace Codemy.Payment.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<CartResponse> AddToCartAsync(Guid courseId);
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest paymentRequest);
        Task<CreatePaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request);
        Task<CartResponse> GetCartAsync();
        Task<ListPaymentResponse> GetListPaymentAsync();
        Task<PaymentResponse> GetPaymentAsync();
        Task<CartResponse> RemoveFromCart(Guid courseId);
        Task<UpdatePaymentResponse> UpdatePaymentStatusAsync(UpdatePaymentRequest request);
        Task UpdateStatusPaymentAutomatic();
    }

    public class CreatePaymentIntentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public PaymentIntentDto paymentIntent { get; set; }
    }

    public class PaymentIntentDto
    {
        public string ClientSecret { get; set; }
        public Guid paymentId { get; set; }
        public string currency { get; set; }
        public decimal amount { get; set; }
    }

    public class ListPaymentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<PaymentDto>? Payments { get; set; }
    }

    public class CartResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }

    public class UpdatePaymentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Payments? Payment { get; set; }
    }

    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public PaymentDto? Payment { get; set; }
    }

    public class PaymentDto
    {
        public Payments Payment { get; set; }
        public List<OrderItemDto> orderItems { get; set; }
    }

    public class OrderItemDto
    {
        public Guid courseId { get; set; }
        public Guid instructorId { get; set; }
        public decimal price { get; set; }
        public string courseTitle { get; set; }
        public string thumbnailUrl { get; set; }
        public string description { get; set; }
    }
}
