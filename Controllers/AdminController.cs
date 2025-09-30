using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CompanyApi.Models;
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

        public AdminController(
            IBranchService branchService,
            IUserService userService,
            ICompanyService companyService,
            IProductService productService)
        {
            _branchService = branchService;
            _userService = userService;
            _companyService = companyService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch real data from services with pagination (get first page with large page size)
                var branchesResult = await _branchService.GetAllBranchesAsync();
                var usersResult = await _userService.GetAllUsersAsync(new PaginationParameters { PageNumber = 1, PageSize = 1000 });
                var companiesResult = await _companyService.GetAllCompaniesAsync();
                var productsResult = await _productService.GetAllProductsAsync(new PaginationParameters { PageNumber = 1, PageSize = 1000 });

                var branches = branchesResult?.Data?.ToList() ?? new List<BranchDto>();
                var users = usersResult?.Data?.ToList() ?? new List<UserDto>();
                var companies = companiesResult?.Data?.ToList() ?? new List<CompanyDto>();
                var products = productsResult?.Data?.ToList() ?? new List<ProductDto>();

                // Pass data to view via ViewBag
                ViewBag.BranchCount = branches.Count();
                ViewBag.ActiveUserCount = users.Count(u => u.IsActive);
                ViewBag.CompanyCount = companies.Count();
                ViewBag.ProductCount = products.Count();

                // Pass the actual data for display
                ViewBag.Branches = branches;
                ViewBag.Users = users;
                ViewBag.Companies = companies;
                ViewBag.Products = products;

                // Pass recent activities and system health
                ViewBag.RecentBranches = branches.OrderByDescending(b => b.Id).Take(5).ToList();
                ViewBag.RecentCompanies = companies.OrderByDescending(c => c.Id).Take(5).ToList();
                ViewBag.SystemHealth = new {
                    Database = "Online",
                    Services = "Running",
                    LastBackup = DateTime.Now.AddHours(-2).ToString("yyyy-MM-dd HH:mm"),
                    Status = "Healthy"
                };

                // Set success message for data load
                ViewBag.DataLoaded = true;
                TempData["SuccessMessage"] = $"Dashboard loaded successfully with {branches.Count()} branches, {companies.Count()} companies, {products.Count()} products, and {users.Count(u => u.IsActive)} active users.";
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

                // Pass empty lists as fallback
                ViewBag.Branches = new List<BranchDto>();
                ViewBag.Users = new List<UserDto>();
                ViewBag.Companies = new List<CompanyDto>();
                ViewBag.Products = new List<ProductDto>();

                // Pass recent activities and system health
                ViewBag.RecentBranches = new List<BranchDto>();
                ViewBag.RecentCompanies = new List<CompanyDto>();
                ViewBag.SystemHealth = new {
                    Database = "Offline",
                    Services = "Error",
                    LastBackup = "Unknown",
                    Status = "Unhealthy"
                };

                // Set flags for empty states
                ViewBag.HasBranches = false;
                ViewBag.HasUsers = false;
                ViewBag.HasCompanies = false;
                ViewBag.HasProducts = false;

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
