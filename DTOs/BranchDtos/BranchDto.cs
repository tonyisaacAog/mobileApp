namespace CompanyApi.DTOs.BranchDtos
{
    // Additional DTOs for other entities
    public class BranchDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Governate { get; set; } = string.Empty;
        public string RegionCity { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }

    }
}
