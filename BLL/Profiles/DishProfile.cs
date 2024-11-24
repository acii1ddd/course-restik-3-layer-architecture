using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class DishProfile : Profile
    {
        public DishProfile()
        {
            CreateMap<Dish, DishDTO>().ReverseMap();
        }
    }
}
