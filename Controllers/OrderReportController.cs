using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.OrderReportDtos;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CompanyApi.Controllers
{
    public class OrderReportController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IDeviceService _deviceService;
        private readonly IUserService _userService;

        public OrderReportController(IDocumentService documentService, IDeviceService deviceService, IUserService userService)
        {
            _documentService = documentService;
            _deviceService = deviceService;
            _userService = userService;
        }

        // GET: OrderReport
        public async Task<IActionResult> Index(OrderReportFilterDto filter)
        {
            ViewData["Title"] = "تقرير الطلبات";

            // Get filter options
            var users = await _userService.GetAllUsersAsync();
            var devices = await _deviceService.GetAllDevicesAsync();

            ViewBag.Users = users.Data?.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.FirstName} {u.LastName}"
            }).ToList() ?? new List<SelectListItem>();

            ViewBag.Devices = devices.Data?.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Name} ({d.Code})"
            }).ToList() ?? new List<SelectListItem>();

            // Get orders with filters
            var ordersResult = await _documentService.GetOrdersWithFiltersAsync(filter);

            var viewModel = new OrderReportViewDto
            {
                Orders = ordersResult.Data ?? new List<OrderReportDto>(),
                Filter = filter,
                TotalCount = ordersResult.Data?.Count() ?? 0
            };

            return View(viewModel);
        }

        // GET: OrderReport/Details/5
        public async Task<IActionResult> Details(int id)
        {
            ViewData["Title"] = "تفاصيل الطلب";

            var orderResult = await _documentService.GetOrderDetailsAsync(id);

            if (orderResult == null || orderResult.Data == null)
            {
                TempData["ErrorMessage"] = "الطلب غير موجود";
                return RedirectToAction("Index");
            }

            return View(orderResult.Data);
        }

        // GET: OrderReport/Print/5
        public async Task<IActionResult> Print(int id)
        {
            ViewData["Title"] = "طباعة الطلب";

            var orderResult = await _documentService.GetOrderDetailsAsync(id);

            if (orderResult == null || orderResult.Data == null)
            {
                TempData["ErrorMessage"] = "الطلب غير موجود";
                return RedirectToAction("Index");
            }

            // Return print view without layout
            return View(orderResult.Data);
        }
    }
}
