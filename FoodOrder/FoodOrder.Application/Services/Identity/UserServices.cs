using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Identity;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Identity
{
    internal class UserServices : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var foods = await _unitOfWork.AppUsers.GetByIdAsync(id);
            return _mapper.Map<UserDto>(foods);
        }

        public async Task<bool> UpdateAsync(int id, string newName)
        {
            try
            {
                var existing = await _unitOfWork.AppUsers.GetByIdAsync(id);
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy");
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    existing.FullName = newName;
                }

                var updated = await _unitOfWork.AppUsers.UpdateAsync(existing);
                if (!updated) return false;

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"EF Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }

    }
}
