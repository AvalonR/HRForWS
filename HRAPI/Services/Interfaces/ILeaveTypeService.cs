using HRAPI.DTOs.LeaveTypes;

namespace HRAPI.Services.Interfaces;

// Contract for leave type operations used by LeaveTypesController.
public interface ILeaveTypeService
{
    Task<List<LeaveTypeReadDto>> GetAllAsync();
    Task<LeaveTypeReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<LeaveTypeReadDto>> CreateAsync(LeaveTypeCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, LeaveTypeUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
