namespace CompanyApi.DTOs
{
    public class CompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public bool IsActive { get; set; }
    }
}
