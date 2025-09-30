using AutoMapper;
using CompanyApi.DTOs;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
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
        private readonly IMapper _mapper;
        public UserManagementController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index(PaginationParameters paginationParameters)
        {
            var result = await _userService.GetAllUsersAsync(paginationParameters);
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
            return View(new CreateUserDto());
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                await _userService.CreateUserAsync(userDto);
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

            var updateUser = _mapper.Map<UpdateUserDto>(userResult.Data);

            return View(updateUser);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateUserDto userDto)
        { 
        //    if (id != userDto.Id)
        //        return NotFound();

            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(id, userDto);
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
