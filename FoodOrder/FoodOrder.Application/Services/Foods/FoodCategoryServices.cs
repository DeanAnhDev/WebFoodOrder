using AutoMapper;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Foods
{
    internal class FoodCategoryServices : IFoodCategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FoodCategoryServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<bool> AddAsync(FoodCategoryDto foodCategoryDto)
        {
            var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
            var result =  await _unitOfWork.FoodCategories.AddAsync(foodCategory);
            if(result)
            {
                return true;
            }
            return false;   
        }

        public async Task<bool> UpdateAsync(FoodCategoryDto foodCategoryDto)
        {
            var foodCategory = _mapper.Map<FoodCategory>(foodCategoryDto);
            var result = await _unitOfWork.FoodCategories.UpdateAsync(foodCategory);
            if (result)
            {
                return true;
            }
            return false;
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

    }
}
