using CompanyApi.DTOs;
using CompanyApi.Repositories;

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
            if( !isUnique )
            {
                throw new InvalidOperationException("Product code must be unique.");
            }
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
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Select(b => new ProductDto
            {
                Name = b.Name,
                SKU = b.SKU,
                Description = b.Description,
                Price = b.Price,
                Category = b.Category,
                Cost = b.Cost,
                CreatedAt = b.CreatedAt
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if( product == null ) return null;
            return new ProductDto
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Cost = product.Cost,
                CreatedAt = product.CreatedAt
            };
        }

        public async Task UpdateProductAsync(int id,ProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if( product == null ) throw new KeyNotFoundException("Product not found.");
            var isUnique = await IsProductCodeUniqueAsync(dto.SKU);
            if( !isUnique && product.SKU != dto.SKU )
            {
                throw new InvalidOperationException("Product code must be unique.");
            }
            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Cost = dto.Cost;
            product.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if( product == null ) throw new KeyNotFoundException("Product not found.");
            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsProductCodeUniqueAsync(string productCode)
        {
            var existingProduct = await _unitOfWork.Products
                .FirstOrDefaultAsync(b => b.SKU == productCode);
            return existingProduct == null;
        }
    }
}
