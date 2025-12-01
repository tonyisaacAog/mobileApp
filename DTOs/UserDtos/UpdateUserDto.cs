using System.ComponentModel.DataAnnotations;

namespace CompanyApi.DTOs.UserDtos
{
    public class UpdateUserDto
    {
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}
