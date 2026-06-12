using BusinessLayer.Common;
using BusinessLayer.DTOs;
namespace BusinessLayer.Interfaces
{
    public interface IPolicyCategoryService
    {
        Task<ApiResponse<IEnumerable<CreateUpdatePolicyCategoryDto>>> GetAll(int userId);
        Task<ApiResponse<IEnumerable<CreateUpdatePolicyCategoryDto>>> GetByCompanyAndRegion(
     int companyId,
     int regionId);
        Task<ApiResponse<CreateUpdatePolicyCategoryDto?>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(CreateUpdatePolicyCategoryDto dto);
        Task<ApiResponse<string>> UpdateAsync(CreateUpdatePolicyCategoryDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);

        Task<ApiResponse<IEnumerable<CreateUpdatePolicyCategoryDto>>>
GetByCompanyRegion(int companyId, int regionId);

    }
}
