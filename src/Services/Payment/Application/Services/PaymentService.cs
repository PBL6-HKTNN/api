using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.CoursesProto;
using Codemy.EnrollmentsProto;
using Codemy.IdentityProto;
using Codemy.Payment.Application.DTOs;
using Codemy.Payment.Application.Interfaces;
using Codemy.Payment.Domain.Entities;
using Codemy.Payment.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; 
using System.Net.WebSockets;
using Stripe;
using System.Security.Claims;
using IdentityService = Codemy.IdentityProto.IdentityService;
namespace Codemy.Payment.Application.Services
{
    internal class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IRepository<Payments> _paymentRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly CoursesService.CoursesServiceClient _courseClient;
        private readonly EnrollmentService.EnrollmentServiceClient _enrollmentClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PaymentGrpcEnrollmentService _paymentGrpcEnrollmentService;

        public PaymentService(
            ILogger<PaymentService> logger,
            IRepository<Payments> paymentRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<OrderItem> orderItemRepository,
            IUnitOfWork unitOfWork,
            IdentityService.IdentityServiceClient client,
            CoursesService.CoursesServiceClient courseClient,
            EnrollmentService.EnrollmentServiceClient enrollmentClient,
            IHttpContextAccessor httpContextAccessor,
            PaymentGrpcEnrollmentService paymentGrpcEnrollmentService
            )
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
            _cartItemRepository = cartItemRepository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
            _client = client;
            _courseClient = courseClient;
            _enrollmentClient = enrollmentClient;
            _httpContextAccessor = httpContextAccessor;
            _paymentGrpcEnrollmentService = paymentGrpcEnrollmentService;
        }

        public async Task UpdateStatusPaymentAutomatic()
        {
            var payments = await _paymentRepository.GetAllAsync(p => p.orderStatus == OrderStatus.Pending);
            foreach (var payment in payments)
            {
                if ((DateTime.UtcNow - payment.paymentDate).TotalMinutes >= 30)
                {
                    payment.orderStatus = OrderStatus.Failed;
                    payment.UpdatedAt = DateTime.UtcNow;
                    _paymentRepository.Update(payment);
                    _logger.LogInformation("Payment with ID {PaymentId} has been automatically updated to Failed status due to timeout.", payment.Id);
                }
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update payment statuses automatically.");
            }
            else _logger.LogInformation("Update payment statuses automatically completed successfully.");
        }

        public async Task<CartResponse> AddToCartAsync(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var cartItemExists = await _cartItemRepository.GetAllAsync(c => c.userId == UserId && c.courseId == courseId);
            if (cartItemExists != null && cartItemExists.Any())
            {
                _logger.LogError("Course {CourseId} is already in the cart for user {UserId}.", courseId, UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Course is already in the cart."
                };
            }

            CartItem cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                userId = UserId,
                courseId = courseId,
                price = decimal.Parse(courseExists.Price),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = UserId,
                UpdatedBy = UserId
            };
            await _cartItemRepository.AddAsync(cartItem);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to add course {CourseId} to cart for user {UserId}.", courseId, UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Failed to add course to cart."
                };
            }
            _logger.LogInformation("Course {CourseId} added to cart for user {UserId}.", courseId, UserId);
            var cartItems = await _cartItemRepository.GetAllAsync(c => c.userId == UserId);
            return new CartResponse
            {
                Success = true,
                Message = "Course added to cart successfully.",
                CartItems = cartItems.ToList()
            };
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest paymentRequest)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new PaymentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var lastPayment = await _paymentRepository.GetAllAsync(p => p.userId == UserId);
            if (lastPayment != null && lastPayment.Any())
            {
                var latestPayment = lastPayment.OrderByDescending(p => p.paymentDate).First();
                if (latestPayment.orderStatus == OrderStatus.Pending)
                {
                    _logger.LogError("User {UserId} has a pending payment. Cannot create a new payment.", UserId);
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "You have a pending payment. Please complete it before creating a new one."
                    };
                }
            }

            Guid paymentId = Guid.NewGuid();
            List<OrderItemDto> orderItems = new List<OrderItemDto>();
            decimal totalAmount = 0;

            
            foreach (var item in paymentRequest.CourseIds)
            {
                var courseExists = await _courseClient.GetCourseByIdAsync(
                    new GetCourseByIdRequest { CourseId = item.ToString() }
                );
                if (!courseExists.Exists)
                {
                    _logger.LogError("Course with ID {CourseId} does not exist.", item);
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = $"Course with ID {item} does not exist."
                    };
                }
                var cartItem = await _cartItemRepository.GetAllAsync(c => c.courseId == item);
                var cartItemFiltered = cartItem.Where(c => !c.IsDeleted);
                if (cartItemFiltered.Any()) {
                    _cartItemRepository.Delete(cartItemFiltered.FirstOrDefault());
                }
                OrderItem orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        paymentId = paymentId,
                        courseId = item,
                        price = decimal.Parse(courseExists.Price),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = UserId,
                        UpdatedBy = UserId
                    };
                OrderItemDto orderItemDto = new OrderItemDto
                    {
                    courseId = item,
                    instructorId = Guid.Parse(courseExists.InstructorId),
                    price = decimal.Parse(courseExists.Price),
                    courseTitle = courseExists.Title,
                    thumbnailUrl = courseExists.Thumbnail,
                    description = courseExists.Description
                };
                orderItems.Add(orderItemDto);
                totalAmount += orderItem.price;
                await _orderItemRepository.AddAsync(orderItem);
            }

            Payments payment = new Payments
            {
                Id = paymentId,
                userId = UserId,
                totalAmount = totalAmount,
                method = paymentRequest.method,
                orderStatus = OrderStatus.Pending,
                paymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = UserId,
                UpdatedBy = UserId
            };
            await _paymentRepository.AddAsync(payment);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to create payment for user {UserId}.", UserId);
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Failed to create payment."
                };
            }
            _logger.LogInformation("Payment created successfully for user {UserId}.", UserId);
            return new PaymentResponse
            {
                Success = true,
                Message = "Payment created successfully.",
                Payment = new PaymentDto
                {
                    Payment = payment,
                    orderItems = orderItems
                }
            };
        }

        public async Task<CartResponse> GetCartAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var cartItems = await _cartItemRepository.GetAllAsync(c => c.userId == UserId);
            var cartItemsFiltered = cartItems.Where(c => !c.IsDeleted);
            return new CartResponse
            {
                Success = true,
                Message = "Cart retrieved successfully.",
                CartItems = cartItemsFiltered.ToList()
            };
        }

        public async Task<ListPaymentResponse> GetListPaymentAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new ListPaymentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new ListPaymentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var payment = await _paymentRepository.GetAllAsync(p => p.userId == UserId);
            if (payment == null || !payment.Any())
            {
                _logger.LogError("No payments found for user {UserId}.", UserId);
                return new ListPaymentResponse
                {
                    Success = false,
                    Message = "No payments found."
                };
            }
            List<PaymentDto> paymentDtos = new List<PaymentDto>();
            foreach (var item in payment)
            {
                var orderItems = await _orderItemRepository.GetAllAsync(oi => oi.paymentId == item.Id);
                List<OrderItemDto> orderItemDtos = new List<OrderItemDto>();
                foreach (var orderItem in orderItems)
                {
                    Guid courseId = orderItem.courseId;
                    var courseExists = await _courseClient.GetCourseByIdAsync(
                    new GetCourseByIdRequest { CourseId = courseId.ToString() }
                    );
                    if (!courseExists.Exists)
                    {
                        _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                        return new ListPaymentResponse
                        {
                            Success = false,
                            Message = $"Course with ID {courseId} does not exist."
                        };
                    }
                    OrderItemDto orderItemDto = new OrderItemDto
                    {
                        courseId = orderItem.courseId,
                        instructorId = Guid.Parse(courseExists.InstructorId),
                        price = orderItem.price,
                        courseTitle = courseExists.Title,
                        thumbnailUrl = courseExists.Thumbnail,
                        description = courseExists.Description
                    };
                    orderItemDtos.Add(orderItemDto);
                }

                PaymentDto paymentDto = new PaymentDto
                {
                    Payment = item,
                    orderItems = orderItemDtos
                };
                paymentDtos.Add(paymentDto);
            }
            return new ListPaymentResponse
           {
                Success = true,
                Message = "Payment retrieved successfully.",
                Payments = paymentDtos
            };
        }

        public async Task<PaymentResponse> GetPaymentAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new PaymentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var payment = await _paymentRepository.GetAllAsync(p => p.userId == UserId);
            if (payment == null || !payment.Any())
            {
                _logger.LogError("No payments found for user {UserId}.", UserId);
                return new PaymentResponse
                {
                    Success = false,
                    Message = "No payments found."
                };
            }
            var latestPayment = payment.OrderByDescending(p => p.paymentDate).First();

            var orderItems = await _orderItemRepository.GetAllAsync(oi => oi.paymentId == latestPayment.Id);
            List<OrderItemDto> orderItemDtos = new List<OrderItemDto>();
            foreach (var orderItem in orderItems)
            {
                Guid courseId = orderItem.courseId;
                var courseExists = await _courseClient.GetCourseByIdAsync(
                     new GetCourseByIdRequest { CourseId = courseId.ToString() }
                     );
                if (!courseExists.Exists)
                {
                    _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = $"Course with ID {courseId} does not exist."
                    };
                }
                OrderItemDto orderItemDto = new OrderItemDto
                {
                    courseId = orderItem.courseId,
                    instructorId = Guid.Parse(courseExists.InstructorId),
                    price = orderItem.price,
                    courseTitle = courseExists.Title,
                    thumbnailUrl = courseExists.Thumbnail,
                    description = courseExists.Description
                };
                orderItemDtos.Add(orderItemDto);
            }

            return new PaymentResponse
            {
                Success = true,
                Message = "Payment retrieved successfully.",
                Payment = new PaymentDto
                {
                    Payment = latestPayment,
                    orderItems = orderItemDtos
                }
            };
        }

        public async Task<CartResponse> RemoveFromCart(Guid courseId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CartResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var courseExists = await _courseClient.GetCourseByIdAsync(
                new GetCourseByIdRequest { CourseId = courseId.ToString() }
            );
            if (!courseExists.Exists)
            {
                _logger.LogError("Course with ID {CourseId} does not exist.", courseId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Course does not exist."
                };
            }

            var cartItemexists = await _cartItemRepository.GetAllAsync(c => c.userId == UserId && c.courseId == courseId);
            if (cartItemexists == null || !cartItemexists.Any())
            {
                _logger.LogError("Cart item for course {CourseId} not found for user {UserId}.", courseId, UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Cart item not found."
                };
            }

            var cartItem = cartItemexists.First();
            _cartItemRepository.Delete(cartItem);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to remove course {CourseId} from cart for user {UserId}.", courseId, UserId);
                return new CartResponse
                {
                    Success = false,
                    Message = "Failed to remove course from cart."
                };
            }
            _logger.LogInformation("Course {CourseId} removed from cart for user {UserId}.", courseId, UserId);
            var cartItems = await _cartItemRepository.GetAllAsync(c => c.userId == UserId);
            return new CartResponse
            {
                Success = true,
                Message = "Course removed from cart successfully.",
                CartItems = cartItems.ToList()
            };
        }

        public async Task<UpdatePaymentResponse> UpdatePaymentStatusAsync(UpdatePaymentRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new UpdatePaymentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new UpdatePaymentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogError("Payment with ID {PaymentId} not found.", request.PaymentId);
                return new UpdatePaymentResponse
                {
                    Success = false,
                    Message = "Payment not found."
                };
            }
            if (payment.orderStatus == OrderStatus.Cancelled || payment.orderStatus == OrderStatus.Completed)
            {
                _logger.LogError("Payment with ID {PaymentId} is not in a pending state or a failed state.", request.PaymentId);
                return new UpdatePaymentResponse
                {
                    Success = false,
                    Message = "Only pending payments and failed payments can be updated."
                };
            }
            if (payment.orderStatus == OrderStatus.Pending) {
                if (request.status == OrderStatus.Pending)
                {
                    _logger.LogError("Payment with ID {PaymentId} is already in Pending state.", request.PaymentId);
                    return new UpdatePaymentResponse
                    {
                        Success = false,
                        Message = "Payment is already in Pending state."
                    };
                }
                payment.orderStatus = request.status;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = UserId;
                if (request.status == OrderStatus.Completed)
                {
                    var orderItems = await _orderItemRepository.GetAllAsync(oi => oi.paymentId == payment.Id);
                    foreach (var item in orderItems)
                    {
                        var enrollmentResponse = await _paymentGrpcEnrollmentService.CreateEnrollmentAsync(item.courseId);
                        if (!enrollmentResponse.Success)
                        {
                            _logger.LogError("Failed to create enrollment for course {CourseId} after payment completion.", item.courseId);
                            return new UpdatePaymentResponse
                            {
                                Success = false,
                                Message = $"Failed to create enrollment for course {item.courseId}."
                            };
                        }
                    }
                }
                _paymentRepository.Update(payment);
            }
            else if (payment.orderStatus == OrderStatus.Failed) { 
                if (request.status != OrderStatus.Pending)
                {
                    _logger.LogError("Payment with ID {PaymentId} can only be updated to Pending from Failed state.", request.PaymentId);
                    return new UpdatePaymentResponse
                    {
                        Success = false,
                        Message = "Only pending status can be set from Failed state."
                    };
                }
                payment.orderStatus = request.status; 
                payment.UpdatedAt = DateTime.UtcNow;
                payment.UpdatedBy = UserId;
                payment.paymentDate = DateTime.UtcNow;
                _paymentRepository.Update(payment);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to update payment status for payment ID {PaymentId}.", request.PaymentId);
                return new UpdatePaymentResponse
                {
                    Success = false,
                    Message = "Failed to update payment status."
                };
            }
            _logger.LogInformation("Payment status updated successfully for payment ID {PaymentId}.", request.PaymentId);
            return new UpdatePaymentResponse
            {
                Success = true,
                Message = "Payment status updated successfully.",
                Payment = payment
            };
        }

        public async Task<CreatePaymentIntentResponse> CreatePaymentIntentAsync(PaymentIntentRequest request)
        {

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CreatePaymentIntentResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = UserId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", UserId);
                return new CreatePaymentIntentResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogError("Payment with ID {PaymentId} not found.", request.PaymentId);
                return new CreatePaymentIntentResponse
                {
                    Success = false,
                    Message = "Payment not found."
                };
            }

            if (payment.totalAmount != request.Amount)
            {
                return new CreatePaymentIntentResponse
                {
                    Success = false,
                    Message = "The amount is not matching."
                };
            }
            LogExtensions.LoadEnvFile(); 
            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("SECRET_KEY");

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string>
                    {
                        { "paymentId", request.PaymentId.ToString() }, // your internal ID
                        { "userId", UserId.ToString() }               // optional
                    }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            if (paymentIntent == null)
            {
                _logger.LogError("Failed to create payment intent.");
                return new CreatePaymentIntentResponse
                {
                    Success = false,
                    Message = "Failed to create payment intent."
                };
            }
            _logger.LogInformation("Payment intent created successfully with ID {PaymentIntentId}.", paymentIntent.Id);
            return new CreatePaymentIntentResponse
            {
                Success = true,
                Message = "Created payment intent.",
                paymentIntent = new PaymentIntentDto
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    paymentId = payment.Id,
                    amount = payment.totalAmount,
                    currency = options.Currency
                }
            };
        }
    }
}
