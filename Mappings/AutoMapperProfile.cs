using AutoMapper;
using CompanyApi.DTOs;
using CompanyApi.Models;

namespace CompanyApi.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
            CreateMap<RegisterDto, User>().ReverseMap();

            // Add mappings for other entities as needed
            CreateMap<Branch, BranchDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
        }
    }
}
