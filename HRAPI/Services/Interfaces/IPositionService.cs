using HRAPI.DTOs.Positions;

namespace HRAPI.Services.Interfaces;

// Contract for position operations used by PositionsController.
public interface IPositionService
{
    Task<List<PositionReadDto>> GetAllAsync();
    Task<PositionReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<PositionReadDto>> CreateAsync(PositionCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, PositionUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
