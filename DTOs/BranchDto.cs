using System.ComponentModel.DataAnnotations;

namespace CompanyApi.DTOs
{
    // Additional DTOs for other entities
    public class BranchDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Governate { get; set; } = string.Empty;
        public string RegionCity { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public int InitialInvoiceTaxSerial { get; set; }
        public int InitialCreditTaxSerial { get; set; }
        public int InitialDebitTaxSerial { get; set; }
    }
}
