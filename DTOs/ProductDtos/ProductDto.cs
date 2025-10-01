namespace CompanyApi.DTOs.ProductDtos
{
    public class ProductDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsTaxable { get; set; }
    }
}
