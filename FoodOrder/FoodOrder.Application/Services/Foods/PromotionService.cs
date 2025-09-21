using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Promotion;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Foods
{
    internal class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region crud
        public async Task<bool> CreatePromotionAsync(PromotionCreateDto dto)
        {
            // ==== VALIDATION ====
            if (string.IsNullOrWhiteSpace(dto.PromotionName))
                throw new ArgumentException("Tên khuyến mãi không được để trống");

            var existed = await _unitOfWork.Promotions.FindAsync(p => p.PromotionName == dto.PromotionName);
            if (existed.Any())
                throw new ArgumentException("Tên khuyến mãi đã tồn tại");

            if (dto.DiscountAmount <= 0)
                throw new ArgumentException("Giá trị giảm phải lớn hơn 0");

            if (dto.Type == PromotionType.Percentage && (dto.DiscountAmount <= 0 || dto.DiscountAmount > 100))
                throw new ArgumentException("Giảm theo phần trăm phải nằm trong khoảng 1-100");

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");
            if (dto.StartDate < DateTime.Today)
                throw new ArgumentException("Ngày kết thúc không được nhỏ hơn hiện tại");
            if (dto.EndDate < DateTime.Today)
                throw new ArgumentException("Ngày kết thúc không được nhỏ hơn hiện tại");

            if (dto.Type == PromotionType.Amount && (dto.FoodIds.Any() || dto.ComboIds.Any()))
            {
                var foodPrices = new List<decimal>();
                var comboPrices = new List<decimal>();

                if (dto.FoodIds.Any())
                {
                    var foods = await _unitOfWork.Foods
                        .FindAsync(f => dto.FoodIds.Contains(f.FoodId));
                    foodPrices.AddRange(foods.Select(f => f.Price));
                }

                if (dto.ComboIds.Any())
                {
                    var combos = await _unitOfWork.Combos
                        .FindAsync(c => dto.ComboIds.Contains(c.ComboId));
                    comboPrices.AddRange(combos.Select(c => c.Price));
                }

                if (foodPrices.Any() || comboPrices.Any())
                {
                    var minPrice = foodPrices.Concat(comboPrices).Min();
                    if (dto.DiscountAmount >= minPrice)
                        throw new ArgumentException($"Số tiền giảm phải nhỏ hơn giá sản phẩm thấp nhất ({minPrice})");
                }
            }

            var promotion = new Promotion
            {
                PromotionName = dto.PromotionName,
                DiscountAmount = dto.DiscountAmount,
                Type = dto.Type,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
            };

            await _unitOfWork.Promotions.AddAsync(promotion);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                throw new Exception("Không thể lưu khuyến mãi");

            if (dto.Type == PromotionType.Amount && (dto.FoodIds.Any() || dto.ComboIds.Any()))
            {
                var foodPrices = new List<decimal>();
                var comboPrices = new List<decimal>();

                if (dto.FoodIds.Any())
                {
                    var foods = await _unitOfWork.Foods
                        .FindAsync(f => dto.FoodIds.Contains(f.FoodId));
                    foodPrices.AddRange(foods.Select(f => f.Price));
                }

                if (dto.ComboIds.Any())
                {
                    var combos = await _unitOfWork.Combos
                        .FindAsync(c => dto.ComboIds.Contains(c.ComboId));
                    comboPrices.AddRange(combos.Select(c => c.Price));
                }

                if (foodPrices.Any() || comboPrices.Any())
                {
                    var minPrice = foodPrices.Concat(comboPrices).Min();
                    if (dto.DiscountAmount >= minPrice)
                        throw new ArgumentException($"Số tiền giảm phải nhỏ hơn giá sản phẩm thấp nhất ({minPrice})");
                }
            }


            return true;
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdWithRelationsAsync(id);
            if (promotion == null)
                throw new ArgumentException("Không tìm thấy khuyến mãi");

            // Xóa liên kết với Foods
            var foods = await _unitOfWork.Foods.FindAsync(f => f.PromotionId == id);
            foreach (var food in foods)
            {
                food.PromotionId = null;
            }

            // Xóa liên kết với Combos
            var combos = await _unitOfWork.Combos.FindAsync(c => c.PromotionId == id);
            foreach (var combo in combos)
            {
                combo.PromotionId = null;
            }

            await _unitOfWork.Promotions.DeleteAsync(promotion);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                throw new Exception("Không thể xóa khuyến mãi");

            return true;
        }


        public async Task<bool> UpdatePromotionAsync(int id, PromotionUpdateDto dto)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdWithRelationsAsync(id);
            if (promotion == null)
                throw new ArgumentException("Không tìm thấy khuyến mãi");

            // ==== VALIDATION ====
            if (string.IsNullOrWhiteSpace(dto.PromotionName))
                throw new ArgumentException("Tên khuyến mãi không được để trống");

            var existed = await _unitOfWork.Promotions
                .FindAsync(p => p.PromotionName == dto.PromotionName && p.PromotionId != id);
            if (existed.Any())
                throw new ArgumentException("Tên khuyến mãi đã tồn tại");

            if (dto.DiscountAmount <= 0)
                throw new ArgumentException("Giá trị giảm phải lớn hơn 0");

            if (dto.Type == PromotionType.Percentage && (dto.DiscountAmount <= 0 || dto.DiscountAmount > 100))
                throw new ArgumentException("Giảm theo phần trăm phải nằm trong khoảng 1-100");

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn ngày kết thúc");

            if (dto.EndDate < DateTime.Today)
                throw new ArgumentException("Ngày kết thúc không được nhỏ hơn hiện tại");


            // Nếu là giảm tiền trực tiếp thì phải nhỏ hơn giá món/combo rẻ nhất
            //if (dto.Type == PromotionType.Amount)
            //{
            //    var foodPrices = new List<decimal>();
            //    var comboPrices = new List<decimal>();

            //    if (dto.FoodIds.Any())
            //    {
            //        var foods = await _unitOfWork.Foods
            //            .FindAsync(f => dto.FoodIds.Contains(f.FoodId));
            //        foodPrices.AddRange(foods.Select(f => f.Price));
            //    }

            //    if (dto.ComboIds.Any())
            //    {
            //        var combos = await _unitOfWork.Combos
            //            .FindAsync(c => dto.ComboIds.Contains(c.ComboId));
            //        comboPrices.AddRange(combos.Select(c => c.Price));
            //    }

            //    if (foodPrices.Any() || comboPrices.Any())
            //    {
            //        var minPrice = foodPrices.Concat(comboPrices).Min();
            //        if (dto.DiscountAmount >= minPrice)
            //            throw new ArgumentException($"Số tiền giảm phải nhỏ hơn giá sản phẩm thấp nhất ({minPrice})");
            //    }
            //}

            // ==== UPDATE ====
            promotion.PromotionName = dto.PromotionName;
            promotion.DiscountAmount = dto.DiscountAmount;
            promotion.Type = dto.Type;
            promotion.StartDate = dto.StartDate;
            promotion.EndDate = dto.EndDate;
            promotion.IsActive = dto.IsActive;

            // Reset các quan hệ cũ
            var foods = await _unitOfWork.Foods.FindAsync(f => f.PromotionId == promotion.PromotionId);
            foreach (var food in foods)
            {
                bool valid = dto.Type switch
                {
                    PromotionType.Percentage => dto.DiscountAmount > 0 && dto.DiscountAmount <= 100,
                    PromotionType.Amount => dto.DiscountAmount < food.Price,
                    _ => false
                };

                if (!valid)
                    food.PromotionId = null; // bỏ nếu không hợp lệ
            }

            // ==== CHECK LẠI COMBOS ====
            var combos = await _unitOfWork.Combos.FindAsync(c => c.PromotionId == promotion.PromotionId);
            foreach (var combo in combos)
            {
                bool valid = dto.Type switch
                {
                    PromotionType.Percentage => dto.DiscountAmount > 0 && dto.DiscountAmount <= 100,
                    PromotionType.Amount => dto.DiscountAmount < combo.Price,
                    _ => false
                };

                if (!valid)
                    combo.PromotionId = null; // bỏ nếu không hợp lệ
            }

            // Gán Foods mới
            //if (dto.FoodIds.Any())
            //{
            //    var foods = await _unitOfWork.Foods
            //        .FindAsync(f => dto.FoodIds.Contains(f.FoodId));
            //    foreach (var food in foods)
            //    {
            //        food.PromotionId = promotion.PromotionId;
            //    }
            //}

            // Gán Combos mới
            //if (dto.ComboIds.Any())
            //{
            //    var combos = await _unitOfWork.Combos
            //        .FindAsync(c => dto.ComboIds.Contains(c.ComboId));
            //    foreach (var combo in combos)
            //    {
            //        combo.PromotionId = promotion.PromotionId;
            //    }
            //}

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                throw new Exception("Không thể cập nhật khuyến mãi");

            return true;
        }
        #endregion

        public async Task<PagedResult<PromotionDto>> GetAllPromotionsAsync(PromotionQuery query)
        {
            var promotions = await _unitOfWork.Promotions.GetAllWithRelationsAsync();

            // Áp dụng lọc
            if (query.StartDateFrom.HasValue)
                promotions = promotions.Where(p => p.StartDate >= query.StartDateFrom.Value);

            if (query.StartDateTo.HasValue)
                promotions = promotions.Where(p => p.StartDate <= query.StartDateTo.Value);

            if (query.IsActive.HasValue)
                promotions = promotions.Where(p => p.IsActive == query.IsActive.Value);

            var totalCount = promotions.Count();

            // Phân trang
            var items = promotions
                .OrderByDescending(p => p.StartDate)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            // Map sang DTO
            var mapped = _mapper.Map<List<PromotionDto>>(items);

            return new PagedResult<PromotionDto>(
                mapped,
                totalCount,
                query.PageNumber,
                query.PageSize
            );
        }


        public async Task<PromotionDto> GetPromotionByIdAsync(int id)
        {
            var promotion = await _unitOfWork.Promotions.GetByIdWithRelationsAsync(id);
            if (promotion == null)
                throw new KeyNotFoundException("Không tìm thấy khuyến mãi");

            return _mapper.Map<PromotionDto>(promotion);
        }
    }
}
