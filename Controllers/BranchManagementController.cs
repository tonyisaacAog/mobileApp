using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class BranchManagementController : Controller
    {
        private readonly IBranchService _branchService;

        public BranchManagementController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        // GET: BranchManagement
        public async Task<IActionResult> Index()
        {
            var result = await _branchService.GetAllBranchesAsync();
            return View(result.Data ?? new List<BranchDto>());
        }

        // GET: BranchManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var branchResult = await _branchService.GetBranchByIdAsync(id);

            if (branchResult == null || branchResult.Data == null)
                return NotFound();

            return View(branchResult.Data);
        }

        // GET: BranchManagement/Create
        public IActionResult Create()
        {
            return View(new BranchDto());
        }

        // POST: BranchManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchDto branchDto)
        {
            if (ModelState.IsValid)
            {
                await _branchService.CreateBranchAsync(branchDto);
                TempData["SuccessMessage"] = "Branch created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(branchDto);
        }

        // GET: BranchManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var branchResult = await _branchService.GetBranchByIdAsync(id);

            if (branchResult == null || branchResult.Data == null)
                return NotFound();

            return View(branchResult.Data);
        }

        // POST: BranchManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BranchDto branchDto)
        {
            if (id != branchDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _branchService.UpdateBranchAsync(id, branchDto);
                TempData["SuccessMessage"] = "Branch updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(branchDto);
        }

        // GET: BranchManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var branchResult = await _branchService.GetBranchByIdAsync(id);

            if (branchResult == null || branchResult.Data == null)
                return NotFound();

            return View(branchResult.Data);
        }

        // POST: BranchManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _branchService.DeleteBranchAsync(id);
            TempData["SuccessMessage"] = "Branch deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
