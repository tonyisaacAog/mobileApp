using CompanyApi.DTOs.BranchDtos;

namespace CompanyApi.DTOs.UserDtos
{
    public class AssignBranchesToUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;

        public List<BranchSelectionDto> Branches { get; set; } = new();
    }
}
