namespace CompanyApi.DTOs.BranchDtos
{
    public class BranchSelectionDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
