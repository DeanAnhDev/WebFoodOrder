using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.DTOs.Foods.Image;
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

        #region crud
        public async Task<bool> AddAsync(FoodCategoryDtoCreate foodCategoryDto)
        {

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

        public async Task<IEnumerable<FoodCategoryListFoodDto?>> GetFoodCategoriesWithFoodsAsync()
        {
            var foodCategoriesWithFoods = await _unitOfWork.FoodCategories.GetFoodCategoriesWithFoods().ToListAsync();
            return _mapper.Map<IEnumerable<FoodCategoryListFoodDto>>(foodCategoriesWithFoods);
        }

        public async Task<FoodsByCategory?> GetFoodsByCategorySlugAsync(string categorySlug)
        {
            var foodsByCategorySlug = await _unitOfWork.FoodCategories
                .GetFoodsByCategorySlug(categorySlug)
                .Select(fc => new FoodsByCategory
                {
                    FoodCategoryId = fc.FoodCategoryId,
                    CategoryName = fc.CategoryName,
                    Slug = fc.Slug,
                    Foods = fc.Foods != null ? fc.Foods.Select(f => new FoodDto
                    {
                        FoodId = f.FoodId,
                        FoodName = f.FoodName,
                        Slug = f.Slug,
                        Description = f.Description,
                        Price = f.Price,
                        IsOutOfStock = f.IsOutOfStock,
                        Status = f.Status,
                        Images = f.Images != null ? new ImageDto
                        {
                            Id = f.Images.Id,
                            Url = f.Images.Url,
                            ThumbnailUrl = f.Images.ThumbnailUrl,
                            Name = f.Images.Name
                        } : null
                    }).ToList() : new List<FoodDto>()
                })
                .FirstOrDefaultAsync();

            return foodsByCategorySlug;
        }


        public async Task<CombosByCategory?> GetCombosByCategorySlugAsync(string categorySlug)
        {
            var combosByCategorySlug = await _unitOfWork.FoodCategories
                .GetCombosByCategorySlug(categorySlug)
                .Select(fc => new CombosByCategory
                {
                    FoodCategoryId = fc.FoodCategoryId,
                    CategoryName = fc.CategoryName,
                    Slug = fc.Slug,
                    Combos = fc.Combos != null ? fc.Combos.Select(f => new ComboDto
                    {
                        ComboId = f.ComboId,
                        ComboName = f.ComboName,
                        Slug = f.Slug,
                        Description = f.Description,
                        Price = f.Price,
                        Status = f.Status
                    }).ToList() : new List<ComboDto>()
                })
                .FirstOrDefaultAsync();

            return combosByCategorySlug;
        }


    }
}
