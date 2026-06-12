using HRAPI.DTOs.Attendances;

namespace HRAPI.Services.Interfaces;

// Contract for attendance operations used by AttendancesController.
public interface IAttendanceService
{
    Task<List<AttendanceReadDto>> GetAllAsync();
    Task<AttendanceReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<AttendanceReadDto>> CreateAsync(AttendanceCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, AttendanceUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
