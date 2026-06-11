using HRAPI.DTOs.Employees;

namespace HRAPI.Services.Interfaces;

// Contract for employee operations, including validation of departments, positions, and managers.
public interface IEmployeeService
{
    Task<List<EmployeeReadDto>> GetAllAsync();
    Task<EmployeeReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<EmployeeReadDto>> CreateAsync(EmployeeCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
