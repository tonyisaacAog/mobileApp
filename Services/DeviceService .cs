using CompanyApi.DTOs;
using CompanyApi.Models;
using CompanyApi.Repositories.Interfaces;
using CompanyApi.Repositories.Utilities;
using CompanyApi.Services.Interfaces;

namespace CompanyApi.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateDeviceAsync(DeviceDto dto)
        {
            var isUnique = await IsDeviceCodeUniqueAsync(dto.Code);
            if (!isUnique)
            {
                throw new InvalidOperationException("Device code must be unique.");
            }

            var device = new Device
            {
                Name = dto.Name,
                Code = dto.Code,
                Serial = dto.Serial,
                OS = dto.OS,
                Model = dto.Model,
                BranchId = dto.BranchId
            };

            await _unitOfWork.Repository<Device>().AddAsync(device);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Result<IEnumerable<DeviceDto>>> GetAllDevicesAsync(PaginationParameters paginationParameters)
        {
            var selectors = MappingUtilities.CreateMapExpression<Device, DeviceDto>();
            var devices = await _unitOfWork.Repository<Device>().GetProjectedAsync(selectors);

            return await Result<IEnumerable<DeviceDto>>.SuccessAsync(devices, "", 200);
        }

        public async Task<Result<DeviceDto>?> GetDeviceByIdAsync(int id)
        {
            var selectors = MappingUtilities.CreateMapExpression<Device, DeviceDto>();
            var device = await _unitOfWork.Repository<Device>().GetByIdAsync(d => d.Id == id, selectors);

            return device == null
                ? null
                : await Result<DeviceDto>.SuccessAsync(device);
        }

        public async Task UpdateDeviceAsync(int id, DeviceDto dto)
        {
            var device = await _unitOfWork.Repository<Device>().GetByIdAsync(id);
            if (device == null) throw new KeyNotFoundException("Device not found.");

            if (device.Code != dto.Code)
            {
                var isUnique = await IsDeviceCodeUniqueAsync(dto.Code);
                if (!isUnique)
                {
                    throw new InvalidOperationException("Device code must be unique.");
                }
            }

            device.Name = dto.Name;
            device.Code = dto.Code;
            device.Serial = dto.Serial;
            device.OS = dto.OS;
            device.Model = dto.Model;
            device.BranchId = dto.BranchId;

            _unitOfWork.Repository<Device>().Update(device);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _unitOfWork.Repository<Device>().GetByIdAsync(id);
            if (device == null) throw new KeyNotFoundException("Device not found.");

            _unitOfWork.Repository<Device>().Remove(device);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<bool> IsDeviceCodeUniqueAsync(string code)
        {
            var existingDevice = await _unitOfWork.Repository<Device>()
                .FirstOrDefaultAsync(d => d.Code == code);

            return existingDevice == null;
        }
    }
}
