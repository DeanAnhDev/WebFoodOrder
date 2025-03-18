using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Foods
{
    internal class FoodServices : IFoodServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FoodServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FoodDto>> GetAllAsync()
        {
            var foods = await _unitOfWork.Foods.GetAllAsync();
            return _mapper.Map<IEnumerable<FoodDto>>(foods);
        }

        public async Task<FoodDto?> GetByIdAsync(int id)
        {
            var foods = await _unitOfWork.Foods.GetByIdAsync(id);
            return _mapper.Map<FoodDto>(foods);
        }

        public async Task<bool> AddAsync(FoodDto foodDto)
        {
            var food = _mapper.Map<Food>(foodDto);
            var result = await _unitOfWork.Foods.AddAsync(food);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(FoodDto foodDto)
        {
            var food = _mapper.Map<Food>(foodDto);
            var result = await _unitOfWork.Foods.UpdateAsync(food);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Foods.DeleteAsync(id);
            if (result)
            {
                return true;
            }
            return false;
        }


    }
}
