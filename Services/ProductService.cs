using CompanyApi.DTOs;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateProductAsync(ProductDto dto)
        {
            var isUnique = await IsProductCodeUniqueAsync(dto.SKU);
            if (!isUnique)
                throw new InvalidOperationException("Product code must be unique.");

            var product = new Models.Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                Cost = dto.Cost,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<Models.Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParams)
        {
            var selectors = MappingUtilities.CreateMapExpression<Models.Product, ProductDto>();
            var products = await _unitOfWork.Repository<Models.Product>()
                .GetProjectedPaginatedAsync<ProductDto>(selectors, paginationParams);

            return await PagedResult<ProductDto>.SuccessAsync(
                products.Items, products.TotalCount, paginationParams.PageNumber, paginationParams.PageSize
            );
        }

        public async Task<Result<ProductDto>?> GetProductByIdAsync(int id)
        {
            var selectors = MappingUtilities.CreateMapExpression<Models.Product, ProductDto>();
            var product = await _unitOfWork.Repository<Models.Product>()
                .GetByIdAsync(obj => obj.Id == id, selectors);

            if (product == null) return null;
            return await Result<ProductDto>.SuccessAsync(product);
        }

        public async Task UpdateProductAsync(int id, ProductDto dto)
        {
            var repo = _unitOfWork.Repository<Models.Product>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            var isUnique = await IsProductCodeUniqueAsync(dto.SKU);
            if (!isUnique && product.SKU != dto.SKU)
                throw new InvalidOperationException("Product code must be unique.");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Cost = dto.Cost;
            product.UpdatedAt = DateTime.UtcNow;

            repo.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var repo = _unitOfWork.Repository<Models.Product>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            repo.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsProductCodeUniqueAsync(string productCode)
        {
            var existingProduct = await _unitOfWork.Repository<Models.Product>()
                .FirstOrDefaultAsync(b => b.SKU == productCode);

            return existingProduct == null;
        }
    }

}