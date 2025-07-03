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
        public FoodServices(IUnitOfWork unitOfWork, IMapper mapper, SlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slugService = slugService;
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
                foodsQuery = foodsQuery.Where(f => f.IsOutOfStock == query.IsOutOfStock.Value);
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
            if (foodDto.Images == null || string.IsNullOrEmpty(foodDto.Images.Id))
            {
                return false;
            }
            var existingCategory = await _unitOfWork.FoodCategories.GetByIdAsync(foodDto.FoodCategoryId);
            if (existingCategory == null)
                throw new ArgumentException("Category not found");

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
                var existing = await _unitOfWork.Foods.GetByIdAsync(dto.FoodId);
                if (existing == null)
                    throw new ArgumentException("Food not found");

                // Cập nhật tên + slug
                if (!string.IsNullOrWhiteSpace(dto.FoodName))
                {
                    existing.FoodName = dto.FoodName;
                    existing.Slug = await _slugService.GenerateUniqueSlug<Food>(dto.FoodName);
                }

                // Cập nhật các thuộc tính khác
                existing.Description = dto.Description;
                existing.Price = dto.Price;
                existing.Status = dto.Status;
                existing.FoodCategoryId = dto.FoodCategoryId;
                existing.IsOutOfStock = dto.IsOutOfStock;

                // Quản lý ảnh
                var dbImage = await _unitOfWork.Images
                    .FirstOrDefaultAsync(i => i.FoodId == dto.FoodId); 

                var newImage = dto.Images;

                if (newImage != null)
                {
                    if (dbImage == null || dbImage.Id != newImage.Id)
                    {
                        if (dbImage != null)
                        {
                            var imageId = dbImage.Id;
                            await _unitOfWork.Images.DeleteAsync(imageId!);
                        }

                        var imageEntity = new Images
                        {
                            Id = newImage.Id,
                            Url = newImage.Url,
                            ThumbnailUrl = newImage.ThumbnailUrl,
                            Name = newImage.Name,
                            FoodId = dto.FoodId // Gán quan hệ ảnh → món ăn
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

                var updated = await _unitOfWork.Foods.UpdateAsync(existing);
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
            var result = await _unitOfWork.Foods.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }


    }
}
