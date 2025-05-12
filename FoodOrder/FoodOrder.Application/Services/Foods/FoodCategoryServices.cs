using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
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

        public async Task<bool> AddAsync(FoodCategoryDto foodCategoryDto)
        {
            var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
            if (foodCategory.CategoryName != null)
            {
                foodCategory.Slug = await _slugService.GenerateUniqueSlug<FoodCategory>(foodCategory.CategoryName);
            }
            var result = await _unitOfWork.FoodCategories.AddAsync(foodCategory);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(FoodCategoryDto foodCategoryDto)
        {
            var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
            if (foodCategory.CategoryName != null)
            {
                foodCategory.Slug = await _slugService.GenerateUniqueSlug<FoodCategory>(foodCategory.CategoryName);
            }
            var result = await _unitOfWork.FoodCategories.UpdateAsync(foodCategory);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.FoodCategories.DeleteAsync(id);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<FoodCategoryListFoodDto?>> GetFoodCategoriesWithFoodsAsync()
        {
            var foodCategoriesWithFoods = await _unitOfWork.FoodCategories.GetFoodCategoriesWithFoods()
                .Select(fc => new FoodCategoryListFoodDto
                {
                    FoodCategoryId = fc.FoodCategoryId,
                    CategoryName = fc.CategoryName,
                    Slug = fc.Slug,
                    Foods = fc.Foods != null ? fc.Foods
                    .Select(f => new FoodDto
                    {
                        FoodId = f.FoodId,
                        FoodName = f.FoodName,
                        Slug = f.Slug,
                        Description = f.Description,
                        Price = f.Price,
                        Image = f.Image,
                        Status = f.Status
                    }).ToList() : new List<FoodDto>(),
                    Combos = fc.Combos != null ? fc.Combos
                    .Select(c => new ComboDto
                    {
                        ComboId = c.ComboId,
                        ComboName = c.ComboName,
                        Slug = c.Slug,
                        Price = c.Price,
                        Image = c.Image,
                        Status = c.Status
                    }).ToList() : new List<ComboDto>()
                }).ToListAsync();
            return foodCategoriesWithFoods;
        }

        public async Task<FoodCategoryListFoodDto?> GetFoodsByCategorySlugAsync(string categorySlug)
        {
            var foodsByCategorySlug = await _unitOfWork.FoodCategories
                .GetFoodsByCategorySlug(categorySlug)
                .Select(fc => new FoodCategoryListFoodDto
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
                        Image = f.Image,
                        Status = f.Status
                    }).ToList() : new List<FoodDto>()
                })
                .FirstOrDefaultAsync();

            return foodsByCategorySlug;
        }


    }
}
