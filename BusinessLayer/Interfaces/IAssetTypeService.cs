using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAssetTypeService
    {
        Task<ApiResponse<IEnumerable<AssetTypeDto>>> GetAll(int userId);
        Task<ApiResponse<AssetTypeDto?>> GetById(int id);
        Task<ApiResponse<string>> CreateAsync(AssetTypeDto dto);
        Task<ApiResponse<string>> UpdateAsync(AssetTypeDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<AssetTypeDto>>> GetByCompanyRegion(int companyId, int regionId, int assetCategoryId);
        Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAssetCategoriestype(int userId);

    }
}
