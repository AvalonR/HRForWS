using HRAPI.DTOs.LeaveRequests;

namespace HRAPI.Services.Interfaces;

// Contract for leave request operations and validation rules.
public interface ILeaveRequestService
{
    Task<List<LeaveRequestReadDto>> GetAllAsync();
    Task<LeaveRequestReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<LeaveRequestReadDto>> CreateAsync(LeaveRequestCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, LeaveRequestUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
