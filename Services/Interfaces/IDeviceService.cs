using CompanyApi.DTOs;

namespace CompanyApi.Services.Interfaces
{
    public interface IDeviceService
    {
        Task CreateDeviceAsync(DeviceDto dto);
        Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync(PaginationParameters paginationParameters);
        Task<Result<DeviceDto>?> GetDeviceByIdAsync(int id);
        Task UpdateDeviceAsync(int id, DeviceDto dto);
        Task DeleteDeviceAsync(int id);
    }
}
