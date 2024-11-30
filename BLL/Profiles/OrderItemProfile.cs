using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dto => dto.Dish, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
