using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class LeaveService:ILeaveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly HRMSContext _context;
        private readonly INotificationService _notificationService;
        public LeaveService(IUnitOfWork unitOfWork, IEmailService emailService,
                            IConfiguration configuration, HRMSContext context, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;
        }
        public class LeaveReportRequest
        {
            public int CompanyId { get; set; }
            public int RegionId { get; set; }
            public int? UserId { get; set; } // NULL = All Employees
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string? Status { get; set; }

        }
        public class LeaveReportDto
        {
            public int LeaveRequestId { get; set; }
            public int UserId { get; set; }
            public string EmployeeName { get; set; }
            public string LeaveType { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalDays { get; set; }
            public string Status { get; set; }
        }


        public async Task<ApiResponse<List<LeaveReportDto>>> GetLeaveReport(LeaveReportRequest request)
        {
            var response = new ApiResponse<List<LeaveReportDto>>();

            try
            {
                if (request == null)
                    return new ApiResponse<List<LeaveReportDto>> { Success = false, Message = "Invalid request" };

                if (request.FromDate > request.ToDate)
                    return new ApiResponse<List<LeaveReportDto>> { Success = false, Message = "FromDate cannot be greater than ToDate" };

                var fromDate = DateOnly.FromDateTime(request.FromDate);
                var toDate = DateOnly.FromDateTime(request.ToDate);

                var query = from lr in _context.LeaveRequests
                            join u in _context.Users on lr.UserId equals u.UserId
                            where lr.CompanyId == request.CompanyId
                               && lr.RegionId == request.RegionId
                               && (lr.StartDate <= toDate && lr.EndDate >= fromDate)
                            select new
                            {
                                lr.LeaveRequestId,
                                lr.UserId,
                                EmployeeName = u.FullName,
                                lr.LeaveTypeId,
                                lr.StartDate,
                                lr.EndDate,
                                lr.TotalDays,
                                lr.Status
                            };

                if (request.UserId.HasValue && request.UserId.Value > 0)
                {
                    query = query.Where(x => x.UserId == request.UserId);
                }
                if (!string.IsNullOrEmpty(request.Status))
                {
                    query = query.Where(x => x.Status == request.Status);
                }

                var data = await query
                    .OrderByDescending(x => x.StartDate)
                    .ToListAsync();

                var result = data.Select(x => new LeaveReportDto
                {
                    LeaveRequestId = x.LeaveRequestId,
                    UserId = x.UserId,
                    EmployeeName = x.EmployeeName,
                    //LeaveType = x.LeaveTypeId.ToString(),
                    LeaveType = _context.LeaveTypes.Where(y => y.LeaveTypeId == x.LeaveTypeId).Select(y => y.LeaveTypeName)
    .FirstOrDefault(),
                    StartDate = x.StartDate.ToDateTime(TimeOnly.MinValue),
                    EndDate = x.EndDate.ToDateTime(TimeOnly.MinValue),
                    TotalDays = x.TotalDays,
                    Status = x.Status
                }).ToList();

                response.Success = true;
                response.Message = "Leave report fetched successfully";
                response.Data = result;

                return response;
            }
            catch (Exception)
            {
                return new ApiResponse<List<LeaveReportDto>>
                {
                    Success = false,
                    Message = "Something went wrong"
                };
            }
        }
        public async Task<IEnumerable<LeaveTypeDto>> GetActiveLeaveTypesAsync()
        {
            var data = await _unitOfWork.Repository<LeaveType>().GetAllAsync();

            return data
                .Where(x => x.IsActive == true && x.IsDeleted == false)
                .Select(x => new LeaveTypeDto
                {
                    LeaveTypeID = x.LeaveTypeId,
                    LeaveTypeName = x.LeaveTypeName,
                    LeaveDays = x.LeaveDays
                })
                .ToList();
        }
        public async Task<ReportingManagerDto> GetReportingManagerAsync(int userId)
        {
            // Get logged-in user
            var user = (await _unitOfWork.Repository<User>()
                .GetAllAsync())
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null)
                return new ReportingManagerDto();

            // Get manager using ReportingTo
            var manager = (await _unitOfWork.Repository<User>()
                .GetAllAsync())
                .FirstOrDefault(x => x.UserId == user.ReportingTo);

            return new ReportingManagerDto
            {
                UserId = user.UserId,
                EmployeeName = user.FullName,
                ManagerId = manager?.UserId,
                ManagerName = manager?.FullName,
                ManagerEmail = manager?.Email
            };
        }

        public async Task<IEnumerable<LeaveBalanceDto>> GetLeaveBalanceAsync(int userId)
        {
            // USER
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found");

            // DESIGNATION
            var designation = await _context.Designations
                .FirstOrDefaultAsync(x => x.DesignationId == user.DesignationId);

            if (designation == null)
                throw new Exception("Designation not mapped");

            // GRADE
            int? gradeId = designation.GradeId;

            if (gradeId == null)
                throw new Exception("Grade not mapped");

            // LEAVE TYPES FOR GRADE
            var leaveGrades = await (
                from ltg in _context.LeaveTypeGrades
                join lt in _context.LeaveTypes
                    on ltg.LeaveTypeId equals lt.LeaveTypeId
                where ltg.GradeId == gradeId
                      && lt.IsActive == true
                      && lt.IsDeleted == false
                select new
                {
                    lt.LeaveTypeId,
                    lt.LeaveTypeName,
                    ltg.LeaveDays
                }
            ).ToListAsync();

            var leaveRequests = await _context.LeaveRequests
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var result = leaveGrades.Select(x =>
            {
                var approved = leaveRequests
                    .Where(l => l.LeaveTypeId == x.LeaveTypeId
                             && l.Status == "Approved")
                    .Sum(l => l.TotalDays);

                var pending = leaveRequests
                    .Where(l => l.LeaveTypeId == x.LeaveTypeId
                             && l.Status == "Pending")
                    .Sum(l => l.TotalDays);

                var rejected = leaveRequests
                    .Where(l => l.LeaveTypeId == x.LeaveTypeId
                             && l.Status == "Rejected")
                    .Sum(l => l.TotalDays);

                return new LeaveBalanceDto
                {
                    LeaveTypeId = x.LeaveTypeId,
                    LeaveTypeName = x.LeaveTypeName,

                    AllocatedLeaves = x.LeaveDays,

                    ApprovedLeaves = approved,

                    PendingLeaves = pending,

                    RejectedLeaves = rejected,

                    RemainingLeaves = x.LeaveDays - approved - pending
                };
            }).ToList();

            return result;
        }

        public async Task<int> SubmitLeaveAsync(LeaveRequestDto dto)
        {
            var startDateOnly = DateOnly.FromDateTime(dto.StartDate);
            var endDateOnly = DateOnly.FromDateTime(dto.EndDate);

            // =====================================================
            // VALIDATE DATES
            // =====================================================

            if (dto.StartDate.Date > dto.EndDate.Date)
                throw new Exception("Start date cannot be greater than end date.");

            // =====================================================
            // DUPLICATE / OVERLAP CHECK
            // =====================================================

            var existingLeaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(x =>
                    x.UserId == dto.UserId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId);

            bool isDuplicate = existingLeaves.Any(l =>
            {
                if (l.Status == "Rejected")
                    return false;

                var existingStart = l.StartDate.ToDateTime(TimeOnly.MinValue);
                var existingEnd = l.EndDate.ToDateTime(TimeOnly.MinValue);

                return dto.StartDate.Date <= existingEnd.Date &&
                       dto.EndDate.Date >= existingStart.Date;
            });

            if (isDuplicate)
            {
                throw new Exception("You already applied leave for selected dates.");
            }

            // =====================================================
            // LOAD WEEKOFFS
            // =====================================================

            var weekoffs = await _unitOfWork.Repository<Weekoff>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId);

            // =====================================================
            // LOAD HOLIDAYS
            // =====================================================

            var holidays = await _unitOfWork.Repository<HolidayList>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId);

            // =====================================================
            // CHECK START DATE
            // =====================================================

            string startDay = dto.StartDate.DayOfWeek.ToString();

            bool isStartWeekoff = weekoffs.Any(x =>
                x.Weekoff1.Equals(startDay, StringComparison.OrdinalIgnoreCase));

            bool isStartHoliday = holidays.Any(x =>
                x.Date.HasValue &&
                x.Date.Value == startDateOnly);

            if (isStartWeekoff || isStartHoliday)
            {
                throw new Exception("Cannot apply leave on holiday/weekoff.");
            }

            // =====================================================
            // CHECK END DATE
            // =====================================================

            string endDay = dto.EndDate.DayOfWeek.ToString();

            bool isEndWeekoff = weekoffs.Any(x =>
                x.Weekoff1.Equals(endDay, StringComparison.OrdinalIgnoreCase));

            bool isEndHoliday = holidays.Any(x =>
                x.Date.HasValue &&
                x.Date.Value == endDateOnly);

            if (isEndWeekoff || isEndHoliday)
            {
                throw new Exception("Cannot apply leave on holiday/weekoff.");
            }

            // =====================================================
            // CALCULATE TOTAL DAYS
            // =====================================================

            decimal totalDays = 0;

            if (dto.IsHalfDay)
            {
                totalDays = 0.5m;
            }
            else
            {
                DateTime current = dto.StartDate.Date;

                while (current <= dto.EndDate.Date)
                {
                    string dayName = current.DayOfWeek.ToString();

                    bool isWeekoff = weekoffs.Any(x =>
                        x.Weekoff1.Equals(dayName, StringComparison.OrdinalIgnoreCase));

                    bool isHoliday = holidays.Any(x =>
                        x.Date.HasValue &&
                        x.Date.Value == DateOnly.FromDateTime(current));

                    if (!isWeekoff && !isHoliday)
                    {
                        totalDays++;
                    }

                    current = current.AddDays(1);
                }
            }

            // =====================================================
            // NO VALID DAYS
            // =====================================================

            if (totalDays <= 0)
            {
                throw new Exception("No working days available in selected range.");
            }

            // =====================================================
            // GET USER
            // =====================================================

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == dto.UserId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // =====================================================
            // GET DESIGNATION
            // =====================================================

            var designation = await _context.Designations
                .FirstOrDefaultAsync(x => x.DesignationId == user.DesignationId);

            if (designation == null)
            {
                throw new Exception("Designation not mapped.");
            }

            // =====================================================
            // GET GRADE
            // =====================================================

            int? gradeId = designation.GradeId;

            if (gradeId == null)
            {
                throw new Exception("Grade not mapped.");
            }

            // =====================================================
            // GET LEAVE ALLOCATION
            // =====================================================

            var leaveAllocation = await _context.LeaveTypeGrades
                .FirstOrDefaultAsync(x =>
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.GradeId == gradeId &&
                    x.IsActive == true);

            if (leaveAllocation == null)
            {
                throw new Exception("Leave allocation not configured.");
            }
            if (leaveAllocation == null)
            {
                throw new Exception("Leave allocation not configured.");
            }

            decimal allocatedLeaves = leaveAllocation.LeaveDays;

            // =====================================================
            // GET USED LEAVES
            // =====================================================

            var usedLeaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(x =>
                    x.UserId == dto.UserId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.LeaveTypeId == dto.LeaveTypeId &&
                    x.Status != "Rejected");

            decimal usedDays = usedLeaves.Sum(x => x.TotalDays);

            // =====================================================
            // BALANCE CHECK
            // =====================================================

            decimal remainingBalance = allocatedLeaves - usedDays;

            // Prevent negative balance issues
            if (remainingBalance < 0)
            {
                remainingBalance = 0;
            }

            // =====================================================
            // OVERRIDE FRONTEND VALUE
            // =====================================================

            totalDays = dto.TotalDays;

            // =====================================================
            // SAVE LEAVE
            // =====================================================

            var entity = new LeaveRequest
            {
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                LeaveTypeId = dto.LeaveTypeId,
                IsHalfDay = dto.IsHalfDay,

                StartDate = startDateOnly,
                EndDate = endDateOnly,

                TotalDays = dto.TotalDays,
                Lopdays = dto.Lopdays,

                Reason = dto.Reason,

                FileName = dto.FileName,
                FilePath = dto.FilePath,

                ReportingManagerId = dto.ReportingManagerId,

                Status = "Pending",

                AppliedDate = DateTime.Now,

                CreatedAt = DateTime.Now,
                CreatedBy = dto.UserId,

                HrEmail = dto.HrEmail
            };

            await _unitOfWork.Repository<LeaveRequest>().AddAsync(entity);

            await _unitOfWork.CompleteAsync();
            var notifyUsers = new List<int>();

            // Manager
            if (dto.ReportingManagerId.HasValue)
            {
                notifyUsers.Add(dto.ReportingManagerId.Value);
            }

            // Reporting HR ni DB nundi fetch cheyyi
            var employee = await _context.EmployeePersonalDetails
                .FirstOrDefaultAsync(x => x.UserId == dto.UserId);

            if (user.ReportingHr != null)
            {
                notifyUsers.Add(user.ReportingHr.Value);
            }

            await _notificationService.CreateNotificationAsync(
                notifyUsers,
                "Leave Request",
                $"{user.FullName} applied leave",
                "Leave",
                entity.LeaveRequestId);

            return entity.LeaveRequestId;
        }
        public async Task<IEnumerable<LeaveRequestDto>> GetMyLeavesAsync(int userId)
        {
            var leaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(x => x.UserId == userId);

            return leaves.Select(l => new LeaveRequestDto
            {
                LeaveRequestId = l.LeaveRequestId,
                LeaveTypeId = l.LeaveTypeId,
                LeaveTypeName = _unitOfWork.Repository<LeaveType>()
                            .GetAllAsync().Result
                            .FirstOrDefault(t => t.LeaveTypeId == l.LeaveTypeId)?.LeaveTypeName,

                // ✅ Send only DATE (no time)
                StartDate = l.StartDate.ToDateTime(TimeOnly.MinValue).Date,
                EndDate = l.EndDate.ToDateTime(TimeOnly.MinValue).Date,
                AppliedDate = l.AppliedDate.HasValue
                        ? l.AppliedDate.Value.Date
                        : null,

                TotalDays = l.TotalDays,
                Reason = l.Reason,
                FileName = l.FileName,
                FilePath = l.FilePath,
                Status = l.Status,
                IsHalfDay = (bool)l.IsHalfDay
            }).ToList();
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetLeavesForManagerAsync(int managerId)
        {
            var leaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(x => x.ReportingManagerId == managerId);

            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var leaveTypes = await _unitOfWork.Repository<LeaveType>().GetAllAsync();

            return leaves.Select(l => new LeaveRequestDto
            {
                LeaveRequestId = l.LeaveRequestId,
                UserId = l.UserId,
                EmployeeName = users.FirstOrDefault(u => u.UserId == l.UserId)?.FullName,
                LeaveTypeName = leaveTypes.FirstOrDefault(t => t.LeaveTypeId == l.LeaveTypeId)?.LeaveTypeName,
                StartDate = l.StartDate.ToDateTime(TimeOnly.MinValue).Date,
                EndDate = l.EndDate.ToDateTime(TimeOnly.MinValue).Date,
                IsHalfDay = l.IsHalfDay ?? false,
                TotalDays = (l.IsHalfDay ?? false) ? 0.5m : l.TotalDays,
                Reason = l.Reason,
                Status = l.Status,
                FileName = l.FileName,
                FilePath = l.FilePath
            });
        }
        public async Task<bool> ApproveLeaveFromEmailAsync(int leaveId)
        {
            var repo = _unitOfWork.Repository<LeaveRequest>();
            var leave = await repo.GetByIdAsync(leaveId);

            if (leave == null) return false;

            leave.Status = "Approved";
            leave.ApprovedRejectedDate = DateTime.Now;

            repo.Update(leave);
            await _unitOfWork.CompleteAsync();

            // ✅ Send email to employee
            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(leave.UserId);

            if (employee != null)
            {
                string subject = "Leave Approved";
                string body = $"Hello {employee.FullName},<br/><br/>Your leave has been <b>approved</b>.";

                await _emailService.SendEmailAsync(employee.Email, subject, body);
            }

            return true;
        }
        public async Task<bool> RejectLeaveFromEmailAsync(int leaveId)
        {
            var repo = _unitOfWork.Repository<LeaveRequest>();
            var leave = await repo.GetByIdAsync(leaveId);

            if (leave == null) return false;

            leave.Status = "Rejected";
            leave.ApprovedRejectedDate = DateTime.Now;

            repo.Update(leave);
            await _unitOfWork.CompleteAsync();

            // ✅ Send email to employee
            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(leave.UserId);

            if (employee != null)
            {
                string subject = "Leave Rejected";
                string body = $"Hello {employee.FullName},<br/><br/>Your leave has been <b>rejected</b>.";

                await _emailService.SendEmailAsync(employee.Email, subject, body);
            }

            return true;
        }
        public async Task SendLeaveEmailToManagerAsync(int leaveRequestId)
        {
            try
            {
                var leave = await _unitOfWork.Repository<LeaveRequest>()
                    .GetByIdAsync(leaveRequestId);

                if (leave == null || leave.ReportingManagerId == null)
                    return;

                var manager = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(leave.ReportingManagerId.Value);

                var employee = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(leave.UserId);
                string? reportingHrEmail = null;

                if (employee.ReportingHr.HasValue)
                {
                    var reportingHrUser = await _unitOfWork.Repository<User>()
                        .GetByIdAsync(employee.ReportingHr.Value);

                    reportingHrEmail = reportingHrUser?.Email;
                }

                var leaveType = await _unitOfWork.Repository<LeaveType>()
                    .GetByIdAsync(leave.LeaveTypeId);

                if (manager == null || employee == null)
                    return;

                string portalUrl = _configuration["AppSettings:PortalUrl"];

                //string approveUrl = $"{portalUrl}/api/Leave/ApproveFromEmail/{leaveRequestId}";
                //string rejectUrl = $"{portalUrl}/api/Leave/RejectFromEmail/{leaveRequestId}";
                string approveUrl = $"{_configuration["AppSettings:LoginUrl"]}";
                string rejectUrl = $"{_configuration["AppSettings:LoginUrl"]}";

                string subject = "New Leave Request";

                string body = $@"
        <html>
        <body style='font-family:Segoe UI'>
            <h3>Leave Request</h3>
            <p><b>Employee:</b> {employee.FullName}</p>
            <p><b>Leave Type:</b> {leaveType?.LeaveTypeName}</p>
            <p><b>From:</b> {leave.StartDate}</p>
            <p><b>To:</b> {leave.EndDate}</p>
            <p><b>Total Days:</b> {leave.TotalDays}</p>
            <p><b>Reason:</b> {leave.Reason}</p>
            <br/>
            <a href='{approveUrl}' style='padding:10px;background:green;color:white;text-decoration:none;'>Approve</a>
            &nbsp;
            <a href='{rejectUrl}' style='padding:10px;background:red;color:white;text-decoration:none;'>Reject</a>
        </body>
        </html>";

                // await _emailService.SendEmailAsync(manager.Email, subject, body);
                var ccEmails = new List<string>();

                // Reporting HR
                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccEmails.Add(reportingHrEmail);
                }

                // UI CC Emails
                if (!string.IsNullOrWhiteSpace(leave.HrEmail))
                {
                    ccEmails.AddRange(
                        leave.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                    );
                }

                ccEmails = ccEmails.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    manager.Email,
                    subject,
                    body,
                    ccEmails
                );
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        // ✅ SINGLE APPROVE
        public async Task<bool> ApproveLeaveByManagerAsync(int leaveId)
        {
            var repo = _unitOfWork.Repository<LeaveRequest>();
            var leave = await repo.GetByIdAsync(leaveId);
            if (leave == null) return false;

            leave.Status = "Approved";
            leave.ApprovedRejectedDate = DateTime.Now;

            repo.Update(leave);
            await _unitOfWork.CompleteAsync();
            // ================= NOTIFICATION =================

            var notificationUsers = new List<int>();

            // Employee
            notificationUsers.Add(leave.UserId);


            // Reporting HR
            var employeeUser = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == leave.UserId);


            if (employeeUser?.ReportingHr != null)
            {
                notificationUsers.Add(employeeUser.ReportingHr.Value);
            }


            notificationUsers = notificationUsers
                .Distinct()
                .ToList();


            await _notificationService.CreateNotificationAsync(
                notificationUsers,
                "Leave Request",
                $"{employeeUser?.FullName} leave request has been Approved by Manager.",
                "Leave",
                leave.LeaveRequestId
            );

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(leave.UserId);
            if (user != null)
            {
                string? reportingHrEmail = null;

                if (user.ReportingHr.HasValue)
                {
                    var reportingHrUser = await _unitOfWork.Repository<User>()
                        .GetByIdAsync(user.ReportingHr.Value);

                    reportingHrEmail = reportingHrUser?.Email;
                }

                var ccList = new List<string>();

                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccList.Add(reportingHrEmail);
                }

                if (!string.IsNullOrWhiteSpace(leave.HrEmail))
                {
                    ccList.AddRange(
                        leave.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                    );
                }

                ccList = ccList.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Leave Approved",
                    $"Hello {user.FullName},<br>Your leave has been <b>approved</b>.",
                    ccList
                );
            }


            if (!string.IsNullOrWhiteSpace(leave.HrEmail))
            {
                await _emailService.SendEmailAsync(
                    leave.HrEmail,
                    "Employee Leave Approved",
                    $"Employee {user?.FullName} leave has been <b>approved</b>.<br/>" +
                    $"From: {leave.StartDate}<br/>" +
                    $"To: {leave.EndDate}<br/>" +
                    $"Total Days: {leave.TotalDays}"
                );
            }

            return true;
        }

        // ✅ SINGLE REJECT
        public async Task<bool> RejectLeaveByManagerAsync(int leaveId)
        {
            var repo = _unitOfWork.Repository<LeaveRequest>();
            var leave = await repo.GetByIdAsync(leaveId);
            if (leave == null) return false;

            leave.Status = "Rejected";
            leave.ApprovedRejectedDate = DateTime.Now;

            repo.Update(leave);
            await _unitOfWork.CompleteAsync();
            // ================= NOTIFICATION =================

            var notificationUsers = new List<int>();

            // Employee
            notificationUsers.Add(leave.UserId);


            var employeeUser = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == leave.UserId);


            // Reporting HR
            if (employeeUser?.ReportingHr != null)
            {
                notificationUsers.Add(employeeUser.ReportingHr.Value);
            }


            notificationUsers = notificationUsers
                .Distinct()
                .ToList();



            await _notificationService.CreateNotificationAsync(
                notificationUsers,
                "Leave Request",
                $"{employeeUser?.FullName} leave request has been Rejected by Manager.",
                "Leave",
                leave.LeaveRequestId
            );

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(leave.UserId);
            if (user != null)
            {
                string? reportingHrEmail = null;

                if (user.ReportingHr.HasValue)
                {
                    var reportingHrUser = await _unitOfWork.Repository<User>()
                        .GetByIdAsync(user.ReportingHr.Value);

                    reportingHrEmail = reportingHrUser?.Email;
                }

                var ccList = new List<string>();

                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccList.Add(reportingHrEmail);
                }

                if (!string.IsNullOrWhiteSpace(leave.HrEmail))
                {
                    ccList.AddRange(
                        leave.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                    );
                }

                ccList = ccList.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Leave Rejected",
                    $"Hello {user.FullName},<br>Your leave has been <b>rejected</b>.",
                    ccList
                );
            }

            // ✅ HR Email
            if (!string.IsNullOrWhiteSpace(leave.HrEmail))
            {
                await _emailService.SendEmailAsync(
                    leave.HrEmail,
                    "Employee Leave Rejected",
                    $"Employee {user?.FullName} leave has been <b>rejected</b>.<br/>" +
                    $"From: {leave.StartDate}<br/>" +
                    $"To: {leave.EndDate}<br/>" +
                    $"Total Days: {(leave.IsHalfDay == true ? "0.5" : leave.TotalDays.ToString())}"
                );
            }

            return true;
        }

        // ✅ BULK APPROVE
        public async Task<bool> BulkApproveLeavesAsync(List<int> leaveIds)
        {
            foreach (var id in leaveIds)
                await ApproveLeaveByManagerAsync(id);

            return true;
        }

        // ✅ BULK REJECT
        public async Task<bool> BulkRejectLeavesAsync(List<int> leaveIds)
        {
            foreach (var id in leaveIds)
                await RejectLeaveByManagerAsync(id);

            return true;
        }
        public async Task<IEnumerable<LeaveRequestDto>> GetLeavesForUserAsync(int userId)
        {
            var leaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(x => x.UserId == userId);

            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var leaveTypes = await _unitOfWork.Repository<LeaveType>().GetAllAsync();

            return leaves.Select(l => new LeaveRequestDto
            {
                LeaveRequestId = l.LeaveRequestId,
                UserId = l.UserId,
                EmployeeName = users.FirstOrDefault(u => u.UserId == l.UserId)?.FullName,
                LeaveTypeName = leaveTypes.FirstOrDefault(t => t.LeaveTypeId == l.LeaveTypeId)?.LeaveTypeName,
                StartDate = l.StartDate.ToDateTime(TimeOnly.MinValue).Date,
                EndDate = l.EndDate.ToDateTime(TimeOnly.MinValue).Date,
                TotalDays = (decimal)l.TotalDays,
                Status = l.Status
            }).ToList();
        }

        public async Task<IEnumerable<LeaveRequestDto>> GetLeavesForManagerUserAsync(int managerId)
        {
            var employees = (await _unitOfWork.Repository<User>()
                .FindAsync(u => u.ReportingTo == managerId))
                .Select(e => e.UserId)
                .ToList();

            var leaves = await _unitOfWork.Repository<LeaveRequest>()
                .FindAsync(l => employees.Contains(l.UserId));

            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var leaveTypes = await _unitOfWork.Repository<LeaveType>().GetAllAsync();

            return leaves.Select(l => new LeaveRequestDto
            {
                LeaveRequestId = l.LeaveRequestId,
                UserId = l.UserId,
                EmployeeName = users.FirstOrDefault(u => u.UserId == l.UserId)?.FullName,
                LeaveTypeName = leaveTypes.FirstOrDefault(t => t.LeaveTypeId == l.LeaveTypeId)?.LeaveTypeName,
                StartDate = l.StartDate.ToDateTime(TimeOnly.MinValue).Date,
                EndDate = l.EndDate.ToDateTime(TimeOnly.MinValue).Date,
                TotalDays = (decimal)l.TotalDays,
                Status = l.Status
            }).ToList();
        }


    }

}
