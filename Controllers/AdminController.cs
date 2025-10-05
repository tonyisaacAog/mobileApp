using CompanyApi.DTOs.BranchDtos;
using CompanyApi.DTOs.CompanyDtos;
using CompanyApi.DTOs.ProductDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.DTOs.UserDtos;
using CompanyApi.Models;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IBranchService _branchService;
        private readonly IUserService _userService;
        private readonly ICompanyService _companyService;
        private readonly IProductService _productService;
        private readonly IDocumentService _documentService;
        private readonly IDeviceService _deviceService;

        public AdminController(
            IBranchService branchService,
            IUserService userService,
            ICompanyService companyService,
            IProductService productService,
            IDocumentService documentService,
            IDeviceService deviceService)
        {
            _branchService = branchService;
            _userService = userService;
            _companyService = companyService;
            _productService = productService;
            _documentService = documentService;
            _deviceService = deviceService;
        }

        [HttpPost("SetLanguage")]
        public IActionResult SetLanguage(string culture,string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch real data from services with pagination (get first page with large page size)
                var branchesResult = await _branchService.GetCountBranches();
                var usersResult = await _userService.GetCountUsers();
                var companiesResult = await _companyService.GetCountCompanies();
                var productsResult = await _productService.GetCountProducts();
                var ordersResult = await _documentService.GetDocumentCountAsync();
                var devicesResult = await _deviceService.GetDeviceCountAsync();

                var branches = branchesResult?.Data ??0;
                var users = usersResult?.Data ?? 0;
                var companies = companiesResult?.Data ?? 0;
                var products = productsResult?.Data ?? 0;
                var orders = ordersResult?.Data ?? 0;
                var devices = devicesResult?.Data ?? 0;

                // Pass data to view via ViewBag
                ViewBag.BranchCount = branches;
                ViewBag.ActiveUserCount = users;
                ViewBag.CompanyCount = companies;
                ViewBag.ProductCount = products;
                ViewBag.OrderCount = orders;
                ViewBag.DeviceCount = devices;

                // Set success message for data load
                ViewBag.DataLoaded = true;
                TempData["SuccessMessage"] = $"Dashboard loaded successfully with {branches} branches, {companies} companies, {products} products, {orders} orders, {devices} devices, and {users} active users.";
            }
            catch (Exception ex)
            {
                // Log the error (in a real app, use proper logging)
                Console.WriteLine($"Error loading admin dashboard data: {ex.Message}");

                // Use default values if services fail
                ViewBag.BranchCount = 0;
                ViewBag.ActiveUserCount = 0;
                ViewBag.CompanyCount = 0;
                ViewBag.ProductCount = 0;
                ViewBag.OrderCount = 0;
                ViewBag.DeviceCount = 0;


                // Set flags for empty states
                ViewBag.HasBranches = false;
                ViewBag.HasUsers = false;
                ViewBag.HasCompanies = false;
                ViewBag.HasProducts = false;
                ViewBag.HasOrders = false;
                ViewBag.HasDevices = false;

                // Set error flag
                ViewBag.DataLoaded = false;
                ViewBag.ErrorMessage = "Unable to load dashboard data. Please try again later.";
                TempData["ErrorMessage"] = "Unable to load dashboard data. Please try again later.";
            }

            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
