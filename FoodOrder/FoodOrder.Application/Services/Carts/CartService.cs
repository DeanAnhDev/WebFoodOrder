using AutoMapper;
using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Application.Interfaces;
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
                    TotalQuantity = 0
                };
            }

            // Danh sách các item cần xoá vì hết hàng
            var itemsToRemove = new List<CartItem>();

            foreach (var item in cart.CartItems.ToList())
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

            // Tính tổng tiền và số lượng
            decimal subtotal = 0;
            int totalQuantity = 0;

            foreach (var item in cartDto.CartItems ?? Enumerable.Empty<CartItemDto>())
            {
                decimal price = 0;

                if (item.Food != null)
                    price = item.Food.Price;
                else if (item.Combo != null)
                    price = item.Combo.Price;

                subtotal += price * item.Quantity;
                totalQuantity += item.Quantity;
            }

            return new CartResponse
            {
                Cart = cartDto,
                Subtotal = subtotal,
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
                existingItem.Quantity = newQuantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
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

    }
}
