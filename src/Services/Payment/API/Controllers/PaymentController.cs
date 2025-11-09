using Codemy.Payment.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Codemy.Payment.Application.DTOs;


namespace Codemy.Payment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet("getCart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var result = await _paymentService.GetCartAsync();
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to retrieve cart.");
                }
                return this.OkResponse(result.CartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("addToCart/{courseId}")]
        public async Task<IActionResult> AddToCart(Guid courseId)
        {
            try
            {
                var result = await _paymentService.AddToCartAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to add course to cart.");
                }
                return
                    this.CreatedResponse(
                        result.CartItems,
                        $"/cart/get"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding course to cart.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }
        [HttpDelete("removeFromCart/{courseId}")]
        public async Task<IActionResult> RemoveFromCart(Guid courseId)
        {
            try
            {
                var result = await _paymentService.RemoveFromCart(courseId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to remove course from cart.");
                }
                return this.OkResponse("Course removed from cart successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing course from cart.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("createPayment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest paymentRequest)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _paymentService.CreatePaymentAsync(paymentRequest);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to create payment.");
                }
                return
                    this.CreatedResponse(
                        result.Payment,
                        $"/payment/{result.Payment?.Payment.Id}"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }
        [HttpGet("payment")]
        public async Task<IActionResult> GetPayment()
        {
            try
            { 
                var result = await _paymentService.GetPaymentAsync();
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to retrieve payment.");
                }
                return this.OkResponse(result.Payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpGet("list-payments")]
        public async Task<IActionResult> GetListPayment()
        {
            try
            {
                var result = await _paymentService.GetListPaymentAsync();
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to retrieve payment.");
                }
                return this.OkResponse(result.Payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }

        [HttpPost("update-payment")]
        public async Task<IActionResult> UpdatePaymentStatus(UpdatePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            { 
                var result = await _paymentService.UpdatePaymentStatusAsync(request);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to update payment status.");
                }
                return this.OkResponse(result.Payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status.");
                return this.InternalServerErrorResponse("Internal server error.");
            }
        }
    } 
}
