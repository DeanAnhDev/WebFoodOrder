using AutoMapper;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.DTOs.Identity.Location;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Identity
{
    internal class LocationServices : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LocationServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<bool> AddAsync(CreateLocationDto dto)
        {

            var location = _mapper.Map<Location>(dto);
            var result = await _unitOfWork.Locations.AddAsync(location);
            if (!result) return false;
            return await _unitOfWork.CompleteAsync() > 0;
        }


        public async Task<bool> UpdateAsync(UpdateLocationDto dto)
        {
            try
            {
                var existing = await _unitOfWork.Locations.GetByIdAsync(dto.Id);
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy địa chỉ nào");

                var location = _mapper.Map(dto, existing);
                var updated = await _unitOfWork.Locations.UpdateAsync(location);
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

            var result = await _unitOfWork.Locations.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }

        public async Task<List<LocationDto>> GetAllByIdUserAsync(int idUser)
        {
            var locations = await _unitOfWork.Locations.GetAllByIdUserAsync(idUser);
            return _mapper.Map<List<LocationDto>>(locations);
        }


        public async Task<LocationDto> GetByIdAsync(int id)
        {
            var location = await _unitOfWork.Locations.GetByIdAsync(id);
            return _mapper.Map<LocationDto>(location);
        }
    }
}
