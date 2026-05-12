using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAssetCategoryService
    {
        Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAll(int userId);

        Task<ApiResponse<AssetCategoryDto?>> GetByIdAsync(int id);

        Task<ApiResponse<string>> CreateAsync(AssetCategoryDto dto);

        Task<ApiResponse<string>> UpdateAsync(AssetCategoryDto dto);

        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<AssetCategoryDto>>> AssetCategoryDropDown(int companyId, int regionId);
    }
}
