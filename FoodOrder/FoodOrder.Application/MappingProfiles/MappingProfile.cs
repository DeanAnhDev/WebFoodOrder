using AutoMapper;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Mapping from FoodCategory to FoodCategoryDto
            CreateMap<FoodCategory, FoodCategoryDto>().ReverseMap();


        }
    }
}
