using HRAPI.DTOs.SalaryHistories;

namespace HRAPI.Services.Interfaces;

public interface ISalaryHistoryService
{
    Task<List<SalaryHistoryReadDto>> GetAllAsync();
    Task<SalaryHistoryReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<SalaryHistoryReadDto>> CreateAsync(SalaryHistoryCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, SalaryHistoryUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
