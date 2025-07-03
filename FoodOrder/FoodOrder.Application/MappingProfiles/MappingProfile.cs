using AutoMapper;
using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
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
            CreateMap<FoodCategory, FoodCategoryListFoodDto>().ReverseMap();

            //Mapping from Food to FoodDto
            CreateMap<Food, FoodDto>().ReverseMap();
            CreateMap<Food, FoodDtoCreate>().ReverseMap();
            CreateMap<Food, FoodDtoUpdate>().ReverseMap();

            //Mapping from ComBo to ComboDto
            CreateMap<Combo, ComboDto>().ReverseMap();
            CreateMap<Combo, ComboDtoCreate>().ReverseMap();
            CreateMap<Combo, ComboDtoUpdate>().ReverseMap();
            CreateMap<Combo, ComboWithFoodDto>().ReverseMap();

            //Mapping from ComboDetail to ComboDetailDto
            CreateMap<ComboDetail, ComboDetailDto>().ReverseMap();

            //Mapping from ImageDto to Image
            CreateMap<ImageDto, Images>().ReverseMap();
        }
    }
}
