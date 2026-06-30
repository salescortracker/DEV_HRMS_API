using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IMaritalStatusService
    {
        Task<List<MaritalStatusDto>> GetAllAsync(int UserId);
        Task<bool> CreateAsync(MaritalStatusDto dto);
        Task<bool> UpdateAsync(MaritalStatusDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
