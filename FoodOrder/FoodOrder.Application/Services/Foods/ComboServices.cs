using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
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

        public async Task<ComboWithFoodDto?> GetComboWithFoodsBySlugAsync(string slug)
        {
            var comboWithFoods = await _unitOfWork.Combos
                .GetComboWithFoodsBySlug(slug)
                .Select(cb => new ComboWithFoodDto
                {
                    ComboId = cb.ComboId,
                    ComboName = cb.ComboName,
                    Slug = cb.Slug,
                    FoodCategoryId = cb.FoodCategoryId,
                    Price = cb.Price,
                    Description = cb.Description,
                    //Image = cb.Image,
                    Status = cb.Status,
                    ComboDetails = cb.ComboDetails != null
                        ? cb.ComboDetails.Select(cd => new ComboDetailDto
                        {
                            ComboId = cd.ComboId,
                            FoodId = cd.FoodId,
                            Quantity = cd.Quantity,
                            Food = cd.Food != null ? new FoodDto
                            {
                                FoodId = cd.Food.FoodId,
                                FoodName = cd.Food.FoodName,
                                Slug = cd.Food.Slug,
                                Description = cd.Food.Description,
                                Price = cd.Food.Price,
                                //Image = cd.Food.Image,
                                Status = cd.Food.Status
                            } : new FoodDto()
                        }).ToList()
                        : new List<ComboDetailDto>()
                })
                .FirstOrDefaultAsync();

            return comboWithFoods;
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
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }
    }
}
