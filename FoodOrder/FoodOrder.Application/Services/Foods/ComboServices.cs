using AutoMapper;
using FoodOrder.Application.ApplicationService;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

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
        public async Task<ComboDto?> GetBySlugAsync(string slug)
        {
            var combo = await _unitOfWork.Combos.GetBySlugAsync(slug);
            return _mapper.Map<ComboDto>(combo);
        }

        public async Task<bool> AddAsync(ComboDto comboDto)
        {
            var combo = _mapper.Map<Combo>(comboDto);
            if (!string.IsNullOrEmpty(combo.ComboName))
            {
                combo.Slug = await _slugService.GenerateUniqueSlug<Combo>(combo.ComboName);
            }
            var result = await _unitOfWork.Combos.AddAsync(combo);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ComboDto comboDto)
        {
            var combo = _mapper.Map<Combo>(comboDto);

            if (!string.IsNullOrEmpty(combo.ComboName))
            {
                combo.Slug = await _slugService.GenerateUniqueSlug<Combo>(combo.ComboName);
            }

            var isUpdated = await _unitOfWork.Combos.UpdateAsync(combo);
            if (!isUpdated) return false;

            return await _unitOfWork.CompleteAsync() > 0;
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
