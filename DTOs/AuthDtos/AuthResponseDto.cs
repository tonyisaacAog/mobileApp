using CompanyApi.DTOs.UserDtos;

namespace CompanyApi.DTOs.AuthDtos
{

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new UserDto();
        public string Message { get; set; } = string.Empty;
    }
}
