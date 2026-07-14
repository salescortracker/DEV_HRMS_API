using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BusinessLayer.Implementations
{
    public class AssetApprovalService:IAssetApprovalService
    {
        private readonly IEmailService _emailService;
        private readonly HRMSContext _context;
        private readonly INotificationService _notificationService;

        public AssetApprovalService(HRMSContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        // 🔹 Manager sees team pending assets
        public async Task<List<AssetApprovalDto>> GetPendingAssetsForManagerAsync(int managerUserId)
        {
            return await (
         from a in _context.AssetRequests

         join at in _context.AssetTypes
             on a.AssetTypeId equals at.AssetTypeId into atJoin
         from at in atJoin.DefaultIfEmpty()

         join p in _context.Priorities
             on a.PriorityId equals p.PriorityId into pJoin
         from p in pJoin.DefaultIfEmpty()

         where a.ReportingTo == managerUserId 

         select new AssetApprovalDto
         {
             AssetID = a.RequestId,
             AssetName = a.EmployeeName,
             AssetCode = a.EmployeeCode,
             AssetLocation = a.Department,
             AssetCost = 0,
             CurrencyCode = "",
             ApprovalStatus = a.Status,
             EmployeeName = a.EmployeeName,

             // ✅ IDs
             AssetType = a.AssetTypeId,
             Priority = a.PriorityId,

             // ✅ NAMES (THIS FIXES YOUR ISSUE)
             AssetTypeName = at.AssetTypeName,   // 🔥
             PriorityName = p.PriorityName,      // 🔥

             AssetCategory = a.AssetCategoryId,
             RequiredDate = a.RequiredDate.ToDateTime(TimeOnly.MinValue)
         }
     )
     .OrderByDescending(a => a.AssetID)
     .ToListAsync();
        }


        // 🔹 Single API → Approve / Reject
        public async Task<bool> ApproveOrRejectAssetAsync(
            int assetId,
            int managerUserId,
            string action)
        {
            var asset = await _context.Assets
                .FirstOrDefaultAsync(a =>
                    a.AssetId == assetId &&
                    a.ReportingTo == managerUserId &&
                    a.ApprovalStatus == "Pending");

            if (asset == null)
                return false;

            if (action == "Approve")
                asset.ApprovalStatus = "Approved";
            else if (action == "Reject")
                asset.ApprovalStatus = "Rejected";
            else
                throw new Exception("Invalid action");

            asset.ApprovedBy = managerUserId;
            asset.ApprovedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task ApproveRejectAssetsAsync(ApproveRejectAssetDto dto)
        {
            var requests = await (
        from r in _context.AssetRequests
        join u in _context.Users on r.UserId equals u.UserId
        where dto.AssetIds.Contains(r.RequestId)
        select new
        {
            Request = r,
            EmployeeName = u.FullName,
            EmployeeEmail = u.Email,
            HrEmail = r.HrEmail,
            ReportingHr = u.ReportingHr
        }
    ).ToListAsync();

            if (!requests.Any())
                throw new Exception("No requests found");

            // ✅ UPDATE STATUS
            foreach (var item in requests)
            {
                item.Request.Status = dto.Action; // Approved / Rejected
                item.Request.ModifiedBy = dto.ManagerId;
                item.Request.ModifiedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            var notificationUsers = new List<int>();

            foreach (var item in requests)
            {
                notificationUsers.Clear();

                // Employee Notification
                notificationUsers.Add(item.Request.UserId);


                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Asset Request",
                    $"Your asset request #{item.Request.RequestId} has been {dto.Action} by Manager.",
                    "Asset",
                    item.Request.RequestId
                );
            }

            // ✅ SEND EMAIL TO EMPLOYEE
            foreach (var item in requests)
            {
                if (!string.IsNullOrWhiteSpace(item.EmployeeEmail))
                {
                      var assetTypeName = await _context.AssetTypes
    .Where(x => x.AssetTypeId == item.Request.AssetTypeId)
    .Select(x => x.AssetTypeName)
    .FirstOrDefaultAsync();

            var categoryName = await _context.AssetCategories
                .Where(x => x.AssetCategoryId == item.Request.AssetCategoryId)
                .Select(x => x.AssetCategoryName)
                .FirstOrDefaultAsync();

            var priorityName = await _context.Priorities
                .Where(x => x.PriorityId == item.Request.PriorityId)
                .Select(x => x.PriorityName)
                .FirstOrDefaultAsync();
                    var body = $@"
                <p>Dear {item.EmployeeName},</p>

                <p>Your asset request has been <b>{dto.Action}</b>.</p>

                <p><b>Request ID:</b> {item.Request.RequestId}</p>
              <p><b>Asset Type:</b> {assetTypeName}</p>
<p><b>Category:</b> {categoryName}</p>
<p><b>Priority:</b> {priorityName}</p>

                <p><b>Required Date:</b> {item.Request.RequiredDate:dd-MM-yyyy}</p>

                <p>Status: <b>{dto.Action}</b></p>

                <p>Regards,<br/>HRMS Team</p>
            ";

                    var ccList = new List<string>();

                    // Reporting HR
                    if (item.ReportingHr.HasValue)
                    {
                        var reportingHrUser = await _context.Users
                            .FirstOrDefaultAsync(x => x.UserId == item.ReportingHr.Value);

                        if (!string.IsNullOrWhiteSpace(reportingHrUser?.Email))
                        {
                            ccList.Add(reportingHrUser.Email);
                        }
                    }

                    // UI CC Emails
                    if (!string.IsNullOrWhiteSpace(item.HrEmail))
                    {
                        ccList.AddRange(
                            item.HrEmail
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                        );
                    }

                    ccList = ccList.Distinct().ToList();


                    await _emailService.SendEmailAsync(
                        item.EmployeeEmail,
                        $"Asset Request {dto.Action}",
                        body,
                        ccList
                    );
                }
            }
        }
        public async Task<List<AssetRequestDto>> GetApprovedRequestsAsync(int companyId, int regionId)
        {
            return await (
                from r in _context.AssetRequests

                join at in _context.AssetTypes
                    on r.AssetTypeId equals at.AssetTypeId into atJoin
                from at in atJoin.DefaultIfEmpty()

                where r.CompanyId == companyId &&
                      r.RegionId == regionId &&
                      r.Status == "Approved"   // ✅ ONLY APPROVED

                select new AssetRequestDto
                {
                    RequestID = r.RequestId,
                    CompanyID = r.CompanyId,
                    RegionID = r.RegionId,
                    UserID = r.UserId,

                    EmployeeName = r.EmployeeName,
                    EmployeeCode = r.EmployeeCode,
                    Department = r.Department,

                    AssetType = r.AssetTypeId,
                    AssetCategory = r.AssetCategoryId,

                    RequiredDate = r.RequiredDate.ToDateTime(TimeOnly.MinValue),
                    Priority = r.PriorityId,

                    Reason = r.Reason,
                    Status = r.Status,

                    // 🔥 IMPORTANT (extra for UI)
                    // add this if needed in DTO
                    // AssetTypeName = at.AssetTypeName
                }
            ).OrderByDescending(x => x.RequestID).ToListAsync();
        }


        private static string BuildAssetEmail(
    string employeeName,
    string assetName,
    string assetCode,
    decimal cost,
    string currency,
    string status)
        {
            var sb = new StringBuilder();

            sb.Append($"<p>Dear {employeeName},</p>");
            sb.Append("<p>Your asset request has been processed.</p>");

            sb.Append("<table border='1' cellpadding='6' cellspacing='0'>");
            sb.Append($"<tr><td><b>Asset</b></td><td>{assetName}</td></tr>");
            sb.Append($"<tr><td><b>Asset Code</b></td><td>{assetCode}</td></tr>");
            sb.Append($"<tr><td><b>Cost</b></td><td>{currency} {cost}</td></tr>");
            sb.Append($"<tr><td><b>Status</b></td><td><b>{status}</b></td></tr>");
            sb.Append("</table>");

            sb.Append("<p>Please login to HRMS for more details.</p>");
            sb.Append("<p>Regards,<br/>HRMS Team</p>");

            return sb.ToString();
        }
    }
}
