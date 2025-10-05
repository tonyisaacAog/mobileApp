using AutoMapper;
using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly IUserBranchService _userBranchService;
        private readonly IMapper _mapper;
        public UserManagementController(IUserService userService,IBranchService branchService,IMapper mapper,IUserBranchService userBranchService)
        {
            _userService = userService;
            _branchService = branchService;
            _mapper = mapper;
            _userBranchService = userBranchService;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllUsersAsync();
            return View(result.Data ?? new List<UserDto>());
        }

        // GET: UserManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if( userResult == null || userResult.Data == null )
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
            if( ModelState.IsValid )
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

            if( userResult == null || userResult.Data == null )
                return NotFound();

            var updateUser = _mapper.Map<UpdateUserDto>(userResult.Data);

            return View(updateUser);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,UpdateUserDto userDto)
        {
            //    if (id != userDto.Id)
            //        return NotFound();

            if( ModelState.IsValid )
            {
                await _userService.UpdateUserAsync(id,userDto);
                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(userDto);
        }

        // GET: UserManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);

            if( userResult == null || userResult.Data == null )
                return NotFound();

            return View(userResult.Data);
        }

        // POST: UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["SuccessMessage"] = "تم حذف المستخدم بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/ManageUserBranches/5
        public async Task<IActionResult> ManageUserBranches(int id)
        {
            var userResult = await _userService.GetUserByIdAsync(id);
            if( userResult == null || userResult.Data == null )
                return NotFound();

            var branchesResult = await _branchService.GetAllBranchesAsync();

            ViewBag.User = userResult.Data;
            ViewBag.AllBranches = branchesResult?.Data ?? Enumerable.Empty<BranchDto>();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AssignBranches(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if( user == null )
                return NotFound();

            var branches = await _branchService.GetAllBranchesAsync();
            if( branches?.Data == null )
                return NotFound();

            ViewBag.User = user.Data;
            ViewBag.AllBranches = branches.Data;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignBranches(int userId,int[] SelectedBranches)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if( user == null || user.Data == null )
                return NotFound();

            if( user.Data.UserBranches.Any() )
                await _userBranchService.RemoveBranchFromUserAsync(user.Data.Id);

            if( SelectedBranches != null && SelectedBranches.Length > 0 )
            {
                await _userBranchService.AddBranchToUserAsync(user.Data.Id,SelectedBranches);
            }

            TempData["SuccessMessage"] = "تم تحديث الفروع الخاصة بالمستخدم بنجاح.";
            return RedirectToAction("Details",new { id = user.Data.Id });
        }

    }
}
