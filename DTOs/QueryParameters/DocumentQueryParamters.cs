
using CompanyApi.DTOs.ResponseDtos;
namespace CompanyApi.DTOs.QueryParameters
{
    public class DocumentQueryParamters : PaginationParameters
    {
        public DateTime DateFrom{ get; set; }
        public DateTime DateTo{ get; set; }
        public string DeviceCode { get; set; }
        public int? UserId{ get; set; }

    }
}
