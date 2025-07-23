using AutoMapper;
using backendNew.Dtos;
using backendNew.Model;

namespace backendNew.AutoMapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, LoginDTO>();
        }
    }
}
