using HRAPI.DTOs.Employees;

namespace HRAPI.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeReadDto>> GetAllAsync();
    Task<EmployeeReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<EmployeeReadDto>> CreateAsync(EmployeeCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
