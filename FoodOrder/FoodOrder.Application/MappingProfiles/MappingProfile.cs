using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Food;
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

            //Mapping from Food to FoodDto
            CreateMap<Food, FoodDto>().ReverseMap();

            //Mapping from ComBo to ComboDto
            CreateMap<Combo, ComboDto>().ReverseMap();
        }
    }
}
