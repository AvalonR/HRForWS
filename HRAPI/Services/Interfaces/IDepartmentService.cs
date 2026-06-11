using HRAPI.DTOs.Departments;

namespace HRAPI.Services.Interfaces;

// Contract used by DepartmentsController so it depends on an abstraction instead of a concrete service.
public interface IDepartmentService
{
    Task<List<DepartmentReadDto>> GetAllAsync();
    Task<DepartmentReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<DepartmentReadDto>> CreateAsync(DepartmentCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, DepartmentUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
