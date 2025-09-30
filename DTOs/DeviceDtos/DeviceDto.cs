namespace CompanyApi.DTOs.DeviceDtos
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string OS { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }
}
