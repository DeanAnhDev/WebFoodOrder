using AutoMapper;
using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Application.DTOs.Foods.Promotion;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Carts
{
    internal class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartResponse> GetOrCreateCartAsync(int userId)
        {
            // Lấy giỏ hàng kèm theo CartItems
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);

            // Nếu chưa có thì tạo mới
            if (cart == null)
            {
                cart = await _unitOfWork.Carts.CreateCartAsync(userId);
                await _unitOfWork.CompleteAsync();

                var emptyDto = _mapper.Map<CartDto>(cart);
                return new CartResponse
                {
                    Cart = emptyDto,
                    Subtotal = 0,
                    OriginalTotal = 0,
                    TotalDiscount = 0,
                    TotalQuantity = 0
                };
            }

            // Danh sách các item cần xoá vì hết hàng
            var itemsToRemove = new List<CartItem>();

            foreach (var item in cart.CartItems?.ToList() ?? new List<CartItem>())
            {
                int? stock = null;

                if (item.FoodId.HasValue)
                {
                    var food = await _unitOfWork.Foods.GetByIdAsync(item.FoodId.Value);
                    stock = food?.Quantity;
                }
                else if (item.ComboId.HasValue)
                {
                    var combo = await _unitOfWork.Combos.GetByIdAsync(item.ComboId.Value);
                    stock = combo?.Quantity;
                }

                // Nếu không tìm thấy hoặc đã hết hàng → xóa khỏi giỏ
                if (!stock.HasValue || stock == 0)
                {
                    itemsToRemove.Add(item);
                }
            }

            // Thực hiện xóa nếu có item hết hàng
            if (itemsToRemove.Any())
            {
                foreach (var item in itemsToRemove)
                {
                    await _unitOfWork.CartItems.RemoveAsync(item);
                }

                await _unitOfWork.CompleteAsync();
            }

            // Map DTO sau khi đã dọn giỏ hàng
            var cartDto = _mapper.Map<CartDto>(cart);

            // Tính tổng tiền và số lượng + cập nhật thông tin giá cho từng item
            decimal subtotal = 0;           // Tổng tiền sau khuyến mãi
            decimal originalTotal = 0;      // Tổng tiền gốc
            int totalQuantity = 0;

            foreach (var item in cartDto.CartItems ?? Enumerable.Empty<CartItemDto>())
            {
                decimal originalPrice = 0;
                decimal finalPrice = 0;

                if (item.Food != null)
                {
                    originalPrice = item.Food.Price;
                    finalPrice = CalculateFinalPrice(originalPrice, item.Food.Promotion);
                }
                else if (item.Combo != null)
                {
                    originalPrice = item.Combo.Price;
                    finalPrice = CalculateFinalPrice(originalPrice, item.Combo.Promotion);
                }

                // Cập nhật thông tin giá cho item
                item.OriginalPrice = originalPrice;
                item.FinalPrice = finalPrice;
                item.OriginalTotal = originalPrice * item.Quantity;
                item.FinalTotal = finalPrice * item.Quantity;
                item.DiscountAmount = item.OriginalTotal - item.FinalTotal;

                subtotal += item.FinalTotal;
                originalTotal += item.OriginalTotal;
                totalQuantity += item.Quantity;
            }

            var totalDiscount = originalTotal - subtotal;

            return new CartResponse
            {
                Cart = cartDto,
                Subtotal = subtotal,
                OriginalTotal = originalTotal,
                TotalDiscount = totalDiscount,
                TotalQuantity = totalQuantity
            };
        }



        public async Task AddToCartAsync(int userId, int? foodId, int? comboId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = await _unitOfWork.Carts.CreateCartAsync(userId);
                await _unitOfWork.CompleteAsync();
            }

            // Lấy sản phẩm hiện có để kiểm tra tồn kho
            int? availableStock = null;
            if (foodId.HasValue)
            {
                var food = await _unitOfWork.Foods.GetByIdAsync(foodId.Value);
                if (food == null)
                    throw new ArgumentException("Món ăn không tồn tại");

                availableStock = food.Quantity;
            }
            else if (comboId.HasValue)
            {
                var combo = await _unitOfWork.Combos.GetByIdAsync(comboId.Value);
                if (combo == null)
                    throw new ArgumentException("Combo không tồn tại");

                availableStock = combo.Quantity;
            }

            if (!availableStock.HasValue || availableStock.Value <= 0)
                throw new InvalidOperationException("Sản phẩm đã hết hàng");

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            CartItem? existingItem = null;
            if (foodId.HasValue)
            {
                existingItem = await _unitOfWork.CartItems.GetByCartAndFoodAsync(cart.CartId, foodId.Value);
            }
            else if (comboId.HasValue)
            {
                existingItem = await _unitOfWork.CartItems.GetByCartAndComboAsync(cart.CartId, comboId.Value);
            }

            int newQuantity = quantity;
            if (existingItem != null)
            {
                newQuantity = existingItem.Quantity + quantity;
            }

            // Kiểm tra tổng số lượng yêu cầu có vượt quá tồn kho không
            if (newQuantity > availableStock.Value)
            {
                newQuantity = availableStock.Value;
                if (existingItem != null)
                {
                    existingItem.Quantity = newQuantity;
                    await _unitOfWork.CartItems.UpdateAsync(existingItem);
                }
                throw new InvalidOperationException($"Số lượng bạn chọn vượt quá tồn kho. Đã cập nhật còn lại {newQuantity}.");
            }


            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    FoodId = foodId,
                    ComboId = comboId,
                    Quantity = quantity
                };
                await _unitOfWork.CartItems.AddAsync(newItem);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task AddToCartByIdAsync(int cartId, int? foodId, int? comboId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetCartByIdAsync(cartId);
            if (cart == null)
                throw new ArgumentException("Cart không tồn tại");

            // Lấy sản phẩm hiện có để kiểm tra tồn kho
            int? availableStock = null;
            if (foodId.HasValue)
            {
                var food = await _unitOfWork.Foods.GetByIdAsync(foodId.Value);
                if (food == null)
                    throw new ArgumentException("Món ăn không tồn tại");

                availableStock = food.Quantity;
            }
            else if (comboId.HasValue)
            {
                var combo = await _unitOfWork.Combos.GetByIdAsync(comboId.Value);
                if (combo == null)
                    throw new ArgumentException("Combo không tồn tại");

                availableStock = combo.Quantity;
            }

            if (!availableStock.HasValue || availableStock.Value <= 0)
                throw new InvalidOperationException("Sản phẩm đã hết hàng");

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            CartItem? existingItem = null;
            if (foodId.HasValue)
            {
                existingItem = await _unitOfWork.CartItems.GetByCartAndFoodAsync(cart.CartId, foodId.Value);
            }
            else if (comboId.HasValue)
            {
                existingItem = await _unitOfWork.CartItems.GetByCartAndComboAsync(cart.CartId, comboId.Value);
            }

            int newQuantity = quantity;
            if (existingItem != null)
            {
                newQuantity = existingItem.Quantity + quantity;
            }

            // Kiểm tra tổng số lượng yêu cầu có vượt quá tồn kho không
            if (newQuantity > availableStock.Value)
            {
                newQuantity = availableStock.Value;
                if (existingItem != null)
                {
                    existingItem.Quantity = newQuantity;
                    await _unitOfWork.CartItems.UpdateAsync(existingItem);
                }
                throw new InvalidOperationException($"Số lượng bạn chọn vượt quá tồn kho. Đã cập nhật còn lại {newQuantity}.");
            }

            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    FoodId = foodId,
                    ComboId = comboId,
                    Quantity = quantity
                };
                await _unitOfWork.CartItems.AddAsync(newItem);
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCartItemAsync(int userId, int cartItemId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (item == null || item.CartId != cart.CartId)
                throw new Exception("Cart item not found or not owned by user");

            int? availableStock = null;

            if (item.FoodId.HasValue)
            {
                var food = await _unitOfWork.Foods.GetByIdAsync(item.FoodId.Value);
                if (food == null)
                    throw new Exception("Food not found");
                availableStock = food.Quantity;
            }
            else if (item.ComboId.HasValue)
            {
                var combo = await _unitOfWork.Combos.GetByIdAsync(item.ComboId.Value);
                if (combo == null)
                    throw new Exception("Combo not found");
                availableStock = combo.Quantity;
            }

            if (!availableStock.HasValue || availableStock == 0)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
                await _unitOfWork.CompleteAsync();
                throw new InvalidOperationException("Sản phẩm đã hết hàng và đã được xóa khỏi giỏ.");
            }

            if (quantity <= 0)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
                await _unitOfWork.CompleteAsync();
                return; // Không cần báo lỗi nếu user tự giảm về 0 => xóa
            }

            if (quantity > availableStock.Value)
            {
                item.Quantity = availableStock.Value;
                await _unitOfWork.CartItems.UpdateAsync(item);
                await _unitOfWork.CompleteAsync();
                throw new InvalidOperationException($"Số lượng vượt quá tồn kho. Đã cập nhật lại thành {availableStock.Value}.");
            }

            item.Quantity = quantity;
            await _unitOfWork.CartItems.UpdateAsync(item);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateCartItemByIdAsync(int cartId, int? foodId, int? comboId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetCartByIdAsync(cartId);
            if (cart == null)
                throw new ArgumentException("Cart không tồn tại");

            if (foodId == null && comboId == null)
                throw new ArgumentException("Phải chọn một loại sản phẩm (FoodId hoặc ComboId)");

            if (foodId != null && comboId != null)
                throw new ArgumentException("Chỉ được chọn một loại sản phẩm (Food hoặc Combo)");

            // Tìm CartItem trong cart
            CartItem? item = null;
            if (foodId.HasValue)
            {
                item = await _unitOfWork.CartItems.GetByCartAndFoodAsync(cartId, foodId.Value);
            }
            else if (comboId.HasValue)
            {
                item = await _unitOfWork.CartItems.GetByCartAndComboAsync(cartId, comboId.Value);
            }

            if (item == null)
                throw new ArgumentException("Sản phẩm không có trong giỏ hàng");

            int? availableStock = null;

            if (item.FoodId.HasValue)
            {
                var food = await _unitOfWork.Foods.GetByIdAsync(item.FoodId.Value);
                if (food == null)
                    throw new ArgumentException("Món ăn không tồn tại");
                availableStock = food.Quantity;
            }
            else if (item.ComboId.HasValue)
            {
                var combo = await _unitOfWork.Combos.GetByIdAsync(item.ComboId.Value);
                if (combo == null)
                    throw new ArgumentException("Combo không tồn tại");
                availableStock = combo.Quantity;
            }

            if (!availableStock.HasValue || availableStock == 0)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
                await _unitOfWork.CompleteAsync();
                throw new InvalidOperationException("Sản phẩm đã hết hàng và đã được xóa khỏi giỏ.");
            }

            if (quantity <= 0)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
                await _unitOfWork.CompleteAsync();
                return; // Xóa sản phẩm nếu quantity <= 0
            }

            if (quantity > availableStock.Value)
            {
                item.Quantity = availableStock.Value;
                await _unitOfWork.CartItems.UpdateAsync(item);
                await _unitOfWork.CompleteAsync();
                throw new InvalidOperationException($"Số lượng vượt quá tồn kho. Đã cập nhật lại thành {availableStock.Value}.");
            }

            item.Quantity = quantity;
            await _unitOfWork.CartItems.UpdateAsync(item);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RemoveCartItemAsync(int userId, int cartItemId)
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null) throw new Exception("Cart not found");

            var item = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (item == null || item.CartId != cart.CartId)
                throw new Exception("Cart item not found or not owned by user");

            await _unitOfWork.CartItems.RemoveAsync(item);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Tính giá cuối cùng sau khi áp dụng khuyến mãi
        /// </summary>
        /// <param name="originalPrice">Giá gốc</param>
        /// <param name="promotion">Thông tin khuyến mãi</param>
        /// <returns>Giá sau khuyến mãi</returns>
        private decimal CalculateFinalPrice(decimal originalPrice, PromotionDtoSelect? promotion)
        {
            // Không có khuyến mãi hoặc khuyến mãi không hoạt động
            if (promotion == null || !promotion.IsActive)
                return originalPrice;

            // Kiểm tra thời gian khuyến mãi có hiệu lực không
            var now = DateTime.Now;
            if (now < promotion.StartDate || now > promotion.EndDate)
                return originalPrice;

            // Tính giá sau khuyến mãi
            switch (promotion.Type)
            {
                case PromotionType.Amount:
                    // Giảm theo số tiền cố định
                    var discountedPrice = originalPrice - promotion.DiscountAmount;
                    return Math.Max(0, discountedPrice); // Đảm bảo giá không âm

                case PromotionType.Percentage:
                    // Giảm theo phần trăm
                    var discountPercent = promotion.DiscountAmount / 100;
                    return originalPrice * (1 - discountPercent);

                default:
                    return originalPrice;
            }
        }

        public async Task<CartResponse> CreateTemporaryCartAsync()
        {
            var cart = await _unitOfWork.Carts.CreateTemporaryCartAsync();
            await _unitOfWork.CompleteAsync();

            var cartDto = _mapper.Map<CartDto>(cart);

            return new CartResponse
            {
                Cart = cartDto,
            };
        }

        public async Task<bool> AssignCartToUserAsync(int cartId, int userId)
        {
            // Check if the cart exists and is temporary
            var cart = await _unitOfWork.Carts.GetCartByIdAsync(cartId);
            if (cart == null || cart.Temporary != true)
                return false;

            // Check if the user already has a cart
            //var existingCart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            //if (existingCart != null)
            //{
            //    // If user already has a cart, we might want to merge them
            //    // But for now, we'll just return false indicating we can't assign
            //    return false;
            //}

            // Update the cart with the user ID
            var success = await _unitOfWork.Carts.UpdateCartUserIdAsync(cartId, userId);
            if (success)
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<CartResponse>> GetAllTemporaryCartsAsync()
        {
            var carts = await _unitOfWork.Carts.GetAllTemporaryCartsAsync();
            var cartResponses = new List<CartResponse>();

            foreach (var cart in carts)
            {
                var cartDto = _mapper.Map<CartDto>(cart);

                // Calculate totals for each cart
                decimal subtotal = 0;
                decimal originalTotal = 0;
                int totalQuantity = 0;

                foreach (var item in cartDto.CartItems ?? Enumerable.Empty<CartItemDto>())
                {
                    decimal originalPrice = 0;
                    decimal finalPrice = 0;

                    if (item.Food != null)
                    {
                        originalPrice = item.Food.Price;
                        finalPrice = CalculateFinalPrice(originalPrice, item.Food.Promotion);
                    }
                    else if (item.Combo != null)
                    {
                        originalPrice = item.Combo.Price;
                        finalPrice = CalculateFinalPrice(originalPrice, item.Combo.Promotion);
                    }

                    // Update price info for the item
                    item.OriginalPrice = originalPrice;
                    item.FinalPrice = finalPrice;
                    item.OriginalTotal = originalPrice * item.Quantity;
                    item.FinalTotal = finalPrice * item.Quantity;
                    item.DiscountAmount = item.OriginalTotal - item.FinalTotal;

                    subtotal += item.FinalTotal;
                    originalTotal += item.OriginalTotal;
                    totalQuantity += item.Quantity;
                }

                var totalDiscount = originalTotal - subtotal;

                cartResponses.Add(new CartResponse
                {
                    Cart = cartDto,
                    Subtotal = subtotal,
                    OriginalTotal = originalTotal,
                    TotalDiscount = totalDiscount,
                    TotalQuantity = totalQuantity
                });
            }

            return cartResponses;
        }

        public async Task<CartResponse?> GetCartByIdAsync(int cartId)
        {
            var cart = await _unitOfWork.Carts.GetCartByIdAsync(cartId);
            if (cart == null) return null;

            var cartDto = _mapper.Map<CartDto>(cart);

            return new CartResponse
            {
                Cart = cartDto,
            };
        }

        public async Task<IEnumerable<CartDto>> GetAllTemporaryCartsBasicAsync()
        {
            var carts = await _unitOfWork.Carts.GetAllTemporaryCartsAsync();

            var cartDtos = new List<CartDto>();
            foreach (var cart in carts)
            {
                var cartDto = new CartDto
                {
                    CartId = cart.CartId,
                    UserId = cart.UserId ?? 0, // Xử lý nullable
                    CartItems = null // Không load cartItems
                };
                cartDtos.Add(cartDto);
            }

            return cartDtos;
        }

        public async Task<bool> DeleteCartByIdAsync(int cartId)
        {
            var result = await _unitOfWork.Carts.DeleteCartByIdAsync(cartId);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCartItemByIdAsync(int cartId, int cartItemId)
        {
            // Kiểm tra cart có tồn tại không
            var cart = await _unitOfWork.Carts.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return false;
            }

            // Tìm CartItem theo cartId và cartItemId
            var cartItem = await _unitOfWork.Carts.GetCartItemByIdAsync(cartId, cartItemId);
            if (cartItem == null)
            {
                return false;
            }

            // Xóa CartItem
            await _unitOfWork.Carts.DeleteCartItemAsync(cartItem);

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
