using HRAPI.DTOs.PerformanceReviews;

namespace HRAPI.Services.Interfaces;

// Contract for performance review operations used by PerformanceReviewsController.
public interface IPerformanceReviewService
{
    Task<List<PerformanceReviewReadDto>> GetAllAsync();
    Task<PerformanceReviewReadDto?> GetByIdAsync(int id);
    Task<ServiceResult<PerformanceReviewReadDto>> CreateAsync(PerformanceReviewCreateDto dto);
    Task<ServiceResult> UpdateAsync(int id, PerformanceReviewUpdateDto dto);
    Task<ServiceResult> DeleteAsync(int id);
}
