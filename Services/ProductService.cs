using AutoMapper;
using CompanyApi.DTOs.ProductDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateProductAsync(ProductDto dto)
        {
            var isUnique = await IsProductCodeUniqueAsync(dto.SKU);
            if (!isUnique)
                throw new InvalidOperationException("كود المنتج يجب أن يكون فريداً.");

            var product = new Models.Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                Cost = dto.Cost,
                IsActive = dto.IsActive,
                IsTaxable = dto.IsTaxable,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Repository<Models.Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(PaginationParameters paginationParams)
        {
            var products = await _unitOfWork.Repository<Models.Product>()
                .GetPaginatedAsync(paginationParams);
                var list = _mapper.Map<List<ProductDto>>(products.Items);
            return await PagedResult<ProductDto>.SuccessAsync(
               list, products.TotalCount, paginationParams.PageNumber, paginationParams.PageSize
            );
        }

        public async Task<Result<ProductDto>?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Repository<Models.Product>()
                .GetByIdAsync(id);
            var dto = _mapper.Map<ProductDto>(product);
            if (product == null) return null;
            return await Result<ProductDto>.SuccessAsync(dto);
        }

        public async Task UpdateProductAsync(int id, ProductDto dto)
        {
            var repo = _unitOfWork.Repository<Models.Product>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("المنتج غير موجود.");

            var isUnique = await IsProductCodeUniqueAsync(dto.SKU);
            if (!isUnique && product.SKU != dto.SKU)
                throw new InvalidOperationException("كود المنتج يجب أن يكون فريداً.");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Cost = dto.Cost;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsTaxable = dto.IsTaxable;
            product.IsActive = dto.IsActive;

            repo.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var repo = _unitOfWork.Repository<Models.Product>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("المنتج غير موجود.");

            repo.Remove(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsProductCodeUniqueAsync(string productCode)
        {
            var existingProduct = await _unitOfWork.Repository<Models.Product>()
                .FirstOrDefaultAsync(b => b.SKU == productCode);

            return existingProduct == null;
        }

        public async Task<Result<int>> GetCountProducts()
        {
            var count = await _unitOfWork.Repository<Product>().CountAsync();
            return await Result<int>.SuccessAsync(count);
        }
    }

}