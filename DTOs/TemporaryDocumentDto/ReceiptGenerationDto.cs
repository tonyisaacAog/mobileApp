namespace CompanyApi.DTOs.TemporaryDocumentDto
{
    public class ReceiptGenerationDto
    {
        public string CustomerName { get; set; }
        public string DeviceSerial { get; set; }
        public List<ProductReceiptDto> ProductReceiptDtos { get; set; }
        public int NumberOfReceipts { get; set; }
    }
}
