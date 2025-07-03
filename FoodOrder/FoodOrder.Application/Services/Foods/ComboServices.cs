using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
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
                comboQuery = comboQuery.Where(f => f.IsOutOfStock == query.IsOutOfStock.Value);
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


        public async Task<ComboDto?> GetByIdAsync(int id)
        {
            var combo = await _unitOfWork.Combos.GetByIdAsync(id);
            return _mapper.Map<ComboDto>(combo);
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



        public async Task<bool> AddAsync(ComboDtoCreate dto)
        {
            // 1. VALIDATE DỮ LIỆU
            if (string.IsNullOrWhiteSpace(dto.ComboName))
                throw new ArgumentException("Tên combo không được để trống.");

            if (dto.Price <= 0)
                throw new ArgumentException("Giá combo phải lớn hơn 0.");

            if (dto.FoodCategoryId <= 0)
                throw new ArgumentException("Danh mục món ăn không hợp lệ.");

            if (dto.Foods == null || !dto.Foods.Any())
                throw new ArgumentException("Phải chọn ít nhất một món ăn cho combo.");

            if (dto.Foods.Any(f => f.FoodId <= 0 || f.Quantity <= 0))
                throw new ArgumentException("Mỗi món ăn phải có ID hợp lệ và số lượng > 0.");

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
                IsOutOfStock = false
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
                if (dto.ComboId <= 0) throw new ArgumentException("ComboId không hợp lệ.");
                if (string.IsNullOrWhiteSpace(dto.ComboName)) throw new ArgumentException("Tên combo không được để trống.");
                if (dto.Price <= 0) throw new ArgumentException("Giá combo phải lớn hơn 0.");
                if (dto.FoodCategoryId <= 0) throw new ArgumentException("Danh mục món ăn không hợp lệ.");
                if (dto.Foods == null || !dto.Foods.Any()) throw new ArgumentException("Combo cần ít nhất một món ăn.");

                // 2. Tìm combo hiện tại
                var combo = await _unitOfWork.Combos.GetByIdAsync(dto.ComboId);
                if (combo == null) throw new ArgumentException("Combo không tồn tại.");

                // 3. Cập nhật thông tin cơ bản
                combo.ComboName = dto.ComboName;
                combo.Slug = await _slugService.GenerateUniqueSlug<Combo>(dto.ComboName!);
                combo.Description = dto.Description;
                combo.Price = dto.Price;
                combo.FoodCategoryId = dto.FoodCategoryId;

                // 4. Cập nhật ảnh
                if (dto.Images != null)
                {
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

                // 5. Xoá combo detail cũ
                var oldDetails = await _unitOfWork.ComboDetails
                    .GetByComboIdAsync(dto.ComboId);
                foreach (var detail in oldDetails)
                {
                    await _unitOfWork.ComboDetails.DeleteAsync(detail.ComboId, detail.FoodId);
                }

                // 6. Thêm lại combo detail mới
                foreach (var food in dto.Foods)
                {
                    var exists = await _unitOfWork.Foods.GetByIdAsync(food.FoodId);
                    if (exists == null) throw new ArgumentException($"Món ăn ID {food.FoodId} không tồn tại.");

                    var comboDetail = new ComboDetail
                    {
                        ComboId = combo.ComboId,
                        FoodId = food.FoodId,
                        Quantity = food.Quantity
                    };

                    await _unitOfWork.ComboDetails.AddAsync(comboDetail);
                }

                // 7. Cập nhật combo và lưu
                var updated = await _unitOfWork.Combos.UpdateAsync(combo);
                if (!updated) return false;

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"EF Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
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
    }
}
