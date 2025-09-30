using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class CompanyManagementController : Controller
    {
        private readonly ICompanyService _companyService;

        public CompanyManagementController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET: CompanyManagement
        public async Task<IActionResult> Index()
        {
            var result = await _companyService.GetAllCompaniesAsync();
            return View(result.Data ?? new List<CompanyDto>());
        }

        // GET: CompanyManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var companyResult = await _companyService.GetCompanyByIdAsync(id);

            if (companyResult == null || companyResult.Data == null)
                return NotFound();

            return View(companyResult.Data);
        }

        // GET: CompanyManagement/Create
        public IActionResult Create()
        {
            return View(new CompanyDto());
        }

        // POST: CompanyManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]CompanyDto companyDto)
        {
            if (ModelState.IsValid)
            {
                await _companyService.CreateCompanyAsync(companyDto);
                TempData["SuccessMessage"] = "Company created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(companyDto);
        }

        // GET: CompanyManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var companyResult = await _companyService.GetCompanyByIdAsync(id);

            if (companyResult == null || companyResult.Data == null)
                return NotFound();

            return View(companyResult.Data);
        }

        // POST: CompanyManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyDto companyDto)
        {
            if (id != companyDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _companyService.UpdateCompanyAsync(id, companyDto);
                TempData["SuccessMessage"] = "Company updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(companyDto);
        }

        // GET: CompanyManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var companyResult = await _companyService.GetCompanyByIdAsync(id);

            if (companyResult == null || companyResult.Data == null)
                return NotFound();

            return View(companyResult.Data);
        }

        // POST: CompanyManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _companyService.DeleteCompanyAsync(id);
            TempData["SuccessMessage"] = "Company deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
