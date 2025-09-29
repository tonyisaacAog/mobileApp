using CompanyApi.DTOs;

namespace CompanyApi.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateProductAsync(ProductDto dto);
        Task<PagedResult<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParams);
        Task<Result<ProductDto>?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, ProductDto dto);
        Task DeleteProductAsync(int id);
    }
}
