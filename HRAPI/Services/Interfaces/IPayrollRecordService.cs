using HRAPI.DTOs.PayrollRecords;

namespace HRAPI.Services.Interfaces;

public interface IPayrollRecordService
{
    Task<List<PayrollRecordReadDto>> GetAllAsync();
    Task<PayrollRecordReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<PayrollRecordReadDto>> CreateAsync(PayrollRecordCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, PayrollRecordUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
