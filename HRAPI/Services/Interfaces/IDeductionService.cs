using HRAPI.DTOs.Deductions;

namespace HRAPI.Services.Interfaces;

// Contract for deduction operations used by DeductionsController.
public interface IDeductionService
{
    Task<List<DeductionReadDto>> GetAllAsync();
    Task<DeductionReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<DeductionReadDto>> CreateAsync(DeductionCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, DeductionUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
