using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.Image;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Image;

namespace FoodOrder.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Mapping from FoodCategory to FoodCategoryDto
            CreateMap<FoodCategory, FoodCategoryDto>().ReverseMap();
            CreateMap<FoodCategory, FoodCategoryDtoUpdate>().ReverseMap();
            CreateMap<FoodCategory, FoodCategoryDtoCreate>().ReverseMap();

            //Mapping from Food to FoodDto
            CreateMap<Food, FoodDto>().ReverseMap();
            CreateMap<Food, FoodDtoCreate>().ReverseMap();
            CreateMap<Food, FoodDtoUpdate>().ReverseMap();

            //Mapping from ComBo to ComboDto
            CreateMap<Combo, ComboDto>().ReverseMap();

            //Mapping from ComboDetail to ComboDetailDto
            CreateMap<ComboDetail, ComboDetailDto>().ReverseMap();

            //Mapping from ImageDto to Image
            CreateMap<ImageDto, Images>().ReverseMap();
        }
    }
}
