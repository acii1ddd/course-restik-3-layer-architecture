using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL.Profiles
{
    public class WorkerProfile : Profile
    {
        public WorkerProfile()
        {
            CreateMap<Worker, WorkerDTO>().ReverseMap();
        }
    }
}
