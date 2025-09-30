using CompanyApi.DTOs;
using CompanyApi.DTOs.DeviceDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    // [Authorize(Policy = "AdminOnly")]
    public class DeviceManagementController : Controller
    {
        private readonly IDeviceService _deviceService;

        public DeviceManagementController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        // GET: DeviceManagement
        public async Task<IActionResult> Index(PaginationParameters paginationParameters)
        {
            var result = await _deviceService.GetAllDevicesAsync(paginationParameters);
            return View(result.Data ?? new List<DeviceDto>());
        }

        // GET: DeviceManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var deviceResult = await _deviceService.GetDeviceByIdAsync(id);

            if (deviceResult == null || deviceResult.Data == null)
                return NotFound();

            return View(deviceResult.Data);
        }

        // GET: DeviceManagement/Create
        public IActionResult Create()
        {
            return View(new DeviceDto());
        }

        // POST: DeviceManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeviceDto deviceDto)
        {
            if (ModelState.IsValid)
            {
                await _deviceService.CreateDeviceAsync(deviceDto);
                TempData["SuccessMessage"] = "Device created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(deviceDto);
        }

        // GET: DeviceManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var deviceResult = await _deviceService.GetDeviceByIdAsync(id);

            if (deviceResult == null || deviceResult.Data == null)
                return NotFound();

            return View(deviceResult.Data);
        }

        // POST: DeviceManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DeviceDto deviceDto)
        {
            if (id != deviceDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _deviceService.UpdateDeviceAsync(id, deviceDto);
                TempData["SuccessMessage"] = "Device updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(deviceDto);
        }

        // GET: DeviceManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var deviceResult = await _deviceService.GetDeviceByIdAsync(id);

            if (deviceResult == null || deviceResult.Data == null)
                return NotFound();

            return View(deviceResult.Data);
        }

        // POST: DeviceManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _deviceService.DeleteDeviceAsync(id);
            TempData["SuccessMessage"] = "Device deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
