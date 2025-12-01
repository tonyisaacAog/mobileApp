using AutoMapper;
using CompanyApi.DTOs.AuthDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Models;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                    return Unauthorized(new { message = "اسم المستخدم أو كلمة المرور غير صحيحة" });
                }

                var token = await _authService.GenerateJwtToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    User = userDto,
                    Message = "تم تسجيل الدخول بنجاح"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء تسجيل الدخول", error = ex.Message });
            }
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        //{
        //    try
        //    {
        //        registerDto.IsAdmin = false; // Regular users are not admins by default

        //        var createdUser = await _userService.CreateUserAsync(registerDto);
        //        var userDto = _mapper.Map<UserDto>(createdUser);

        //        return Ok(createdUser);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
        //    }
        //}

        [HttpPost("add-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAdmin([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                createUserDto.IsAdmin = true;

                var createdUser = await _userService.CreateUserAsync(createUserDto);
                var userDto = _mapper.Map<UserDto>(createdUser);

                return Ok(userDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء إنشاء مستخدم المدير", error = ex.Message });
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
                    return NotFound(new { message = "المستخدم غير موجود" });
                }

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء استرجاع المستخدم", error = ex.Message });
            }
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء استرجاع المستخدمين", error = ex.Message });
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
                    return Unauthorized(new { message = "المستخدم غير موجود" });
                }

                // Allow users to update their own profile or admins to update any profile
                if (id != currentUserId && !currentUser.Data.IsAdmin)
                {
                    return Forbid();
                }

                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

                if (updatedUser == null)
                {
                    return NotFound(new { message = "المستخدم غير موجود" });
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
                return StatusCode(500, new { message = "حدث خطأ أثناء تحديث المستخدم", error = ex.Message });
            }
        }

        [HttpDelete("user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result.Data)
                {
                    return NotFound(new { message = "المستخدم غير موجود" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء حذف المستخدم", error = ex.Message });
            }
        }

        [HttpPost("user/{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);

                if (!result.Data)
                {
                    return NotFound(new { message = "المستخدم غير موجود" });
                }

                return Ok(new { message = "تم تفعيل المستخدم بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء تفعيل المستخدم", error = ex.Message });
            }
        }

        [HttpPost("user/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);

                if (!result.Data)
                {
                    return NotFound(new { message = "المستخدم غير موجود" });
                }

                return Ok(new { message = "تم إلغاء تفعيل المستخدم بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء إلغاء تفعيل المستخدم", error = ex.Message });
            }
        }
    }
}
