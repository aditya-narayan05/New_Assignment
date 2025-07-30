using AutoMapper;
using backendNew.Dtos;
using backendNew.Model;
using backendNew.Repository;

namespace backendNew.AutoMapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, LoginDTO>().ReverseMap();
            CreateMap<UserRepo, UserDto>().ReverseMap();
        }
    }
}
