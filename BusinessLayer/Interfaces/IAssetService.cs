using BusinessLayer.DTOs;


namespace BusinessLayer.Interfaces
{
    public interface IAssetService
    {
        Task<List<AssetDto>> GetAllAssetsAsync();
        Task<List<AssetDto>> GetAssetsByUserIdAsync(int userId);
        Task<int> CreateAssetAsync(AssetDto assetDto);
        Task<bool> UpdateAssetAsync(AssetDto assetDto);
        Task<bool> DeleteAssetAsync(int assetId);
        Task<List<AssetStatusDto>> GetAllAssetStatusesAsync(int companyId, int regionId);

        Task<List<EmployeeDto>> GetAllEmployeesAsync();
        Task<int> CreateAssetRequestAsync(AssetRequestDto dto);
        Task<List<AssetRequestDto>> GetAssetRequestsByUserAsync(int userId);
        Task<List<AssetDto>> GetAvailableAssetsAsync(int companyId, int regionId, int userId);
        Task<int> CreateAssignmentAsync(AssetAssignmentDto dto);
        Task<List<AssetAssignmentDto>> GetAssignmentsAsync(int companyId, int regionId);


    }
}
