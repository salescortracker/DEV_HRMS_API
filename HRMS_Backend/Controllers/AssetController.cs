using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IAssetApprovalService _assetApprovalService;

        public AssetController(IAssetService assetService, IAssetApprovalService assetApprovalService)
        {
            _assetService = assetService;
            _assetApprovalService = assetApprovalService;
        }

        /// <summary>
        /// Get all assets
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AssetDto>>> GetAllAssets()
        {
            var assets = await _assetService.GetAllAssetsAsync();
            return Ok(assets);
        }

        /// <summary>
        /// Get assets for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<AssetDto>>> GetAssetsByUser(int userId)
        {
            var assets = await _assetService.GetAssetsByUserIdAsync(userId);
            return Ok(assets);
        }

        /// <summary>
        /// Create a new asset
        /// </summary>
        [HttpPost("CreateAsset")]
        public async Task<ActionResult<int>> CreateAsset([FromBody] AssetDto assetDto)
        {
            var id = await _assetService.CreateAssetAsync(assetDto);
            return CreatedAtAction(nameof(GetAllAssets), new { id }, id);
        }

        /// <summary>
        /// Update an existing asset
        /// </summary>
        [HttpPost("UpdateAsset")]
        public async Task<ActionResult> UpdateAsset([FromBody] AssetDto assetDto)
        {
            var success = await _assetService.UpdateAssetAsync(assetDto);
            if (!success) return NotFound($"Asset with ID {assetDto.AssetID} not found.");
            return NoContent();
        }

        /// <summary>
        /// Delete an asset
        /// </summary>
        [HttpPost("DeleteAsset")]
        public async Task<ActionResult> DeleteAsset([FromQuery] int id)
        {
            var success = await _assetService.DeleteAssetAsync(id);
            if (!success) return NotFound($"Asset with ID {id} not found.");
            return NoContent();
        }
        /// <summary>
        /// Get all Status 
        /// </summary>
        [HttpGet("statuses")]
        public async Task<ActionResult<List<AssetStatusDto>>> GetAssetStatuses(int companyId, int regionId)
        {
            var statuses = await _assetService.GetAllAssetStatusesAsync(companyId, regionId);
            return Ok(statuses);
        }
        /// <summary>
        /// Get all active employees (for Asset dropdown)
        /// </summary>
        [HttpGet("employees")]
        public async Task<ActionResult<List<EmployeeDto>>> GetAllEmployees()
        {
            var employees = await _assetService.GetAllEmployeesAsync();
            return Ok(employees);
        }
        #region for approval assest
        /// <summary>
        /// Get pending asset approvals for reporting manager
        /// </summary>
        [HttpGet("manager/pending-approvals")]
        public async Task<ActionResult<List<AssetApprovalDto>>> GetPendingApprovals(
            [FromQuery] int managerUserId)
            {
            var result = await _assetApprovalService
                .GetPendingAssetsForManagerAsync(managerUserId);

            return Ok(result);
        }
        /// <summary>
        /// Approve or Reject asset by reporting manager
        /// </summary>
        [HttpGet("manager/approval-action")]
        public async Task<IActionResult> ApproveOrRejectAsset(
            [FromQuery] int assetId,
            [FromQuery] int managerUserId,
            [FromQuery] string action) // Approve / Reject
        {
            var success = await _assetApprovalService
                .ApproveOrRejectAssetAsync(assetId, managerUserId, action);

            if (!success)
                return BadRequest("Unable to process asset approval");

            return Ok(new
            {
                Message = $"Asset {action}d successfully"
            });
        }
        #endregion
        [HttpPost("assetsApproveReject")]
        public async Task<IActionResult> AssetsApproveReject([FromBody] ApproveRejectAssetDto dto)
        {
            try
            {
                await _assetApprovalService.ApproveRejectAssetsAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = $"Assets {dto.Action} successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost("CreateRequest")]
        public async Task<ActionResult<int>> CreateRequest([FromBody] AssetRequestDto dto)
        {
            var id = await _assetService.CreateAssetRequestAsync(dto);
            return Ok(id);
        }

        [HttpGet("user-requests")]
        public async Task<ActionResult<List<AssetRequestDto>>> GetRequestsByUser(int userId)
        {
            var data = await _assetService.GetAssetRequestsByUserAsync(userId);
            return Ok(data);
        }
        [HttpGet("approved-requests")]
        public async Task<ActionResult<List<AssetRequestDto>>> GetApprovedRequests(
     [FromQuery] int companyId,
     [FromQuery] int regionId)
        {
            var data = await _assetApprovalService
                .GetApprovedRequestsAsync(companyId, regionId);

            return Ok(data);
        }
        [HttpGet("available-assets")]
        public async Task<ActionResult<List<AssetDto>>> GetAvailableAssets(
      int companyId,
      int regionId,
      int userId)
        {
            var data = await _assetService.GetAvailableAssetsAsync(companyId, regionId, userId);
            return Ok(data);
        }

        [HttpPost("assign-asset")]
        public async Task<IActionResult> AssignAsset([FromBody] AssetAssignmentDto dto)
        {
            var id = await _assetService.CreateAssignmentAsync(dto);

            return Ok(new
            {
                success = true,
                message = "Asset Assigned Successfully",
                id = id
            });
        }

        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignments(int companyId, int regionId)
        {
            var data = await _assetService.GetAssignmentsAsync(companyId, regionId);
            return Ok(data);
        }

    }
}
