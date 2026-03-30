using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAssetApprovalService
    {
        Task<List<AssetApprovalDto>> GetPendingAssetsForManagerAsync(int managerUserId);

        Task<bool> ApproveOrRejectAssetAsync(
            int assetId,
            int managerUserId,
            string action   // "Approve" or "Reject"
        );
        Task ApproveRejectAssetsAsync(ApproveRejectAssetDto dto);
        Task<List<AssetRequestDto>> GetApprovedRequestsAsync(int companyId, int regionId);


    }
}
