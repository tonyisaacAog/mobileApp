using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class ProductManagementController : Controller
    {
        private readonly IProductService _productService;

        public ProductManagementController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: ProductManagement
        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAllProductsAsync(new PaginationParameters());
            return View(result.Data ?? new List<ProductDto>());
        }

        // GET: ProductManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var productResult = await _productService.GetProductByIdAsync(id);

            if (productResult == null || productResult.Data == null)
                return NotFound();

            return View(productResult.Data);
        }

        // GET: ProductManagement/Create
        public IActionResult Create()
        {
            return View(new ProductDto());
        }

        // POST: ProductManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(productDto);
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        // GET: ProductManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var productResult = await _productService.GetProductByIdAsync(id);

            if (productResult == null || productResult.Data == null)
                return NotFound();

            return View(productResult.Data);
        }

        // POST: ProductManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto productDto)
        {
            if (id != productDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(id, productDto);
                TempData["SuccessMessage"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(productDto);
        }

        // GET: ProductManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var productResult = await _productService.GetProductByIdAsync(id);

            if (productResult == null || productResult.Data == null)
                return NotFound();

            return View(productResult.Data);
        }

        // POST: ProductManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
