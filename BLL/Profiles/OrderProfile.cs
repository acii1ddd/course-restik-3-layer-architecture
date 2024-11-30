using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>()
                .ForMember(or => or.Items, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
