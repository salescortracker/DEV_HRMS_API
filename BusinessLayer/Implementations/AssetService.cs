using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;


namespace BusinessLayer.Implementations
{
    public class AssetService:IAssetService
    {
        private readonly IEmailService _emailService;
        private readonly HRMSContext _context;
        private readonly INotificationService _notificationService;

        public AssetService(HRMSContext context, IEmailService emailService, INotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }


        public async Task<int> CreateAssetAsync(AssetDto assetDto)
        {
            // Get reporting manager for selected employee
            var employee = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == assetDto.UserID);

            if (employee == null)
                throw new Exception("Selected employee not found");

            var reportingTo = employee.ReportingTo;

            // Create asset for employee
            var asset = new Asset
            {

                CompanyId = assetDto.CompanyID,
                RegionId = assetDto.RegionID,
                UserId = assetDto.UserID,
                EmployeeName = employee.FullName,  // ✅ Save employee name from DB
                AssetName = assetDto.AssetName,
                AssetCode = assetDto.AssetCode,
                AssetTypeId = assetDto.AssetType,
                AssetCategoryId = assetDto.AssetCategory,

                AssetLocation = assetDto.AssetLocation,
                AssetCost = assetDto.AssetCost,
                CurrencyCode = assetDto.CurrencyCode,
                AssetDescription = assetDto.AssetDescription,
                AssetModel = assetDto.AssetModel,
                PurchaseOrder = assetDto.PurchaseOrder,
                WarrantyStartDate = assetDto.WarrantyStartDate.HasValue
                    ? DateOnly.FromDateTime(assetDto.WarrantyStartDate.Value)
                    : null,
                WarrantyEndDate = assetDto.WarrantyEndDate.HasValue
                    ? DateOnly.FromDateTime(assetDto.WarrantyEndDate.Value)
                    : null,
                AssetReturnDate = assetDto.AssetReturnDate.HasValue
                    ? DateOnly.FromDateTime(assetDto.AssetReturnDate.Value)
                    : null,
                AssetStatusId = assetDto.AssetStatusID,
                CreatedAt = assetDto.CreatedAt ?? DateTime.Now,
                CreatedBy = assetDto.CreatedBy,
                ReportingTo = assetDto.ReportingTo,
                ApprovalStatus = "Pending" // default pending for manager approval
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            // TODO: Optionally trigger email to reporting manager here

            return asset.AssetId;
        }

        public async Task<bool> UpdateAssetAsync(AssetDto assetDto)
        {
            var asset = await _context.Assets.FindAsync(assetDto.AssetID);
            if (asset == null) return false;

            // Update employee name if UserID changed
            var employee = await _context.Users.FirstOrDefaultAsync(u => u.UserId == assetDto.UserID);
            if (employee != null)
            {
                asset.EmployeeName = employee.FullName;
                asset.UserId = employee.UserId;
                asset.ReportingTo = employee.ReportingTo;
            }

            asset.AssetName = assetDto.AssetName;
            asset.AssetCode = assetDto.AssetCode;
            asset.AssetTypeId = assetDto.AssetType;
            asset.AssetCategoryId = assetDto.AssetCategory;

            asset.AssetLocation = assetDto.AssetLocation;
            asset.AssetCost = assetDto.AssetCost;
            asset.CurrencyCode = assetDto.CurrencyCode;
            asset.AssetDescription = assetDto.AssetDescription;
            asset.AssetModel = assetDto.AssetModel;
            asset.PurchaseOrder = assetDto.PurchaseOrder;
            asset.WarrantyStartDate = assetDto.WarrantyStartDate.HasValue
                ? DateOnly.FromDateTime(assetDto.WarrantyStartDate.Value)
                : null;
            asset.WarrantyEndDate = assetDto.WarrantyEndDate.HasValue
                ? DateOnly.FromDateTime(assetDto.WarrantyEndDate.Value)
                : null;
            asset.AssetReturnDate = assetDto.AssetReturnDate.HasValue
                ? DateOnly.FromDateTime(assetDto.AssetReturnDate.Value)
                : null;
            asset.AssetStatusId = assetDto.AssetStatusID;
            asset.ModifiedAt = DateTime.Now;
            asset.ModifiedBy = assetDto.ModifiedBy;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAssetAsync(int assetId)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null) return false;

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AssetDto>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Select(a => new AssetDto
                {
                    AssetID = a.AssetId,
                    CompanyID = a.CompanyId,
                    RegionID = a.RegionId,
                    UserID = a.UserId,
                    EmployeeName = a.EmployeeName,
                    AssetName = a.AssetName,
                    AssetCode = a.AssetCode,
                    AssetType = a.AssetTypeId,
                    AssetCategory = a.AssetCategoryId,

                    AssetLocation = a.AssetLocation,
                    AssetCost = a.AssetCost,
                    CurrencyCode = a.CurrencyCode,
                    AssetDescription = a.AssetDescription,
                    AssetModel = a.AssetModel,
                    PurchaseOrder = a.PurchaseOrder,
                    WarrantyStartDate = a.WarrantyStartDate.HasValue
                        ? a.WarrantyStartDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    WarrantyEndDate = a.WarrantyEndDate.HasValue
                        ? a.WarrantyEndDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    AssetReturnDate = a.AssetReturnDate.HasValue
                        ? a.AssetReturnDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    AssetStatusID = a.AssetStatusId,
                    ReportingTo = a.ReportingTo,
                    ApprovalStatus = a.ApprovalStatus
                })
                .OrderByDescending(x => x.AssetID)
                .ToListAsync();
        }

        public async Task<List<AssetDto>> GetAssetsByUserIdAsync(int userId)
        {
            return await _context.Assets
                .Where(a => a.UserId == userId)
                .Select(a => new AssetDto
                {
                    AssetID = a.AssetId,
                    CompanyID = a.CompanyId,
                    RegionID = a.RegionId,
                    UserID = a.UserId,
                    EmployeeName = a.EmployeeName,
                    AssetName = a.AssetName,
                    AssetCode = a.AssetCode,
                    AssetType = a.AssetTypeId,
                    AssetCategory = a.AssetCategoryId,

                    AssetLocation = a.AssetLocation,
                    AssetCost = a.AssetCost,
                    CurrencyCode = a.CurrencyCode,
                    AssetDescription = a.AssetDescription,
                    AssetModel = a.AssetModel,
                    PurchaseOrder = a.PurchaseOrder,
                    WarrantyStartDate = a.WarrantyStartDate.HasValue
                        ? a.WarrantyStartDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    WarrantyEndDate = a.WarrantyEndDate.HasValue
                        ? a.WarrantyEndDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    AssetReturnDate = a.AssetReturnDate.HasValue
                        ? a.AssetReturnDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null,
                    AssetStatusID = a.AssetStatusId,
                    ReportingTo = a.ReportingTo,
                    ApprovalStatus = a.ApprovalStatus
                })
                .ToListAsync();
        }

        public async Task<List<AssetStatusDto>> GetAllAssetStatusesAsync(int companyId, int regionId)
        {
            return await _context.AssetStatuses
                .Where(x => x.IsActive &&
                !x.IsDeleted &&
                 x.CompanyId == companyId &&
                x.RegionId == regionId )
                .Select(x => new AssetStatusDto
                {
                    AssetStatusId = x.AssetStatusId,
                    AssetStatusName = x.AssetStatusName,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId
                })
                .ToListAsync();
        }
        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _context.Users
                .Where(u => u.Status == "Active") // optional filter
                .Select(u => new EmployeeDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<int> CreateAssetRequestAsync(AssetRequestDto dto)
        {
            var entity = new AssetRequest
            {
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                UserId = dto.UserID,

                EmployeeName = dto.EmployeeName,
                EmployeeCode = dto.EmployeeCode,
                Department = dto.DepartmentName,

                AssetTypeId = dto.AssetType,
                AssetCategoryId = dto.AssetCategory,

                RequiredDate = DateOnly.FromDateTime(dto.RequiredDate),
                PriorityId = dto.Priority,

                Reason = dto.Reason,
                FileName = dto.FileName,
                FilePath = dto.FilePath,

                ReportingTo = dto.ReportingTo,

                Status = "Pending",
                CreatedBy = dto.UserID,
                CreatedAt = DateTime.Now,
                HrEmail = dto.HrEmail,
            };

            _context.AssetRequests.Add(entity);
            await _context.SaveChangesAsync();
            // ================= NOTIFICATION SECTION =================

            var notificationUsers = new List<int>();


            // 1. Manager Notification
            if (dto.ReportingTo.HasValue && dto.ReportingTo.Value > 0)
            {
                notificationUsers.Add(dto.ReportingTo.Value);
            }


            // 2. Admin Notification By Email
            if (!string.IsNullOrWhiteSpace(dto.HrEmail))
            {
                var adminEmails = dto.HrEmail
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();


                var adminUsers = await _context.Users
                    .Where(x =>
                        adminEmails.Contains(x.Email) &&
                        x.CompanyId == dto.CompanyID &&
                        x.RegionId == dto.RegionID
                    )
                    .Select(x => x.UserId)
                    .ToListAsync();


                notificationUsers.AddRange(adminUsers);
            }


            // Remove duplicates
            notificationUsers = notificationUsers
                .Distinct()
                .ToList();



            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Asset Request",
                    $"{dto.EmployeeName} submitted a new Asset Request.",
                    "Asset",
                    entity.RequestId
                );
            }

            // ✅ STEP 1: Get Reporting Manager Email
            var manager = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == dto.ReportingTo);
            // ✅ Get Asset Type Name
            var assetTypeName = await _context.AssetTypes
                .Where(x => x.AssetTypeId == dto.AssetType)
                .Select(x => x.AssetTypeName)
                .FirstOrDefaultAsync();

            // ✅ Get Asset Category Name
            var assetCategoryName = await _context.AssetCategories
                .Where(x => x.AssetCategoryId == dto.AssetCategory)
                .Select(x => x.AssetCategoryName)
                .FirstOrDefaultAsync();

            // ✅ Get Priority Name
            var priorityName = await _context.Priorities
                .Where(x => x.PriorityId == dto.Priority)
                .Select(x => x.PriorityName)
                .FirstOrDefaultAsync();


            if (manager != null && !string.IsNullOrEmpty(manager.Email))
            {
                string? reportingHrEmail = null;

                var employee = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == dto.UserID);

                if (employee?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }
                // ✅ STEP 2: Build Email Body
                var body = $@"
            <h3>New Asset Request</h3>
            <p><b>Request ID:</b> {entity.RequestId}</p>
            <p><b>Employee Name:</b> {dto.EmployeeName}</p>
            <p><b>Employee Code:</b> {dto.EmployeeCode}</p>
           <p><b>Department:</b> {dto.DepartmentName}</p>
           <p><b>Asset Type:</b> {assetTypeName ?? "-"}</p>
    <p><b>Asset Category:</b> {assetCategoryName ?? "-"}</p>
    <p><b>Priority:</b> {priorityName ?? "-"}</p>
            <p><b>Required Date:</b> {dto.RequiredDate:dd-MM-yyyy}</p>
            <p><b>Reason:</b> {dto.Reason}</p>
            <p>Status: <b>Pending</b></p>
        ";

                // ✅ STEP 3: Send Email
                //await _emailService.SendEmailAsync(
                //    manager.Email,
                //    "New Asset Request Approval",
                //    body
                //);
                var ccEmails = new List<string>();

                // Reporting HR
                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccEmails.Add(reportingHrEmail);
                }

                // UI CC Emails
                if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                {
                    ccEmails.AddRange(
                        dto.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                    );
                }

                ccEmails = ccEmails.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    manager.Email,
                    "New Asset Request Approval",
                    body,
                    ccEmails   // ✅ PASS CC
                );
            }

            return entity.RequestId;
        }


        public async Task<List<AssetRequestDto>> GetAssetRequestsByUserAsync(int userId)
        {
            return await _context.AssetRequests
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.RequestId)
                .Select(x => new AssetRequestDto
                {
                    RequestID = x.RequestId,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    UserID = x.UserId,

                    EmployeeName = x.EmployeeName,
                    EmployeeCode = x.EmployeeCode,
                    DepartmentName = x.Department,

                    AssetType = x.AssetTypeId,
                    AssetCategory = x.AssetCategoryId,

                    RequiredDate = x.RequiredDate.ToDateTime(TimeOnly.MinValue),
                    Priority = x.PriorityId,

                    Reason = x.Reason,
                    FileName = x.FileName,
                    FilePath = x.FilePath,

                    ReportingTo = x.ReportingTo,
                    Status = x.Status
                })
                .ToListAsync();
        }
        public async Task<List<AssetDto>> GetAvailableAssetsAsync(int companyId, int regionId, int userId)
        {
            return await _context.Assets
                .Where(a => a.CompanyId == companyId
                         && a.RegionId == regionId
                         && a.UserId == userId
                         && a.AssetStatus.AssetStatusName == "Available"
                         ) // ✅ Available only
                .Select(a => new AssetDto
                {
                    AssetID = a.AssetId,
                    AssetName = a.AssetName,
                    AssetCode = a.AssetCode
                })
                .ToListAsync();
        }
        public async Task<int> CreateAssignmentAsync(AssetAssignmentDto dto)
        {
            var entity = new AssetAssignment
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                RequestId = dto.RequestId,
                AssetId = dto.AssetId,
                EmployeeName = dto.EmployeeName,
                AssetType = dto.AssetType,
                AssetName = dto.AssetName,
                AssetCode = dto.AssetCode,

                AssignDate = DateOnly.FromDateTime(dto.AssignDate.ToUniversalTime()),

                ReturnDate = dto.ReturnDate.HasValue
                    ? DateOnly.FromDateTime(dto.ReturnDate.Value.ToUniversalTime())
                    : null,

                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            _context.AssetAssignments.Add(entity);

            // 🔥 REMOVE REQUEST FROM DROPDOWN
            var request = await _context.AssetRequests
                .FirstOrDefaultAsync(x => x.RequestId == dto.RequestId);

            if (request != null)
            {
                request.Status = "Assigned";
            }

            // 🔥 MAKE ASSET UNAVAILABLE
            var asset = await _context.Assets
                .FirstOrDefaultAsync(x => x.AssetId == dto.AssetId);

            if (asset != null)
            {
                asset.AssetStatusId = 2; // Assigned
            }

            await _context.SaveChangesAsync();

            return entity.AssignmentId;
        }

        public async Task<List<AssetAssignmentDto>> GetAssignmentsAsync(int companyId, int regionId)
        {
            return await _context.AssetAssignments
                .Where(x => x.CompanyId == companyId && x.RegionId == regionId)
                .OrderByDescending(x => x.AssignmentId)
                .Select(x => new AssetAssignmentDto
                {
                    AssignmentId = x.AssignmentId,
                    RequestId = x.RequestId,
                    EmployeeName = x.EmployeeName,
                    AssetType = x.AssetType,
                    AssetName = x.AssetName,
                    AssetCode = x.AssetCode,
                    AssignDate = x.AssignDate.ToDateTime(TimeOnly.MinValue),

                    ReturnDate = x.ReturnDate.HasValue
                ? x.ReturnDate.Value.ToDateTime(TimeOnly.MinValue)
                : null,

                    Remarks = x.Remarks
                })
                .ToListAsync();
        }


    }
}
