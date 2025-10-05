using CompanyApi.DTOs.DeviceDtos;
using CompanyApi.DTOs.ResponseDtos;

namespace CompanyApi.Services.Interfaces
{
    public interface IDeviceService
    {
        Task CreateDeviceAsync(DeviceDto dto);
        Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync();
        Task<Result<DeviceDto>?> GetDeviceByIdAsync(int id);
        Task UpdateDeviceAsync(int id, DeviceDto dto);
        Task DeleteDeviceAsync(int id);
        Task<Result<int>> GetDeviceCountAsync();
    }
}
