using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Models;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class ShiftAllocationService: IShiftAllocationService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService emailservice;

        public ShiftAllocationService(HRMSContext context, IEmailService emailService)
        {
            _context = context;
            emailservice = emailService;
        }

        // ======================================================
        //              SHIFT MASTER SERVICES
        // ======================================================

        public async Task<IEnumerable<ShiftMasterDto>> GetAllShiftsAsync(int userId)
        {
            return await _context.ShiftMasters
                .Where(x => x.UserId == userId)
                .Select(x => new ShiftMasterDto
                {
                    ShiftID = x.ShiftId,
                    ShiftName = x.ShiftName,
                    ShiftStartTime = x.ShiftStartTime.ToString("HH:mm"),
                    ShiftEndTime = x.ShiftEndTime.ToString("HH:mm"),
                    GraceTime = x.GraceTime,
                    CompanyName = x.CompanyId != null ? _context.Companies.Where(c => c.CompanyId == x.CompanyId).FirstOrDefault().CompanyName : null,
                    RegionName = x.RegionId != null ? _context.Regions.Where(r => r.RegionId == x.RegionId).FirstOrDefault().RegionName : null,
                    IsActive = x.IsActive,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ModifiedAt = x.ModifiedAt,
                    ModifiedBy = x.ModifiedBy,
                    UserId = x.UserId
                }).ToListAsync();
        }

        public async Task<ShiftMasterDto?> GetShiftByIdAsync(int shiftId)
        {
            return await _context.ShiftMasters
                .Where(x => x.ShiftId == shiftId)
                .Select(x => new ShiftMasterDto
                {
                    ShiftID = x.ShiftId,
                    ShiftName = x.ShiftName,
                    ShiftStartTime = x.ShiftStartTime.ToString("HH:mm"),
                    ShiftEndTime = x.ShiftEndTime.ToString("HH:mm"),
                    GraceTime = x.GraceTime,
                    IsActive = x.IsActive,
                    CompanyID = x.CompanyId,
                    RegionID = x.RegionId,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    ModifiedAt = x.ModifiedAt,
                    ModifiedBy = x.ModifiedBy,
                    UserId = x.UserId
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> AddShiftAsync(ShiftMasterDto dto)
        {
            try
            {
                if (!TimeOnly.TryParse(dto.ShiftStartTime, out var startTime))
                    throw new ArgumentException("Invalid ShiftStartTime");

                if (!TimeOnly.TryParse(dto.ShiftEndTime, out var endTime))
                    throw new ArgumentException("Invalid ShiftEndTime");

                bool exists = await _context.ShiftMasters
              .AnyAsync(s => s.ShiftName == dto.ShiftName
                            && s.CompanyId == dto.CompanyID
                            && s.RegionId == dto.RegionID);

                if (exists)
                    throw new InvalidOperationException(
                        $"Shift '{dto.ShiftName}' already exists for this Company and Region."
                    );
                if (exists)
                    throw new InvalidOperationException(
                        $"Shift '{dto.ShiftName}' already exists for this Company and Region."
                    );

                var entity = new ShiftMaster
                {
                    ShiftName = dto.ShiftName,
                    ShiftStartTime = startTime,
                    ShiftEndTime = endTime,
                    GraceTime = dto.GraceTime,
                    CompanyId = dto.CompanyID,
                    RegionId = dto.RegionID,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId,
                    UserId = dto.UserId
                };

                _context.ShiftMasters.Add(entity);
                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> UpdateShiftAsync(ShiftMasterDto dto)
        {
            var entity = await _context.ShiftMasters.FindAsync(dto.ShiftID);
            if (entity == null) return false;

            entity.ShiftName = dto.ShiftName;
            entity.ShiftStartTime = TimeOnly.Parse(dto.ShiftStartTime);
            entity.ShiftEndTime = TimeOnly.Parse(dto.ShiftEndTime);
            entity.GraceTime = dto.GraceTime;
            entity.GraceTime = dto.GraceTime;
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.UserId = dto.UserId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ApiResponse<bool>> DeleteShiftAsync(int shiftId)
        {
            try
            {
                var entity = await _context.ShiftMasters
                    .FirstOrDefaultAsync(x => x.ShiftId == shiftId);

                if (entity == null)
                {
                    return new ApiResponse<bool>(
                        false,
                        "Shift not found.",
                        false);
                }

                // Check whether Shift is assigned
                var isAssigned = await _context.ShiftAllocations
                    .AnyAsync(x => x.ShiftId == shiftId);

                if (isAssigned)
                {
                    return new ApiResponse<bool>(
                        false,
                        "You cannot delete this shift. It is assigned to one or more employees.",
                        false);
                }

                // Delete (or Soft Delete if your table has IsDeleted)
                _context.ShiftMasters.Remove(entity);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(
                    true,
                    "Shift deleted successfully.",
                    true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(
                    false,
                    ex.Message,
                    false);
            }
        }
        public async Task<bool> ActivateShiftAsync(int shiftId)
        {
            var entity = await _context.ShiftMasters.FindAsync(shiftId);
            if (entity == null) return false;

            entity.IsActive = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeactivateShiftAsync(int shiftId)
        {
            var entity = await _context.ShiftMasters.FindAsync(shiftId);
            if (entity == null) return false;

            entity.IsActive = false;
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<ShiftMasterDto>> GetShiftsForDropdownAsync(int companyId, int regionId)
        {
            return await _context.ShiftMasters
                .Where(x => x.CompanyId == companyId && x.RegionId == regionId && x.IsActive)
                .Select(x => new ShiftMasterDto
                {
                    ShiftID = x.ShiftId,
                    ShiftName = x.ShiftName,
                    ShiftStartTime = x.ShiftStartTime.ToString("HH:mm"),
                    ShiftEndTime = x.ShiftEndTime.ToString("HH:mm")
                })
                .ToListAsync();
        }

        // ======================================================
        //              SHIFT ALLOCATION SERVICES
        // ======================================================

        public async Task<IEnumerable<ShiftAllocationDto>> GetAllAllocationsAsync(int userId)
        {
            return await (
                from sa in _context.ShiftAllocations
                where sa.UserId == userId
                join u in _context.Users
                    on sa.UserId equals u.UserId into userGroup
                from u in userGroup.DefaultIfEmpty()

                join sm in _context.ShiftMasters
                    on sa.ShiftId equals sm.ShiftId into shiftGroup
                from sm in shiftGroup.DefaultIfEmpty()

                select new ShiftAllocationDto
                {
                    ShiftAllocationId = sa.ShiftAllocationId,
                    UserID = sa.UserId,
                    EmployeeCode = sa != null ? sa.EmployeeCode : "",
                    FullName = sa != null ? sa.FullName : "",
                    CompanyID = sa.CompanyId,
                    RegionID = sa.RegionId,
                    ShiftID = sa.ShiftId ?? 0,
                    ShiftName = sm != null ? sm.ShiftName : "",

                    StartDate = sa.StartDate,
                    EndDate = sa.EndDate,

                    IsActive = sa.IsActive,
                    CreatedBy = sa.CreatedBy,
                    CreatedDate = sa.CreatedDate,
                    ModifiedBy = sa.ModifiedBy,
                    ModifiedDate = sa.ModifiedDate
                }
            ).ToListAsync();
        }

        public async Task<IEnumerable<ShiftAllocationDto>> GetAllocationsAsync(int companyId, int regionId)
        {
            return await (
                from sa in _context.ShiftAllocations
                where sa.CompanyId == companyId && sa.RegionId == regionId

                join u in _context.Users
                    on sa.UserId equals u.UserId into userGroup
                from u in userGroup.DefaultIfEmpty()

                join sm in _context.ShiftMasters
                    on sa.ShiftId equals sm.ShiftId into shiftGroup
                from sm in shiftGroup.DefaultIfEmpty()

                select new ShiftAllocationDto
                {
                    ShiftAllocationId = sa.ShiftAllocationId,
                    UserID = sa.UserId,
                    EmployeeCode = sa.EmployeeCode,
                    FullName = sa.FullName,

                    CompanyID = sa.CompanyId,
                    RegionID = sa.RegionId,

                    ShiftID = sa.ShiftId ?? 0,
                    ShiftName = sm != null ? sm.ShiftName : "",

                    StartDate = sa.StartDate,
                    EndDate = sa.EndDate,

                    IsActive = sa.IsActive,
                    CreatedBy = sa.CreatedBy,
                    CreatedDate = sa.CreatedDate
                }
            ).ToListAsync();
        }



        public async Task<ShiftAllocationDto?> GetAllocationByIdAsync(int id)
        {
            return await (
                from sa in _context.ShiftAllocations
                join u in _context.Users on sa.UserId equals u.UserId
                join sm in _context.ShiftMasters on sa.ShiftId equals sm.ShiftId
                where sa.ShiftAllocationId == id
                select new ShiftAllocationDto
                {
                    ShiftAllocationId = sa.ShiftAllocationId,
                    UserID = sa.UserId,
                    EmployeeCode = u.EmployeeCode,
                    FullName = u.FullName,
                    CompanyID = sa.CompanyId,
                    RegionID = sa.RegionId,
                    ShiftID = sa.ShiftId ?? 0,
                    ShiftName = sm.ShiftName,
                    StartDate = sa.StartDate,
                    EndDate = sa.EndDate,
                    IsActive = sa.IsActive,
                    CreatedBy = sa.CreatedBy,
                    CreatedDate = sa.CreatedDate,
                    ModifiedBy = sa.ModifiedBy,
                    ModifiedDate = sa.ModifiedDate
                }
            ).FirstOrDefaultAsync();
        }

        public async Task<bool> AllocateShiftAsync(ShiftAllocationDto dto)
        {
            var entity = new ShiftAllocation
            {
                UserId = dto.UserID,
                ShiftId = dto.ShiftID,
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                EmployeeCode = dto.EmployeeCode,
                FullName = dto.FullName,
                ShiftName = dto.ShiftName,

                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true,
                CreatedBy = dto.CreatedBy,
                CreatedDate = dto.CreatedDate
            };

            _context.ShiftAllocations.Add(entity);
            var saved = await _context.SaveChangesAsync() > 0;

            if (saved)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == dto.UserID);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    try
                    {
                        //var subject = "New Shift Assigned";

                        //var body = $@"
                        //   <p>Dear {dto.FullName},</p>

                        //   <p>Your shift has been assigned successfully.</p>

                        //   <table border='1' cellpadding='5' cellspacing='0'>
                        //       <tr><td><b>Shift</b></td><td>{dto.ShiftName}</td></tr>
                        //       <tr><td><b>Start Date</b></td><td>{dto.StartDate:dd-MM-yyyy}</td></tr>
                        //       <tr><td><b>End Date</b></td><td>{(dto.EndDate.HasValue ? dto.EndDate.Value.ToString("dd-MM-yyyy") : "N/A")}</td></tr>
                        //   </table>

                        //   <p>Regards,<br/>HR Team</p>";
                        var shift = await _context.ShiftMasters
    .FirstOrDefaultAsync(x => x.ShiftId == dto.ShiftID);
                        var subject = $"Shift Allocation Notification - {dto.ShiftName}";

                        var body = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #333;
            line-height: 1.6;
        }}

        table {{
            border-collapse: collapse;
            width: 600px;
            margin-top: 15px;
        }}

        th {{
            background-color: #0d6efd;
            color: white;
            padding: 10px;
            text-align: left;
        }}

        td {{
            border: 1px solid #ddd;
            padding: 10px;
        }}

        .footer {{
            margin-top: 20px;
        }}
    </style>
</head>

<body>

<p>Dear <strong>{dto.FullName}</strong>,</p>

<p>
We would like to inform you that your work shift has been successfully assigned.
Please find the details of your allocated shift below:
</p>

<table>
    <tr>
        <th colspan='2'>Shift Allocation Details</th>
    </tr>

    <tr>
        <td><strong>Employee Code</strong></td>
        <td>{dto.EmployeeCode}</td>
    </tr>

    <tr>
        <td><strong>Employee Name</strong></td>
        <td>{dto.FullName}</td>
    </tr>

    <tr>
        <td><strong>Shift Name</strong></td>
        <td>{dto.ShiftName}</td>
    </tr>

    <tr>
        <td><strong>Shift Timings</strong></td>
        <td>{shift?.ShiftStartTime:hh\\:mm} - {shift?.ShiftEndTime:hh\\:mm}</td>
    </tr>

    <tr>
        <td><strong>Effective From</strong></td>
        <td>{dto.StartDate:dd MMM yyyy}</td>
    </tr>

    <tr>
        <td><strong>Effective To</strong></td>
        <td>{(dto.EndDate.HasValue ? dto.EndDate.Value.ToString("dd MMM yyyy") : "Until Further Notice")}</td>
    </tr>
</table>

<p>
Kindly ensure that you report to work according to the above shift timings.
If you have any questions regarding your shift allocation, please contact the HR Department.
</p>

<p>Thank you for your cooperation.</p>

<div class='footer'>
Regards,<br/>
<strong>HR Department</strong><br/>
Your Company Name
</div>

</body>
</html>";

                        await emailservice.SendEmailAsync(user.Email, subject, body);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return saved;
        }

        public async Task<bool> UpdateAllocationAsync(ShiftAllocationDto dto)
        {
            var entity = await _context.ShiftAllocations.FindAsync(dto.ShiftAllocationId);
            if (entity == null) return false;

            entity.UserId = dto.UserID;
            entity.FullName = dto.FullName;
            entity.EmployeeCode = dto.EmployeeCode;
            entity.ShiftId = dto.ShiftID;  // FIXED
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedDate = DateTime.Now;

            var updated = await _context.SaveChangesAsync() > 0;
            if (updated)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == dto.UserID);

                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    try
                    {
                        //var subject = "Shift Updated";

                        //var body = $@"
                        //   <p>Dear {dto.FullName},</p>

                        //   <p>Your shift has been <b>updated</b>.</p>

                        //   <table border='1' cellpadding='5' cellspacing='0'>
                        //       <tr><td><b>Shift</b></td><td>{dto.ShiftName}</td></tr>
                        //       <tr><td><b>Start Date</b></td><td>{dto.StartDate:dd-MM-yyyy}</td></tr>
                        //       <tr><td><b>End Date</b></td><td>{(dto.EndDate.HasValue ? dto.EndDate.Value.ToString("dd-MM-yyyy") : "N/A")}</td></tr>
                        //   </table>

                        //   <p>Regards,<br/>HR Team</p>";
                        var shift = await _context.ShiftMasters
    .FirstOrDefaultAsync(x => x.ShiftId == dto.ShiftID);
                        var subject = $"Shift Allocation Updated - {dto.ShiftName}";

                        var body = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #333;
            line-height: 1.6;
        }}

        table {{
            border-collapse: collapse;
            width: 600px;
            margin-top: 15px;
        }}

        th {{
            background-color: #0d6efd;
            color: #ffffff;
            padding: 10px;
            text-align: left;
        }}

        td {{
            border: 1px solid #ddd;
            padding: 10px;
        }}

        .note {{
            margin-top: 15px;
            padding: 12px;
            background-color: #f8f9fa;
            border-left: 4px solid #0d6efd;
        }}

        .footer {{
            margin-top: 20px;
        }}
    </style>
</head>

<body>

<p>Dear <strong>{dto.FullName}</strong>,</p>

<p>
This is to inform you that your work shift has been <strong>updated</strong>.
Please find your revised shift details below.
</p>

<table>
    <tr>
        <th colspan='2'>Updated Shift Details</th>
    </tr>

    <tr>
        <td><strong>Employee Code</strong></td>
        <td>{dto.EmployeeCode}</td>
    </tr>

    <tr>
        <td><strong>Employee Name</strong></td>
        <td>{dto.FullName}</td>
    </tr>

    <tr>
        <td><strong>Shift Name</strong></td>
        <td>{dto.ShiftName}</td>
    </tr>

    <tr>
        <td><strong>Shift Timings</strong></td>
        <td>{shift?.ShiftStartTime:hh\\:mm} - {shift?.ShiftEndTime:hh\\:mm}</td>
    </tr>

    <tr>
        <td><strong>Effective From</strong></td>
        <td>{dto.StartDate:dd MMM yyyy}</td>
    </tr>

    <tr>
        <td><strong>Effective To</strong></td>
        <td>{(dto.EndDate.HasValue ? dto.EndDate.Value.ToString("dd MMM yyyy") : "Until Further Notice")}</td>
    </tr>
</table>

<div class='note'>
<strong>Important:</strong> Your previous shift assignment has been replaced with the above schedule.
Please report to work according to the updated shift timings from the effective date.
</div>

<p>
If you have any questions regarding this update, please contact the HR Department.
</p>

<div class='footer'>
Regards,<br/>
<strong>HR Department</strong><br/>
Your Company Name
</div>

</body>
</html>";

                        await emailservice.SendEmailAsync(user.Email, subject, body);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return updated;
        }

        public async Task<bool> DeleteAllocationAsync(int id)
        {
            var entity = await _context.ShiftAllocations.FindAsync(id);
            if (entity == null) return false;

            _context.ShiftAllocations.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ======================================================
        //              USER DETAILS
        // ======================================================

        public async Task<UserReadDto?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Where(x => x.UserId == userId)
                .Select(x => new UserReadDto
                {
                    UserID = x.UserId,
                    EmployeeCode = x.EmployeeCode,
                    FullName = x.FullName,
                    Email = x.Email,
                    CompanyID = x.CompanyId,    // Added
                    RegionID = x.RegionId,
                    Status = x.Status,
                    RoleName = x.RoleId.ToString(),
                    CreatedDate = (DateTime)x.CreatedDate
                }).FirstOrDefaultAsync();
        }
        public async Task<EmployeeShiftDto?> GetEmployeeShiftByEmployeeCodeAsync(string employeeCode)
        {
            return await (
                from s in _context.ShiftAllocations
                join sm in _context.ShiftMasters
                    on s.ShiftId equals sm.ShiftId
                where s.EmployeeCode == employeeCode
                select new EmployeeShiftDto
                {
                    ShiftName = sm.ShiftName,
                    ShiftStartTime = sm.ShiftStartTime,
                    ShiftEndTime = sm.ShiftEndTime
                }
            ).FirstOrDefaultAsync();
        }
        public async Task<EmployeeShiftDto?> GetEmployeeShiftByEmployeeCodeAsync(
      string employeeCode,
      int companyId,
      int regionId)
        {
            var result = await (
                from s in _context.ShiftAllocations.AsNoTracking()
                join sm in _context.ShiftMasters.AsNoTracking()
                    on s.ShiftId equals sm.ShiftId
                where s.EmployeeCode == employeeCode
                      && s.CompanyId == companyId
                      && s.RegionId == regionId
                orderby s.ShiftAllocationId descending   // latest allocation
                select new EmployeeShiftDto
                {
                    ShiftName = sm.ShiftName,
                    ShiftStartTime = sm.ShiftStartTime,
                    ShiftEndTime = sm.ShiftEndTime,
                    GrassTime = sm.GraceTime,   // latest value
                    allocationId = s.ShiftAllocationId
                }
            ).FirstOrDefaultAsync();

            return result;
        }
    }
}
