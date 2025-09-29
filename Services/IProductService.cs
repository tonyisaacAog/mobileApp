using CompanyApi.DTOs;

namespace CompanyApi.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(ProductDto dto);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, ProductDto dto);
        Task DeleteProductAsync(int id);
        Task<bool> IsProductCodeUniqueAsync(string sku);
    }
}
