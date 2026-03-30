using BusinessLayer.Common;
using BusinessLayer.DTOs;
namespace BusinessLayer.Interfaces
{
    public interface IAssetStatusService
    {
        Task<ApiResponse<IEnumerable<AssetStatusDto>>> GetAll(int userId);
        Task<ApiResponse<string>> CreateAsync(AssetStatusDto dto);
        Task<ApiResponse<string>> UpdateAsync(AssetStatusDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
       

    }
}
