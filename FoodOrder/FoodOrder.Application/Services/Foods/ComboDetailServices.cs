using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Application.Services.Foods
{
    internal class ComboDetailServices : IComboDetailServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ComboDetailServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<bool> AddAsync(ComboDetailDto comboDetailDto)
        {
            var comboDetail = _mapper.Map<ComboDetail>(comboDetailDto);
            var result = await _unitOfWork.ComboDetails.AddAsync(comboDetail);
            if (!result) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateAsync(ComboDetailDto comboDetailDto)
        {
            var comboDetail = _mapper.Map<ComboDetail>(comboDetailDto);
            var isUpdated = await _unitOfWork.ComboDetails.UpdateAsync(comboDetail);
            if (!isUpdated) return false;

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int comboId, int foodId)
        {
            var result = await _unitOfWork.ComboDetails.DeleteAsync(comboId, foodId);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }
     
        public async Task<ComboDetailDto> GetByIdAsync(int comboId, int foodId)
        {
            var comboDetail = await _unitOfWork.ComboDetails.GetByIdAsync(comboId, foodId);
            return _mapper.Map<ComboDetailDto>(comboDetail);

        }

        public async Task<IEnumerable<ComboDetailDto>> GetComboDetailsByComboIdAsync(int comboId)
        {
            var listComboDetails = await _unitOfWork.ComboDetails.GetComboDetailsByComboIdAsync(comboId);
            if (listComboDetails == null || !listComboDetails.Any())
            {
                return new List<ComboDetailDto>();
            }
            return _mapper.Map<IEnumerable<ComboDetailDto>>(listComboDetails);
        }
    }
}
