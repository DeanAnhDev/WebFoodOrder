using AutoMapper;
using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.DTOs.Foods.Image;
using FoodOrder.Application.DTOs.Identity;
using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Entities.Orders;

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
            CreateMap<Combo, ComboDtoById>().ReverseMap();

            //Mapping from ComboDetail to ComboDetailDto
            CreateMap<ComboDetail, ComboDetailDto>().ReverseMap();

            //Mapping from ImageDto to Image
            CreateMap<ImageDto, Images>().ReverseMap();

            //Mapping from Cart to CartDto
            CreateMap<Cart, CartCreateDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();

            //Mapping from CartItem to CartItemDto
            CreateMap<CartItem, CartItemDto>().ReverseMap();

            //Mapping from AppUser to UserDto
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<AppUser, UpdateUserDto>().ReverseMap();
        }
    }
}
