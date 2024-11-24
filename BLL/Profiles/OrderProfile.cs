using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }
}
