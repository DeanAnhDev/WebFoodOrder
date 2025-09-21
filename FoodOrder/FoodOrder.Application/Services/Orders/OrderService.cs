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

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IAhamoveService ahamoveService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _ahamoveService = ahamoveService;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, int userId)
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

                // 8. Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = Domain.Entities.Orders.OrderStatus.Pending,
                    PaymentStatus = Domain.Entities.Orders.PaymentStatus.Unpaid,
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
                foreach (var cartItem in cart.CartItems.ToList())
                {
                    await _unitOfWork.CartItems.RemoveAsync(cartItem);
                }
                await _unitOfWork.CompleteAsync();

                // 11. Map và trả về result
                var orderDto = _mapper.Map<OrderDto>(order);
                return orderDto;
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
    }
}
