using AutoMapper;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FoodOrder.Application.Services.Foods
{
    internal class FoodCategoryServices : IFoodCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SlugService _slugService;
        public FoodCategoryServices(IUnitOfWork unitOfWork, IMapper mapper, SlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slugService = slugService;
        }

        #region queries
        public async Task<IEnumerable<FoodCategoryDto>> GetAllAsync()
        {
            var foodCategories = await _unitOfWork.FoodCategories.GetAllAsync();
            return _mapper.Map<IEnumerable<FoodCategoryDto>>(foodCategories);
        }

        public async Task<FoodCategoryDto?> GetByIdAsync(int id)
        {
            var foodCategory = await _unitOfWork.FoodCategories.GetByIdAsync(id);
            return _mapper.Map<FoodCategoryDto>(foodCategory);
        }

        public async Task<FoodCategoryDto?> GetBySlugAsync(string slug)
        {
            var foodCategory = await _unitOfWork.FoodCategories.GetBySlugAsync(slug);
            return _mapper.Map<FoodCategoryDto>(foodCategory);
        }

  
        public async Task<IEnumerable<FoodCategoryListFoodDto?>> GetFoodCategoriesWithFoodsAsync()
        {
            var foodCategoriesWithFoods = await _unitOfWork.FoodCategories.GetFoodCategoriesWithFoods().ToListAsync();
            return _mapper.Map<IEnumerable<FoodCategoryListFoodDto>>(foodCategoriesWithFoods);
        }

        public async Task<FoodCategoryListFoodDto?> GetFoodsByCategorySlugAsync(string categorySlug)
        {
            var entity = await _unitOfWork.FoodCategories
                .GetFoodsByCategorySlug(categorySlug)
                .FirstOrDefaultAsync();

            if (entity == null)
                return null;

            var dto = _mapper.Map<FoodCategoryListFoodDto>(entity);
            return dto;
        }
        #endregion

        #region crud
        public async Task<bool> AddAsync(FoodCategoryDtoCreate foodCategoryDto)
        {
            // validate cơ bản
            if (string.IsNullOrWhiteSpace(foodCategoryDto.CategoryName))
                throw new ArgumentException("Tên danh mục không được để trống");

            if (foodCategoryDto.CategoryName.Length > 100)
                throw new ArgumentException("Tên danh mục tối đa 100 ký tự");

            if (!string.IsNullOrWhiteSpace(foodCategoryDto.Description) &&
                foodCategoryDto.Description.Length > 500)
                throw new ArgumentException("Mô tả tối đa 500 ký tự");

            if (foodCategoryDto.Images == null || string.IsNullOrEmpty(foodCategoryDto.Images.Id))
                throw new ArgumentException("Danh mục cần có ảnh");

            // validate business rule: không trùng tên
            var existCategory = await _unitOfWork.FoodCategories
            .FirstOrDefaultAsync(c => c.CategoryName == foodCategoryDto.CategoryName.Trim());
            if (existCategory != null)
                throw new InvalidOperationException("Tên danh mục đã tồn tại");

            if (foodCategoryDto.Images == null || string.IsNullOrEmpty(foodCategoryDto.Images.Id))
            {
                return false;
            }

            var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
            if (foodCategory.CategoryName != null)
            {
                foodCategory.Slug = await _slugService.GenerateUniqueSlug<FoodCategory>(foodCategory.CategoryName);
            }

            var result = await _unitOfWork.FoodCategories.AddAsync(foodCategory);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(FoodCategoryDtoUpdate dto)
        {
            try
            {
                var existing = await _unitOfWork.FoodCategories.GetByIdAsync(dto.FoodCategoryId);
                if (existing == null)
                    throw new ArgumentException("Category not found");

                if (string.IsNullOrWhiteSpace(dto.CategoryName))
                    throw new ArgumentException("Tên danh mục không được để trống");

                if (dto.CategoryName.Length > 100)
                    throw new ArgumentException("Tên danh mục tối đa 100 ký tự");

                if (!string.IsNullOrWhiteSpace(dto.Description) &&
                    dto.Description.Length > 500)
                    throw new ArgumentException("Mô tả tối đa 500 ký tự");

                if (dto.Images == null || string.IsNullOrEmpty(dto.Images.Id))
                    throw new ArgumentException("Danh mục cần có ảnh");

                var existCategory = await _unitOfWork.FoodCategories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName.Trim());
                if (existCategory != null && existCategory.FoodCategoryId != dto.FoodCategoryId)
                    throw new InvalidOperationException("Tên danh mục đã tồn tại");

                if (!string.IsNullOrWhiteSpace(dto.CategoryName))
                {
                    existing.CategoryName = dto.CategoryName;
                    existing.Slug = await _slugService.GenerateUniqueSlug<FoodCategory>(dto.CategoryName);
                }

                existing.Description = dto.Description;

                var dbImage = await _unitOfWork.Images
                    .FirstOrDefaultAsync(i => i.CategoryId == dto.FoodCategoryId);

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
                            CategoryId = dto.FoodCategoryId
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

                var updated = await _unitOfWork.FoodCategories.UpdateAsync(existing);
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
            var result = await _unitOfWork.FoodCategories.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }

        #endregion






    }
}
