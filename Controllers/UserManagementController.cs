using CompanyApi.DTOs;
using CompanyApi.Models;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllUsersAsync(new PaginationParameters());
            return View(result.Data ?? new List<UserDto>());
        }

        // GET: UserManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if (userResult == null || userResult.Data == null)
                return NotFound();

            return View(userResult.Data);
        }

        // GET: UserManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PhoneNumber = userDto.PhoneNumber,
                    IsActive = userDto.IsActive,
                    IsAdmin = userDto.IsAdmin
                };

                await _userService.CreateUserAsync(user);
                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(userDto);
        }

        // GET: UserManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if (userResult == null || userResult.Data == null)
                return NotFound();

            return View(userResult.Data);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserDto userDto)
        {
            if (id != userDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PhoneNumber = userDto.PhoneNumber,
                    IsActive = userDto.IsActive,
                    IsAdmin = userDto.IsAdmin
                };

                await _userService.UpdateUserAsync(id, user);
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(userDto);
        }

        // GET: UserManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if (userResult == null || userResult.Data == null)
                return NotFound();

            return View(userResult.Data);
        }

        // POST: UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
