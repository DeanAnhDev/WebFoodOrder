using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Foods
{
    internal class ComboServices : IComboServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ComboServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ComboDto>> GetAllAsync()
        {
            var combos = await _unitOfWork.Combos.GetAllAsync();
            return _mapper.Map<IEnumerable<ComboDto>>(combos);
        }

        public async Task<ComboDto?> GetByIdAsync(int id)
        {
            var combo = await _unitOfWork.Combos.GetByIdAsync(id);
            return _mapper.Map<ComboDto>(combo);
        }

        public async Task<bool> AddAsync(ComboDto comboDto)
        {
            var combo = _mapper.Map<Combo>(comboDto);
            var result = await _unitOfWork.Combos.AddAsync(combo);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(ComboDto comboDto)
        {
            var combo = _mapper.Map<Combo>(comboDto);
            var result = await _unitOfWork.Combos.UpdateAsync(combo);
            if (result)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Combos.DeleteAsync(id);
            if (result)
            {
                return true;
            }
            return false;
        }
    }
}
