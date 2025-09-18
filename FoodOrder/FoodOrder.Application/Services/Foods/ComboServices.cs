using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Foods
{
    internal class ComboServices : IComboServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SlugService _slugService;
        public ComboServices(IUnitOfWork unitOfWork, IMapper mapper, SlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slugService = slugService;
        }

        public async Task<PagedResult<ComboDto>> GetPagedCombosAsync(PagedQuery query)
        {
            var comboQuery = _unitOfWork.Combos.GetQueryableWithIncludes();
            if (!string.IsNullOrWhiteSpace(query.CategoryName))
            {
                comboQuery = comboQuery.Where(f => f.FoodCategorys.CategoryName.Contains(query.CategoryName));
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                comboQuery = comboQuery.Where(c => c.ComboName.Contains(query.Name));
            }
            if (query.Status.HasValue)
            {
                comboQuery = comboQuery.Where(f => f.Status == query.Status.Value);
            }
            if (query.IsOutOfStock.HasValue)
            {
                if (query.IsOutOfStock.Value)
                {
                    // Lấy những combo đã hết hàng
                    comboQuery = comboQuery.Where(f => f.Quantity == 0);
                }
                else
                {
                    // Lấy những combo còn hàng
                    comboQuery = comboQuery.Where(f => f.Quantity > 0);
                }
            }

            comboQuery = query.SortOrder.ToLower() switch
            {
                "asc" => comboQuery.OrderBy(c => c.CreatedAt),
                _ => comboQuery.OrderByDescending(c => c.CreatedAt)
            };

            var totalCount = await comboQuery.CountAsync();

            var items = await comboQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var comboDtos = _mapper.Map<List<ComboDto>>(items);

            return new PagedResult<ComboDto>(comboDtos, totalCount, query.Page, query.PageSize);
        }


        public async Task<ComboDtoById?> GetByIdAsync(int id)
        {
            var combo = await _unitOfWork.Combos.GetByIdAsync(id);
            return _mapper.Map<ComboDtoById>(combo);
        }

        public async Task<ComboDto?> GetBySlugAsync(string slug)
        {
            var combo = await _unitOfWork.Combos.GetBySlugAsync(slug);
            return _mapper.Map<ComboDto>(combo);
        }

        public async Task<ComboWithFoodDto?> GetComboWithFoodsBySlugAsync(string slug)
        {
            var comboWithFoods = await _unitOfWork.Combos
                                    .GetComboWithFoodsBySlug(slug)
                                    .FirstOrDefaultAsync();

            return _mapper.Map<ComboWithFoodDto>(comboWithFoods);
        }

        public async Task<List<FoodDto>> GetFoodsNotInComboAsync()
        {
            var allFoods = await _unitOfWork.Foods.GetAllAsync();
            return _mapper.Map<List<FoodDto>>(allFoods);
        }

        public async Task<List<ComboDto>> GetAllComboAsync()
        {
            var allCombos = await _unitOfWork.Combos.GetAllComboAsync();
            return _mapper.Map<List<ComboDto>>(allCombos);
        }


        public async Task UpdateCombosByFoodIdAsync(int foodId)
        {
            var food = await _unitOfWork.Foods.GetByIdAsync(foodId);
            if (food == null) throw new ArgumentException("Food not found");

            var combos = await _unitOfWork.Combos.GetCombosByFoodIdAsync(foodId);

            foreach (var combo in combos)
            {
                if (!food.Status )
                {
                    combo.Status = false;
                }
                else
                {
                    var allFoods = await _unitOfWork.Combos.GetFoodsInComboAsync(combo.ComboId);
                    bool allAvailable = allFoods.All(f => f.Status);
                    combo.Status = allAvailable;
                }
            }
        }

        public async Task<bool> UpdateComboStatusAsync(int id, bool isActive)
        {
            var combo = await _unitOfWork.Combos.GetByIdAsync(id);
            if (combo == null)
                throw new KeyNotFoundException("Không tìm thấy combo");

            // Nếu bật thì kiểm tra các Food
            if (isActive)
            {
                var hasInactiveFood = combo.ComboDetails.Any(cd => cd.Food != null && !cd.Food.Status);
                if (hasInactiveFood)
                    throw new InvalidOperationException("Không thể mở bán combo vì có món ăn đang bị tắt");
            }

            combo.Status = isActive;
            await _unitOfWork.Combos.UpdateAsync(combo);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }

        #region crud

        public async Task<bool> AddAsync(ComboDtoCreate dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ComboName))
                throw new ArgumentException("Tên combo không được để trống.");
            if (dto.ComboName.Length > 200)
                throw new ArgumentException("Tên combo không được vượt quá 200 ký tự.");

            if (dto.Price <= 0)
                throw new ArgumentException("Giá combo phải lớn hơn 0.");

            if (dto.Quantity < 0)
                throw new ArgumentException("Số lượng combo không được nhỏ hơn 0.");

            if (dto.FoodCategoryId <= 0)
                throw new ArgumentException("Danh mục món ăn không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description.Length > 500)
                throw new ArgumentException("Mô tả tối đa 500 ký tự");

            if (dto.Foods == null || !dto.Foods.Any())
                throw new ArgumentException("Phải chọn ít nhất một món ăn cho combo.");

            if (dto.Foods.Any(f => f.FoodId <= 0))
                throw new ArgumentException("Mỗi món ăn trong combo phải có ID hợp lệ.");

            if (dto.Foods.Any(f => f.Quantity <= 0))
                throw new ArgumentException("Số lượng của mỗi món ăn trong combo phải lớn hơn 0.");

            var existCombo = await _unitOfWork.Combos
              .FirstOrDefaultAsync(f => f.ComboName == dto.ComboName.Trim());
            if (existCombo != null)
                throw new InvalidOperationException("Tên combo đã tồn tại");

            // Kiểm tra danh mục có tồn tại không
            var category = await _unitOfWork.FoodCategories.GetByIdAsync(dto.FoodCategoryId);
            if (category == null)
                throw new ArgumentException("Danh mục không tồn tại.");

            // Kiểm tra từng món ăn có tồn tại không
            foreach (var food in dto.Foods)
            {
                var exists = await _unitOfWork.Foods.GetByIdAsync(food.FoodId);
                if (exists == null)
                    throw new ArgumentException($"Món ăn với ID {food.FoodId} không tồn tại.");
            }

            // Nếu có PromotionId thì kiểm tra khuyến mãi còn hiệu lực không
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


            // 2. Tạo Combo entity
            var combo = new Combo
            {
                ComboName = dto.ComboName,
                Description = dto.Description,
                Price = dto.Price,
                FoodCategoryId = dto.FoodCategoryId,
                CreatedAt = DateTime.UtcNow,
                Slug = await _slugService.GenerateUniqueSlug<Combo>(dto.ComboName!),
                Status = true,
                Quantity = dto.Quantity,
                PromotionId = dto.PromotionId
            };

            // 3. Gán ảnh nếu có
            if (dto.Images != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Images.Id) || string.IsNullOrWhiteSpace(dto.Images.Url))
                    throw new ArgumentException("Ảnh combo không hợp lệ.");

                combo.Images = new Images
                {
                    Id = dto.Images.Id,
                    Url = dto.Images.Url,
                    ThumbnailUrl = dto.Images.ThumbnailUrl,
                    Name = dto.Images.Name
                };
            }

            // 4. Thêm combo vào DB
            var result = await _unitOfWork.Combos.AddAsync(combo);
            if (!result) return false;

            await _unitOfWork.CompleteAsync(); // đảm bảo có ComboId

            // 5. Thêm combo details (food)
            foreach (var food in dto.Foods)
            {
                var comboDetail = new ComboDetail
                {
                    ComboId = combo.ComboId,
                    FoodId = food.FoodId,
                    Quantity = food.Quantity
                };

                await _unitOfWork.ComboDetails.AddAsync(comboDetail);
            }

            // 6. Save lần cuối
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ComboDtoUpdate dto)
        {
            try
            {
                // 1. VALIDATE cơ bản
                if (dto.ComboId <= 0)
                    throw new ArgumentException("ComboId không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.ComboName))
                    throw new ArgumentException("Tên combo không được để trống.");

                if (dto.Price <= 0)
                    throw new ArgumentException("Giá combo phải lớn hơn 0.");

                if (dto.FoodCategoryId <= 0)
                    throw new ArgumentException("Danh mục món ăn không hợp lệ.");

                if (dto.Foods == null || !dto.Foods.Any())
                    throw new ArgumentException("Combo cần ít nhất một món ăn.");

                if (dto.Foods.Any(f => f.FoodId <= 0 || f.Quantity <= 0))
                    throw new ArgumentException("Mỗi món ăn phải có ID hợp lệ và số lượng > 0.");

                // 2. Kiểm tra trùng tên combo
                var existCombo = await _unitOfWork.Combos.FirstOrDefaultAsync(f => f.ComboName == dto.ComboName.Trim());
                if (existCombo != null && existCombo.ComboId != dto.ComboId)
                    throw new InvalidOperationException("Tên combo đã tồn tại");

                // check trùng món ăn
                if (dto.Foods.GroupBy(f => f.FoodId).Any(g => g.Count() > 1))
                    throw new ArgumentException("Combo có chứa món ăn bị trùng lặp.");

                // 2. Tìm combo hiện tại
                var combo = await _unitOfWork.Combos.GetByIdAsync(dto.ComboId);
                if (combo == null)
                    throw new ArgumentException("Combo không tồn tại.");

                // 3. Kiểm tra danh mục tồn tại
                var category = await _unitOfWork.FoodCategories.GetByIdAsync(dto.FoodCategoryId);
                if (category == null)
                    throw new ArgumentException("Danh mục không tồn tại.");

                // Nếu có PromotionId thì kiểm tra khuyến mãi còn hiệu lực không
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

                // 4. Cập nhật thông tin cơ bản
                combo.ComboName = dto.ComboName;
                combo.Slug = await _slugService.GenerateUniqueSlug<Combo>(dto.ComboName!);
                combo.Description = dto.Description;
                combo.Price = dto.Price;
                combo.FoodCategoryId = dto.FoodCategoryId;
                combo.Status = dto.Status;
                combo.Quantity = dto.Quantity;
                combo.PromotionId = dto.PromotionId;
                // 5. Cập nhật ảnh
                if (dto.Images != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.Images.Id) || string.IsNullOrWhiteSpace(dto.Images.Url))
                        throw new ArgumentException("Ảnh combo không hợp lệ.");

                    if (combo.Images == null || combo.Images.Id != dto.Images.Id)
                    {
                        if (combo.Images != null)
                            await _unitOfWork.Images.DeleteAsync(combo.Images.Id!);

                        combo.Images = new Images
                        {
                            Id = dto.Images.Id,
                            Url = dto.Images.Url,
                            ThumbnailUrl = dto.Images.ThumbnailUrl,
                            Name = dto.Images.Name
                        };
                    }
                }

                // 6. Xoá combo detail cũ
                var oldDetails = await _unitOfWork.ComboDetails.GetByComboIdAsync(dto.ComboId);
                foreach (var detail in oldDetails)
                {
                    await _unitOfWork.ComboDetails.DeleteAsync(detail.ComboId, detail.FoodId);
                }

                // 7. Thêm combo detail mới
                foreach (var food in dto.Foods)
                {
                    var exists = await _unitOfWork.Foods.GetByIdAsync(food.FoodId);
                    if (exists == null)
                        throw new ArgumentException($"Món ăn ID {food.FoodId} không tồn tại.");

                    var comboDetail = new ComboDetail
                    {
                        ComboId = combo.ComboId,
                        FoodId = food.FoodId,
                        Quantity = food.Quantity
                    };

                    await _unitOfWork.ComboDetails.AddAsync(comboDetail);
                }

                // 8. Cập nhật combo và lưu
                var updated = await _unitOfWork.Combos.UpdateAsync(combo);
                if (!updated) return false;

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Lỗi khi lưu EF: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Combos.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }

      

        #endregion
    }
}
