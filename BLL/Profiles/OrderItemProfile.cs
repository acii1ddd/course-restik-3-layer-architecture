using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }
}
