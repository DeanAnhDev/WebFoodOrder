using AutoMapper;
using FoodOrder.Application.DTOs.Ahamove;
using FoodOrder.Application.DTOs.Orders;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Orders
{
    internal class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAhamoveService _ahamoveService;
        private readonly IVNPayService _vnPayService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IAhamoveService ahamoveService, IVNPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ahamoveService = ahamoveService;
            _vnPayService = vnPayService;
        }

        #region crud 
        public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto, int userId)
        {
            try
            {
                // 0. Validate input parameters
                if (createOrderDto == null)
                    throw new ArgumentNullException(nameof(createOrderDto));

                if (userId <= 0)
                    throw new ArgumentException("UserId không hợp lệ", nameof(userId));

                // 1. Validate Cart và lấy thông tin cart
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    throw new InvalidOperationException("Giỏ hàng trống hoặc không tồn tại");
                }

                // Kiểm tra cart có đúng thuộc về user không
                if (cart.UserId != userId)
                {
                    throw new UnauthorizedAccessException("Giỏ hàng không thuộc về người dùng này");
                }

                // 2. Validate Location (nếu có - null nghĩa là bán tại quầy)
                string deliveryAddress = "Bán tại quầy"; // Default cho bán tại quầy

                if (createOrderDto.LocationId.HasValue)
                {
                    var location = await _unitOfWork.Locations.GetByIdAsync(createOrderDto.LocationId.Value);
                    if (location == null)
                    {
                        throw new InvalidOperationException("Địa chỉ giao hàng không tồn tại");
                    }

                    if (location.UserId != userId)
                    {
                        throw new UnauthorizedAccessException("Địa chỉ giao hàng không thuộc về người dùng này");
                    }

                    deliveryAddress = location.Address;
                }

                // 3. Validate và xử lý Voucher (nếu có)
                Voucher? voucher = null;
                if (createOrderDto.VoucherId.HasValue)
                {
                    voucher = await _unitOfWork.Vouchers.GetByIdAsync(createOrderDto.VoucherId.Value);
                    if (voucher == null)
                    {
                        throw new InvalidOperationException("Voucher không tồn tại");
                    }

                    if (!voucher.IsActive)
                    {
                        throw new InvalidOperationException("Voucher đã bị vô hiệu hóa");
                    }

                    if (voucher.Quantity <= 0)
                    {
                        throw new InvalidOperationException("Voucher đã hết số lượng");
                    }

                    var currentTime = DateTime.UtcNow;
                    if (currentTime < voucher.StartDate)
                    {
                        throw new InvalidOperationException("Voucher chưa có hiệu lực");
                    }

                    if (currentTime > voucher.EndDate)
                    {
                        throw new InvalidOperationException("Voucher đã hết hạn");
                    }
                }

                // 4. Tính toán giá tiền và validate tồn kho
                decimal subtotalAmount = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Quantity <= 0)
                    {
                        throw new InvalidOperationException("Số lượng sản phẩm trong giỏ hàng không hợp lệ");
                    }

                    // Xử lý Food
                    if (cartItem.FoodId.HasValue)
                    {
                        var food = await _unitOfWork.Foods.GetByIdAsync(cartItem.FoodId.Value);
                        if (food == null)
                        {
                            throw new InvalidOperationException($"Món ăn với ID {cartItem.FoodId} không tồn tại");
                        }

                        if (!food.Status)
                        {
                            throw new InvalidOperationException($"Món {food.FoodName} hiện không khả dụng");
                        }

                        if (food.Quantity < cartItem.Quantity)
                        {
                            throw new InvalidOperationException($"Món {food.FoodName} chỉ còn {food.Quantity} phần, không đủ cho {cartItem.Quantity} phần yêu cầu");
                        }

                        // Tính giá sau khuyến mãi
                        var originalPrice = food.Price;
                        var discountedPrice = originalPrice;

                        if (food.Promotion != null && food.Promotion.IsActive)
                        {
                            var now = DateTime.Now;
                            if (now >= food.Promotion.StartDate && now <= food.Promotion.EndDate)
                            {
                                if (food.Promotion.Type == Domain.Entities.Foods.PromotionType.Amount)
                                {
                                    discountedPrice = Math.Max(0, originalPrice - food.Promotion.DiscountAmount);
                                }
                                else if (food.Promotion.Type == Domain.Entities.Foods.PromotionType.Percentage)
                                {
                                    discountedPrice = originalPrice * (1 - food.Promotion.DiscountAmount / 100);
                                }
                            }
                        }

                        var totalPrice = discountedPrice * cartItem.Quantity;
                        subtotalAmount += totalPrice;

                        // Tạo OrderDetail
                        orderDetails.Add(new OrderDetail
                        {
                            FoodId = food.FoodId,
                            ItemName = food.FoodName ?? "",
                            OriginalPrice = originalPrice,
                            DiscountedPrice = discountedPrice,
                            Quantity = cartItem.Quantity,
                            TotalPrice = totalPrice
                        });

                        // Trừ số lượng Food
                        food.Quantity -= cartItem.Quantity;
                        food.Sold += cartItem.Quantity;
                        await _unitOfWork.Foods.UpdateAsync(food);
                    }
                    // Xử lý Combo
                    else if (cartItem.ComboId.HasValue)
                    {
                        var combo = await _unitOfWork.Combos.GetByIdAsync(cartItem.ComboId.Value);
                        if (combo == null)
                        {
                            throw new InvalidOperationException($"Combo với ID {cartItem.ComboId} không tồn tại");
                        }

                        if (!combo.Status)
                        {
                            throw new InvalidOperationException($"Combo {combo.ComboName} hiện không khả dụng");
                        }

                        if (combo.Quantity < cartItem.Quantity)
                        {
                            throw new InvalidOperationException($"Combo {combo.ComboName} chỉ còn {combo.Quantity} phần, không đủ cho {cartItem.Quantity} phần yêu cầu");
                        }

                        // Tính giá sau khuyến mãi
                        var originalPrice = combo.Price;
                        var discountedPrice = originalPrice;

                        if (combo.Promotion != null && combo.Promotion.IsActive)
                        {
                            var now = DateTime.Now;
                            if (now >= combo.Promotion.StartDate && now <= combo.Promotion.EndDate)
                            {
                                if (combo.Promotion.Type == Domain.Entities.Foods.PromotionType.Amount)
                                {
                                    discountedPrice = Math.Max(0, originalPrice - combo.Promotion.DiscountAmount);
                                }
                                else if (combo.Promotion.Type == Domain.Entities.Foods.PromotionType.Percentage)
                                {
                                    discountedPrice = originalPrice * (1 - combo.Promotion.DiscountAmount / 100);
                                }
                            }
                        }

                        var totalPrice = discountedPrice * cartItem.Quantity;
                        subtotalAmount += totalPrice;

                        // Tạo OrderDetail
                        orderDetails.Add(new OrderDetail
                        {
                            ComboId = combo.ComboId,
                            ItemName = combo.ComboName ?? "",
                            OriginalPrice = originalPrice,
                            DiscountedPrice = discountedPrice,
                            Quantity = cartItem.Quantity,
                            TotalPrice = totalPrice
                        });

                        // Trừ số lượng Combo
                        combo.Quantity -= cartItem.Quantity;
                        combo.Sold += cartItem.Quantity;
                        await _unitOfWork.Combos.UpdateAsync(combo);
                    }
                    else
                    {
                        throw new InvalidOperationException("CartItem phải có FoodId hoặc ComboId");
                    }
                }

                if (orderDetails.Count == 0)
                {
                    throw new InvalidOperationException("Không có sản phẩm hợp lệ trong giỏ hàng");
                }

                // 5. Tính toán voucher discount
                decimal voucherDiscountAmount = 0;
                if (voucher != null)
                {
                    // Kiểm tra điều kiện tối thiểu
                    if (subtotalAmount < voucher.MinOrderPrice)
                    {
                        throw new InvalidOperationException($"Đơn hàng tối thiểu {voucher.MinOrderPrice:C} để sử dụng voucher này");
                    }

                    if (voucher.Type == VoucherType.Amount)
                    {
                        voucherDiscountAmount = Math.Min(voucher.DiscountAmount, voucher.MaxDiscountPrice);
                    }
                    else if (voucher.Type == VoucherType.Percentage)
                    {
                        voucherDiscountAmount = Math.Min(
                            subtotalAmount * voucher.DiscountAmount / 100,
                            voucher.MaxDiscountPrice
                        );
                    }

                    // Không cho phép voucher discount lớn hơn subtotal
                    voucherDiscountAmount = Math.Min(voucherDiscountAmount, subtotalAmount);

                    // Trừ số lượng voucher
                    voucher.Quantity -= 1;
                    await _unitOfWork.Vouchers.UpdateAsync(voucher);
                }

                // 6. Tính phí giao hàng (nếu có địa chỉ giao hàng)
                decimal shipFee = 0;
                if (createOrderDto.LocationId.HasValue)
                {
                    try
                    {
                        // Lấy thông tin user để có phone number
                        var user = await _unitOfWork.AppUsers.GetByIdAsync(userId);
                        if (user != null && !string.IsNullOrEmpty(user.PhoneNumber))
                        {
                            var location = await _unitOfWork.Locations.GetByIdAsync(createOrderDto.LocationId.Value);
                            if (location != null)
                            {
                                var shippingRequest = new EstimateShippingFeeRequestDto
                                {
                                    ToAddress = location.Address,
                                    ToName = user.FullName ?? "Khách hàng",
                                    ToPhone = user.PhoneNumber,
                                    ItemValue = 0,
                                    CodAmount = 0, // COD = tổng tiền sau discount
                                    Remarks = $"Đơn hàng #{DateTime.Now.Ticks.ToString()[^6..]}"
                                };

                                var shippingResponse = await _ahamoveService.EstimateShippingFeeAsync(shippingRequest);
                                if (shippingResponse.Success)
                                {
                                    shipFee = shippingResponse.Fee;
                                }
                                else
                                {
                                    // Log lỗi nhưng không fail order, để shipFee = 0
                                    Console.WriteLine($"Không thể tính phí ship: {shippingResponse.Message}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nhưng không fail order, để shipFee = 0
                        Console.WriteLine($"Lỗi khi tính phí ship: {ex.Message}");
                    }
                }

                // 7. Tính tổng tiền cuối cùng (bao gồm phí ship)
                var totalAmount = Math.Max(0, subtotalAmount - voucherDiscountAmount + shipFee);

                var paymentStatus = PaymentStatus.Unpaid;
                var orderStatus = OrderStatus.Pending;

                if (!createOrderDto.LocationId.HasValue && createOrderDto.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    paymentStatus = PaymentStatus.Paid;
                    orderStatus = OrderStatus.Completed;
                }

                // 8. Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Status = orderStatus,
                    PaymentStatus = paymentStatus,
                    PaymentMethod = createOrderDto.PaymentMethod,
                    Note = createOrderDto.Note?.Trim(),
                    Address = deliveryAddress,
                    SubtotalAmount = subtotalAmount,
                    VoucherDiscountAmount = voucherDiscountAmount,
                    TotalAmount = totalAmount,
                    ShipFee = shipFee,
                    VoucherId = createOrderDto.VoucherId,
                    Reason = createOrderDto.Reason?.Trim(),
                    OrderDetails = orderDetails
                };

                // 9. Lưu Order
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.CompleteAsync();

                // 10. Xóa cart items sau khi tạo order thành công
                if (createOrderDto.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    foreach (var cartItem in cart.CartItems.ToList())
                    {
                        await _unitOfWork.CartItems.RemoveAsync(cartItem);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                // 11. Map order result
                var orderDto = _mapper.Map<OrderDto>(order);

                // 12. Tạo payment URL nếu payment method là BankTransfer
                string? paymentUrl = null;
                string message = "Đặt hàng thành công";

                if (createOrderDto.PaymentMethod == Domain.Entities.Orders.PaymentMethod.BankTransfer)
                {
                    try
                    {
                        var orderInfo = $"Thanh toán đơn hàng #{order.OrderCode} - {orderDetails.Count} món";
                        paymentUrl = _vnPayService.CreatePaymentUrl(
                            amount: order.TotalAmount,
                            orderId: order.OrderCode,
                            orderInfo: orderInfo
                        );
                        message = "Đặt hàng thành công. Vui lòng thanh toán để hoàn tất đơn hàng.";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi tạo payment URL: {ex.Message}");
                        // Không fail order, chỉ log lỗi
                        message = "Đặt hàng thành công nhưng không thể tạo link thanh toán. Vui lòng liên hệ hỗ trợ.";
                    }
                }

                return new CreateOrderResponseDto
                {
                    Order = orderDto,
                    PaymentUrl = paymentUrl,
                    Message = message,
                    Success = true
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Có lỗi xảy ra khi tạo đơn hàng", ex);
            }
        }

        public async Task<bool> ProcessPaymentCallbackAsync(string orderCode, string responseData)
        {
            try
            {
                // 1. Validate payment với VNPay
                var isValidPayment = _vnPayService.ValidatePayment(responseData);
                if (!isValidPayment)
                {
                    Console.WriteLine($"Invalid VNPay payment for order {orderCode}");
                    return false;
                }

                // 2. Tìm order theo orderCode
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
                if (order == null)
                {
                    Console.WriteLine($"Order not found: {orderCode}");
                    return false;
                }

                // 3. Kiểm tra order đã được thanh toán chưa
                if (order.PaymentStatus == Domain.Entities.Orders.PaymentStatus.Paid)
                {
                    Console.WriteLine($"Order {orderCode} already paid");
                    return true; // Đã thanh toán rồi
                }

                // 4. Cập nhật payment status
                order.PaymentStatus = Domain.Entities.Orders.PaymentStatus.Paid;
                order.Status = Domain.Entities.Orders.OrderStatus.Processing; // Chuyển sang đang xử lý

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();

                Console.WriteLine($"Payment processed successfully for order {orderCode}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment callback for order {orderCode}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ProcessPaymentSuccessAsync(string orderCode)
        {
            try
            {
                // 1. Tìm order theo orderCode
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);

                if (order == null)
                {
                    Console.WriteLine($"Order not found: {orderCode}");
                    return false;
                }

                // 2. Kiểm tra order đã được thanh toán chưa
                if (order.PaymentStatus == Domain.Entities.Orders.PaymentStatus.Paid)
                {
                    Console.WriteLine($"Order {orderCode} already paid");
                    return true;
                }

                // 3. Cập nhật payment status thành công
                order.PaymentStatus = Domain.Entities.Orders.PaymentStatus.Paid;
                order.Status = Domain.Entities.Orders.OrderStatus.Processing;
                if (order.Address == "Bán tại quầy")
                {
                    order.Status = OrderStatus.Completed;
                }

                await _unitOfWork.Orders.UpdateAsync(order);

                // 4. Xóa cart items của user này
                var userId = order.UserId;
                var userCart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);

                if (userCart != null && userCart.CartItems != null)
                {
                    foreach (var cartItem in userCart.CartItems.ToList())
                    {
                        await _unitOfWork.CartItems.RemoveAsync(cartItem);
                    }
                }

                await _unitOfWork.CompleteAsync();
                Console.WriteLine($"Payment success processed for order {orderCode}, cart cleared");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment success for order {orderCode}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ProcessPaymentFailureAsync(string orderCode)
        {
            try
            {
                // 1. Tìm order theo orderCode
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);

                if (order == null)
                {
                    Console.WriteLine($"Order not found: {orderCode}");
                    return false;
                }

                // 2. Kiểm tra trạng thái order
                if (order.PaymentStatus == Domain.Entities.Orders.PaymentStatus.Paid)
                {
                    Console.WriteLine($"Order {orderCode} already paid, cannot rollback");
                    return false;
                }

                // 3. Lấy order details
                var orderDetails = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == order.OrderId);

                // 4. Rollback số lượng Food và Combo
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.FoodId.HasValue)
                    {
                        var food = await _unitOfWork.Foods.GetByIdAsync(orderDetail.FoodId.Value);
                        if (food != null)
                        {
                            // Cộng lại số lượng đã trừ
                            food.Quantity += orderDetail.Quantity;
                            food.Sold -= orderDetail.Quantity;
                            if (food.Sold < 0) food.Sold = 0; // Đảm bảo không âm
                            await _unitOfWork.Foods.UpdateAsync(food);
                        }
                    }
                    else if (orderDetail.ComboId.HasValue)
                    {
                        var combo = await _unitOfWork.Combos.GetByIdAsync(orderDetail.ComboId.Value);
                        if (combo != null)
                        {
                            // Cộng lại số lượng đã trừ
                            combo.Quantity += orderDetail.Quantity;
                            combo.Sold -= orderDetail.Quantity;
                            if (combo.Sold < 0) combo.Sold = 0; // Đảm bảo không âm
                            await _unitOfWork.Combos.UpdateAsync(combo);
                        }
                    }
                }

                // 5. Rollback voucher quantity nếu có
                if (order.VoucherId.HasValue)
                {
                    var voucher = await _unitOfWork.Vouchers.GetByIdAsync(order.VoucherId.Value);
                    if (voucher != null)
                    {
                        voucher.Quantity += 1; // Cộng lại 1 voucher đã trừ
                        await _unitOfWork.Vouchers.UpdateAsync(voucher);
                    }
                }

                // 6. Cập nhật order status thành failed
                order.PaymentStatus = Domain.Entities.Orders.PaymentStatus.Fail;
                order.Status = Domain.Entities.Orders.OrderStatus.Cancelled;

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();

                Console.WriteLine($"Payment failure processed for order {orderCode}, quantities restored");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing payment failure for order {orderCode}: {ex.Message}");
                return false;
            }
        }

        #endregion




        public async Task<GetOrdersResponseDto> GetAllOrdersAsync(GetOrdersRequestDto request)
        {
            try
            {
                // Use the new OrderRepository method
                var (orders, totalCount) = await _unitOfWork.Orders.GetOrdersWithPaginationAsync(
                    orderCode: request.OrderCode,
                    userId: request.UserId,
                    status: request.Status,
                    paymentStatus: request.PaymentStatus,
                    page: request.Page,
                    pageSize: request.PageSize,
                    sortBy: request.SortBy ?? "CreatedAt",
                    sortOrder: request.SortOrder ?? "desc"
                );

                // Get statistics with the same filters (excluding status filter for complete statistics)
                var statistics = await GetOrderStatisticsAsync(
                    request.OrderCode,
                    request.UserId,
                    request.PaymentStatus
                );

                // Map to DTOs
                var orderDtos = _mapper.Map<List<OrderDto>>(orders);

                return new GetOrdersResponseDto
                {
                    Orders = orderDtos,
                    TotalCount = totalCount,
                    CurrentPage = request.Page,
                    PageSize = request.PageSize,
                    Statistics = statistics
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting orders: {ex.Message}");
                throw;
            }
        }

        public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(
            string? orderCode = null,
            int? userId = null,
            PaymentStatus? paymentStatus = null)
        {
            try
            {
                var statusCounts = await _unitOfWork.Orders.GetOrderCountByStatusAsync(
                    orderCode, userId, paymentStatus);

                var statusCountDtos = statusCounts.Select(kvp => new OrderStatusCountDto
                {
                    Status = kvp.Key,
                    Count = kvp.Value,
                    StatusName = GetStatusDisplayName(kvp.Key)
                }).ToList();

                var totalOrders = statusCounts.Values.Sum();

                return new OrderStatisticsDto
                {
                    StatusCounts = statusCountDtos,
                    TotalOrders = totalOrders
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order statistics: {ex.Message}");
                throw;
            }
        }

        private static string GetStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Chờ xử lý",
                OrderStatus.Accepted => "Đã xác nhận",
                OrderStatus.Processing => "Đang xử lý",
                OrderStatus.Done => "Đã làm xong",
                OrderStatus.Shipping => "Đang giao hàng",
                OrderStatus.Completed => "Hoàn thành",
                OrderStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }

        public async Task<UpdateOrderStatusResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto request)
        {
            try
            {
                // Validate input
                if (request.OrderId <= 0)
                {
                    return new UpdateOrderStatusResponseDto
                    {
                        Success = false,
                        Message = "OrderId không hợp lệ"
                    };
                }

                // Find the order
                var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId);
                if (order == null)
                {
                    return new UpdateOrderStatusResponseDto
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                // Validate status transition
                if (!IsValidStatusTransition(order.Status, request.NewStatus))
                {
                    return new UpdateOrderStatusResponseDto
                    {
                        Success = false,
                        Message = $"Không thể chuyển từ trạng thái '{GetStatusDisplayName(order.Status)}' sang '{GetStatusDisplayName(request.NewStatus)}'"
                    };
                }

                // Update order status and reason
                var oldStatus = order.Status;
                order.Status = request.NewStatus;

                if (request.NewStatus == OrderStatus.Cancelled)
                {

                    order.PaymentStatus = PaymentStatus.Fail;

                        // Rollback số lượng food và combo khi đơn hàng bị hủy
                        await RollbackInventoryForCancelledOrderAsync(order.OrderId);

                        // Rollback voucher nếu có sử dụng
                        await RollbackVoucherForCancelledOrderAsync(order);
                }

                // Update reason if provided or if status is being cancelled
                if (!string.IsNullOrWhiteSpace(request.Reason) || request.NewStatus == OrderStatus.Cancelled)
                {
                    order.Reason = request.Reason;
                }

               

                if (request.NewStatus == OrderStatus.Accepted && order.PaymentMethod == PaymentMethod.CashOnDelivery)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                }

                // Save changes
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();

                // Map to DTO for response
                var orderDto = _mapper.Map<OrderDto>(order);

                return new UpdateOrderStatusResponseDto
                {
                    Success = true,
                    Message = $"Đã cập nhật trạng thái đơn hàng từ '{GetStatusDisplayName(oldStatus)}' sang '{GetStatusDisplayName(request.NewStatus)}'",
                    Order = orderDto
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return new UpdateOrderStatusResponseDto
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng"
                };
            }
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid status transitions
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                [OrderStatus.Pending] = new() { OrderStatus.Accepted, OrderStatus.Cancelled },
                [OrderStatus.Accepted] = new() { OrderStatus.Processing, OrderStatus.Cancelled },
                [OrderStatus.Processing] = new() { OrderStatus.Done, OrderStatus.Cancelled },
                [OrderStatus.Done] = new() { OrderStatus.Shipping, OrderStatus.Completed, OrderStatus.Cancelled },
                [OrderStatus.Shipping] = new() { OrderStatus.Completed, OrderStatus.Cancelled },
                [OrderStatus.Completed] = new() { }, // No transitions from completed
                [OrderStatus.Cancelled] = new() { }  // No transitions from cancelled
            };

            // Allow same status (no change)
            if (currentStatus == newStatus)
                return true;

            return validTransitions.ContainsKey(currentStatus) &&
                   validTransitions[currentStatus].Contains(newStatus);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetOrderByIdWithDetailsAsync(orderId);
                if (order == null)
                    return null;

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order by id: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Rollback inventory (food and combo quantities) when order is cancelled
        /// </summary>
        /// <param name="orderId">ID của order bị hủy</param>
        private async Task RollbackInventoryForCancelledOrderAsync(int orderId)
        {
            try
            {
                // Lấy order details của đơn hàng bị hủy
                var orderDetails = await _unitOfWork.OrderDetails.FindAsync(od => od.OrderId == orderId);

                foreach (var orderDetail in orderDetails)
                {
                    // Rollback Food quantity
                    if (orderDetail.FoodId.HasValue)
                    {
                        var food = await _unitOfWork.Foods.GetByIdAsync(orderDetail.FoodId.Value);
                        if (food != null)
                        {
                            // Cộng lại số lượng đã bị trừ khi tạo order
                            food.Quantity += orderDetail.Quantity;

                            // Trừ lại số lượng đã bán (sold)
                            food.Sold -= orderDetail.Quantity;
                            if (food.Sold < 0) food.Sold = 0; // Đảm bảo không âm

                            await _unitOfWork.Foods.UpdateAsync(food);
                            Console.WriteLine($"Rollback Food ID {food.FoodId}: +{orderDetail.Quantity} quantity, -{orderDetail.Quantity} sold");
                        }
                    }
                    // Rollback Combo quantity
                    else if (orderDetail.ComboId.HasValue)
                    {
                        var combo = await _unitOfWork.Combos.GetByIdAsync(orderDetail.ComboId.Value);
                        if (combo != null)
                        {
                            // Cộng lại số lượng đã bị trừ khi tạo order
                            combo.Quantity += orderDetail.Quantity;

                            // Trừ lại số lượng đã bán (sold)
                            combo.Sold -= orderDetail.Quantity;
                            if (combo.Sold < 0) combo.Sold = 0; // Đảm bảo không âm

                            await _unitOfWork.Combos.UpdateAsync(combo);
                            Console.WriteLine($"Rollback Combo ID {combo.ComboId}: +{orderDetail.Quantity} quantity, -{orderDetail.Quantity} sold");
                        }
                    }
                }

                Console.WriteLine($"Inventory rollback completed for cancelled order {orderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling back inventory for order {orderId}: {ex.Message}");
                // Không throw exception để không làm fail việc hủy order
                // Chỉ log lỗi để admin có thể xử lý thủ công nếu cần
            }
        }

        /// <summary>
        /// Rollback voucher quantity when order is cancelled
        /// </summary>
        /// <param name="order">Order bị hủy</param>
        private async Task RollbackVoucherForCancelledOrderAsync(Order order)
        {
            try
            {
                // Nếu order có sử dụng voucher
                if (order.VoucherId.HasValue && order.VoucherDiscountAmount > 0)
                {
                    var voucher = await _unitOfWork.Vouchers.GetByIdAsync(order.VoucherId.Value);
                    if (voucher != null)
                    {
                        // Cộng lại 1 voucher đã bị trừ khi tạo order
                        voucher.Quantity += 1;
                        await _unitOfWork.Vouchers.UpdateAsync(voucher);

                        Console.WriteLine($"Rollback Voucher ID {voucher.VoucherId}: +1 quantity (now {voucher.Quantity})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rolling back voucher for order {order.OrderId}: {ex.Message}");
                // Không throw exception để không làm fail việc hủy order
            }
        }
    }
}
