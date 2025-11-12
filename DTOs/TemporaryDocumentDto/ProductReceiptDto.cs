namespace CompanyApi.DTOs.TemporaryDocumentDto
{
    public class ProductReceiptDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal minRange { get; set; }
        public decimal maxRange { get; set; }

    }
}
