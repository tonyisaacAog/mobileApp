using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using CompanyApi.DTOs;
using CompanyApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CompanyApi.Models;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IUserService userService, IMapper mapper)
        {
            _authService = authService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _authService.AuthenticateUser(loginDto.Username, loginDto.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = await _authService.GenerateJwtToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    User = userDto,
                    Message = "Login successful"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = _mapper.Map<User>(registerDto);
                user.IsAdmin = false; // Regular users are not admins by default

                var createdUser = await _userService.CreateUserAsync(user);
                var userDto = _mapper.Map<UserDto>(createdUser);

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("add-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var user = _mapper.Map<User>(createUserDto);
                user.IsAdmin = true;

                var createdUser = await _userService.CreateUserAsync(user);
                var userDto = _mapper.Map<UserDto>(createdUser);

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating admin user", error = ex.Message });
            }
        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }

        [HttpPut("user/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                // Check if user is updating their own profile or if they're an admin
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _userService.GetUserByIdAsync(currentUserId);

                if (currentUser == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                // Allow users to update their own profile or admins to update any profile
                if (id != currentUserId && !currentUser.IsAdmin)
                {
                    return Forbid();
                }

                var user = _mapper.Map<User>(updateUserDto);
                var updatedUser = await _userService.UpdateUserAsync(id, user);

                if (updatedUser == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = _mapper.Map<UserDto>(updatedUser);
                return Ok(userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user", error = ex.Message });
            }
        }

        [HttpDelete("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting user", error = ex.Message });
            }
        }

        [HttpPost("user/{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating user", error = ex.Message });
            }
        }

        [HttpPost("user/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);

                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating user", error = ex.Message });
            }
        }
    }
}
