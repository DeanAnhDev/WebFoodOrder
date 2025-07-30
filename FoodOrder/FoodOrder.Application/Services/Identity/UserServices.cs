using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Identity;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Interfaces;

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

    }
}
