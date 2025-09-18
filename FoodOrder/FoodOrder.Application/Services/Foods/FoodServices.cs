using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Foods
{
    internal class FoodServices : IFoodServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SlugService _slugService;
        private readonly IComboServices _comboService; 
        public FoodServices(IUnitOfWork unitOfWork, IMapper mapper, SlugService slugService, IComboServices comboService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slugService = slugService;
            _comboService = comboService;
        }

        public async Task<PagedResult<FoodDto>> GetPagedFoodsAsync(PagedQuery query)
        {
            var foodsQuery = _unitOfWork.Foods.GetQueryableWithIncludes();

            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                foodsQuery = foodsQuery.Where(f => f.FoodCategory.CategoryName.Contains(query.CategoryName));
            }
            // Lọc theo tên món ăn nếu có
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                foodsQuery = foodsQuery.Where(f =>
                    f.FoodName != null &&
                    f.FoodName.Contains(query.Name));
            }
            if (query.Status.HasValue)
            {
                foodsQuery = foodsQuery.Where(f => f.Status == query.Status.Value);
            }
            if (query.IsOutOfStock.HasValue)
            {
                if (query.IsOutOfStock.Value)
                {
                    // Lấy những combo đã hết hàng
                    foodsQuery = foodsQuery.Where(f => f.Quantity == 0);
                }
                else
                {
                    // Lấy những combo còn hàng
                    foodsQuery = foodsQuery.Where(f => f.Quantity > 0);
                }
            }
            // Sắp xếp theo CreatedAt
            foodsQuery = query.SortOrder.ToLower() switch
            {
                "asc" => foodsQuery.OrderBy(f => f.CreatedAt),
                _ => foodsQuery.OrderByDescending(f => f.CreatedAt)
            };

            var totalCount = await foodsQuery.CountAsync();

            var items = await foodsQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var foodDtos = _mapper.Map<List<FoodDto>>(items);

            return new PagedResult<FoodDto>(foodDtos, totalCount, query.Page, query.PageSize);
        }

        public async Task<FoodDto> GetByIdAsync(int id)
        {
            var foods = await _unitOfWork.Foods.GetByIdAsync(id);
            return _mapper.Map<FoodDto>(foods);
        }

        public async Task<FoodDto> GetBySlugAsync(string slug)
        {
            var foods = await _unitOfWork.Foods.GetBySlugAsync(slug);
            return _mapper.Map<FoodDto>(foods);
        }

        public async Task<bool> AddAsync(FoodDtoCreate foodDto)
        {
            // ✅ Validate cơ bản
            if (string.IsNullOrWhiteSpace(foodDto.FoodName))
                throw new ArgumentException("Tên món ăn không được để trống");

            if (foodDto.FoodName.Length > 200)
                throw new ArgumentException("Tên món ăn tối đa 200 ký tự");

            if (!string.IsNullOrWhiteSpace(foodDto.Description) && foodDto.Description.Length > 500)
                throw new ArgumentException("Mô tả tối đa 500 ký tự");

            if (foodDto.Price <= 0)
                throw new ArgumentException("Giá món ăn phải lớn hơn 0");

            if (foodDto.Quantity < 0)
                throw new ArgumentException("Số lượng không được âm");

            if (foodDto.Images == null || string.IsNullOrEmpty(foodDto.Images.Id))
                throw new ArgumentException("Món ăn cần có ảnh");

            // ✅ Validate business: Category phải tồn tại
            var existingCategory = await _unitOfWork.FoodCategories.GetByIdAsync(foodDto.FoodCategoryId);
            if (existingCategory == null)
                throw new ArgumentException("Danh mục không tồn tại");

            // ✅ Check tên món ăn trùng
            var existFood = await _unitOfWork.Foods
                .FirstOrDefaultAsync(f => f.FoodName == foodDto.FoodName.Trim());
            if (existFood != null)
                throw new InvalidOperationException("Tên món ăn đã tồn tại");

            Promotion? promotion = null;
            if (foodDto.PromotionId.HasValue)
            {
                promotion = await _unitOfWork.Promotions.GetByIdWithRelationsAsync(foodDto.PromotionId.Value);
                if (promotion == null)
                    throw new ArgumentException("Khuyến mãi không tồn tại");

                if (!promotion.IsActive || promotion.EndDate < DateTime.UtcNow)
                    throw new ArgumentException("Khuyến mãi không còn hiệu lực");

                // Nếu kiểu Amount thì phải check giá
                if (promotion.Type == PromotionType.Amount && promotion.DiscountAmount >= foodDto.Price)
                    throw new ArgumentException("Giá trị giảm phải nhỏ hơn giá combo");
            }

            var food = _mapper.Map<Food>(foodDto);
            if (food.FoodName != null)
            {
                food.Slug = await _slugService.GenerateUniqueSlug<Food>(food.FoodName);
            }
            var result = await _unitOfWork.Foods.AddAsync(food);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(FoodDtoUpdate dto)
        {
            try
            {
                // ✅ Kiểm tra món ăn tồn tại
                var existing = await _unitOfWork.Foods.GetByIdAsync(dto.FoodId);
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy món ăn");

                // ✅ Validate tên
                if (!string.IsNullOrWhiteSpace(dto.FoodName))
                {
                    if (dto.FoodName.Length > 200)
                        throw new ArgumentException("Tên món ăn không được vượt quá 200 ký tự");

                    var existFood = await _unitOfWork.Foods.FirstOrDefaultAsync(f => f.FoodName == dto.FoodName.Trim());
                    if (existFood != null && existFood.FoodId != dto.FoodId)
                        throw new InvalidOperationException("Tên món ăn đã tồn tại");

                    existing.FoodName = dto.FoodName;
                    existing.Slug = await _slugService.GenerateUniqueSlug<Food>(dto.FoodName);
                }

                // ✅ Validate giá
                if (dto.Price <= 0)
                    throw new ArgumentException("Giá phải lớn hơn 0");
                existing.Price = dto.Price;

                // ✅ Validate số lượng
                if (dto.Quantity < 0)
                    throw new ArgumentException("Số lượng không được nhỏ hơn 0");
                existing.Quantity = dto.Quantity;

                // ✅ Validate danh mục
                var category = await _unitOfWork.FoodCategories.GetByIdAsync(dto.FoodCategoryId);
                if (category == null)
                    throw new ArgumentException("Danh mục món ăn không tồn tại");
                existing.FoodCategoryId = dto.FoodCategoryId;

                // ✅ Trạng thái
                existing.Status = dto.Status;

                // ✅ Mô tả
                if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 500)
                    throw new ArgumentException("Mô tả không được vượt quá 500 ký tự");
                existing.Description = dto.Description;

                Promotion? promotion = null;
                if (dto.PromotionId.HasValue)
                {
                    promotion = await _unitOfWork.Promotions.GetByIdWithRelationsAsync(dto.PromotionId.Value);
                    if (promotion == null)
                        throw new ArgumentException("Khuyến mãi không tồn tại");

                    if (!promotion.IsActive || promotion.EndDate < DateTime.UtcNow)
                        throw new ArgumentException("Khuyến mãi không còn hiệu lực");

                    // Nếu kiểu Amount thì phải check giá
                    if (promotion.Type == PromotionType.Amount && promotion.DiscountAmount >= dto.Price)
                        throw new ArgumentException("Giá trị giảm phải nhỏ hơn giá combo");
                }

                // ✅ Quản lý ảnh
                var dbImage = await _unitOfWork.Images.FirstOrDefaultAsync(i => i.FoodId == dto.FoodId);
                var newImage = dto.Images;

                if (newImage != null)
                {
                    if (string.IsNullOrEmpty(newImage.Id))
                        throw new ArgumentException("Ảnh phải có Id hợp lệ");

                    if (dbImage == null || dbImage.Id != newImage.Id)
                    {
                        if (dbImage != null)
                            await _unitOfWork.Images.DeleteAsync(dbImage.Id!);

                        var imageEntity = new Images
                        {
                            Id = newImage.Id,
                            Url = newImage.Url,
                            ThumbnailUrl = newImage.ThumbnailUrl,
                            Name = newImage.Name,
                            FoodId = dto.FoodId
                        };

                        await _unitOfWork.Images.AddAsync(imageEntity);
                    }
                    else
                    {
                        dbImage.Url = newImage.Url;
                        dbImage.ThumbnailUrl = newImage.ThumbnailUrl;
                        dbImage.Name = newImage.Name;
                    }
                }

                // ✅ Cập nhật database
                existing.PromotionId = dto.PromotionId;
                var updated = await _unitOfWork.Foods.UpdateAsync(existing);
                if (!updated) return false;

                await _comboService.UpdateCombosByFoodIdAsync(dto.FoodId);
                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Lỗi lưu dữ liệu EF: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Foods.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }

        public async Task<bool> UpdateFoodStatusAsync(int id, bool isActive)
        {
            var food = await _unitOfWork.Foods.GetByIdAsync(id);
            if (food == null)
                throw new KeyNotFoundException("Không tìm thấy món ăn");

            food.Status = isActive;

            await _comboService.UpdateCombosByFoodIdAsync(id);
            await _unitOfWork.Foods.UpdateAsync(food);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }
    }
}
