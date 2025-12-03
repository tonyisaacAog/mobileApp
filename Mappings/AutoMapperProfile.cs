using AutoMapper;
using CompanyApi.DTOs.AuthDtos;
using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.CompanyDtos;
using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ProductDtos;
using CompanyApi.DTOs.TemporaryDocumentDto;
using CompanyApi.DTOs.UserDtos;
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
            CreateMap<UpdateUserDto, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, UserDto>().ReverseMap();

            // Add mappings for other entities as needed
            CreateMap<Branch, BranchDto>().ReverseMap();
            CreateMap<CreateBranchDto, Branch>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<Document,DocumentDetailsDto>()
                .ForMember(d => d.ReceiptItems,
                           opt => opt.MapFrom(s => s.ReceiptItems));
            CreateMap<DocumentLines,DocumentLinesDto>();

            CreateMap<Document, DocumentDetailsDto>();
            CreateMap<DocumentLines, DocumentLinesDto>();

            CreateMap<TemporaryDocument, TemporaryDocumentDto>().ReverseMap();
            CreateMap<TemporaryDocumentLine, TemporaryDocumentLineDto>().ReverseMap();

        }
    }
}
