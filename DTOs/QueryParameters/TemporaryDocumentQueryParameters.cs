using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.DTOs.QueryParameters
{
    public class TemporaryDocumentQueryParameters: PaginationParameters
    {
        public int GroupId { get; set; }
    }
}
