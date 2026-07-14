using Azure.Core;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class TimesheetService:ITimesheetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly HRMSContext _hRMSContext;
        private readonly INotificationService _notificationService;

        public TimesheetService(IUnitOfWork unitOfWork, IEmailService emailService, HRMSContext hRMSContext, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hRMSContext = hRMSContext;
            _notificationService = notificationService;
        }

        public async Task<LoggedInUserDto> GetLoggedInUserAsync(int userId)
        {
            var user = (await _unitOfWork.Repository<User>().GetAllAsync())
                .FirstOrDefault(x => x.UserId == userId);

            if (user == null)
                return new LoggedInUserDto();

            return new LoggedInUserDto
            {
                UserId = user.UserId,
                EmployeeName = user.FullName,
                EmployeeCode = user.EmployeeCode
            };
        }

        public async Task<int> SaveTimesheetAsync(TimesheetRequestDto dto)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(dto.UserId);

            var timesheet = new Timesheet
            {
                UserId = dto.UserId,
                ManagerUserId = user?.ReportingTo,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                EmployeeCode = dto.EmployeeCode,
                EmployeeName = dto.EmployeeName,
                TimesheetDate = DateOnly.FromDateTime(dto.TimesheetDate),
                Comments = dto.Comments,
                FileName = dto.FileName ?? "",
                FilePath = dto.FilePath,
                Status = "Pending",
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now,
                HrEmail = dto.HrEmail
            };

            await _unitOfWork.Repository<Timesheet>().AddAsync(timesheet);
            await _unitOfWork.CompleteAsync();

            foreach (var p in dto.Projects)
            {
                var project = new TimesheetProject
                {
                    TimesheetId = timesheet.TimesheetId,
                    ProjectName = p.ProjectName,
                   Description = p.Description ?? "",
                    StartTime = TimeOnly.Parse(p.StartTime),
                    EndTime = TimeOnly.Parse(p.EndTime),

                    TotalMinutes = p.TotalMinutes,
                    TotalHoursText = p.TotalHoursText ?? "0 Hours",

                    Otminutes = p.OTMinutes.HasValue && p.OTMinutes.Value >= 0 ? p.OTMinutes.Value : 0,
                    OthoursText = p.OTHoursText ?? "0 Hours",

                    CreatedAt = DateTime.Now
                };


                await _unitOfWork.Repository<TimesheetProject>().AddAsync(project);
            }

            await _unitOfWork.CompleteAsync();
            return timesheet.TimesheetId;
        }

        public async Task<int> UpdateTimesheetAsync(TimesheetRequestDto dto)
        {
            var timesheet = await _unitOfWork.Repository<Timesheet>()
                .GetByIdAsync(dto.TimesheetId);

            if (timesheet == null)
                throw new Exception("Timesheet not found");

            if (timesheet.Status != "Pending")
                throw new Exception("Only pending timesheets can be edited");

            // Update header
            timesheet.TimesheetDate = DateOnly.FromDateTime(dto.TimesheetDate);
            timesheet.Comments = dto.Comments;
            timesheet.HrEmail = dto.HrEmail;

            if (!string.IsNullOrWhiteSpace(dto.FileName))
            {
                timesheet.FileName = dto.FileName;
                timesheet.FilePath = dto.FilePath;
            }

            timesheet.ModifiedBy = dto.UserId;
            timesheet.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Timesheet>().Update(timesheet);

            // Delete existing projects
            var oldProjects = _hRMSContext.TimesheetProjects
    .Where(x => x.TimesheetId == dto.TimesheetId)
    .ToList();

            _hRMSContext.TimesheetProjects.RemoveRange(oldProjects);

            await _hRMSContext.SaveChangesAsync();


            // Insert latest projects
            foreach (var p in dto.Projects)
            {
                var project = new TimesheetProject
                {
                    TimesheetId = dto.TimesheetId,

                    ProjectName = p.ProjectName,
                    Description = p.Description ?? "",

                    StartTime = TimeOnly.Parse(p.StartTime),
                    EndTime = TimeOnly.Parse(p.EndTime),

                    TotalMinutes = p.TotalMinutes,
                    TotalHoursText = p.TotalHoursText ?? "0 Hours",

                    Otminutes = p.OTMinutes ?? 0,
                    OthoursText = p.OTHoursText ?? "0 Hours",

                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<TimesheetProject>()
                    .AddAsync(project);
            }

            await _unitOfWork.CompleteAsync();

            return dto.TimesheetId;
        }

        public async Task<IEnumerable<TimesheetListDto>> GetMyTimesheetsAsync(int userId)
        {
            var timesheets = await _unitOfWork.Repository<Timesheet>()
                .FindAsync(x => x.UserId == userId);

            var timesheetIds = timesheets.Select(t => t.TimesheetId).ToList();

            var projects = await _unitOfWork.Repository<TimesheetProject>()
                .FindAsync(p => timesheetIds.Contains(p.TimesheetId));

            return timesheets.Select(t => new TimesheetListDto
            {
                TimesheetId = t.TimesheetId,
                EmployeeName = t.EmployeeName,
                EmployeeCode = t.EmployeeCode,
                TimesheetDate = t.TimesheetDate.ToDateTime(TimeOnly.MinValue),

                Comments = t.Comments,
                HrEmail = t.HrEmail,

                FileName = t.FileName,
                FilePath = t.FilePath,

                Status = t.Status,

                Projects = projects
                    .Where(p => p.TimesheetId == t.TimesheetId)
                    .Select(p => new TimesheetProjectDto
                    {
                        ProjectName = p.ProjectName,
                        Description = p.Description,
                        //StartTime = p.StartTime.ToString(),
                        //EndTime = p.EndTime.ToString(),
                        StartTime = p.StartTime.ToString("HH:mm"),
                        EndTime = p.EndTime.ToString("HH:mm"),
                        TotalMinutes = p.TotalMinutes,
                        TotalHoursText = p.TotalHoursText,
                        OTMinutes = p.Otminutes,
                        OTHoursText = string.IsNullOrEmpty(p.OthoursText) ? "0 Hours" : p.OthoursText

                    }).ToList()
            });
        }

        // ✅ SEND SELECTED TIMESHEETS + EMAIL MANAGER
        public async Task<bool> SendSelectedTimesheetsAsync(List<int> timesheetIds)
        {
            var timesheets = await _unitOfWork.Repository<Timesheet>()
                .FindAsync(x => timesheetIds.Contains(x.TimesheetId));

            foreach (var ts in timesheets)
            {
                ts.Status = "Submitted";
                ts.ModifiedAt = DateTime.Now;
                await SendTimesheetNotificationAsync(ts);

                await SendManagerEmailAsync(ts);
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
        // =====================================
        // 🔔 TIMESHEET SUBMIT NOTIFICATION
        // =====================================
        private async Task SendTimesheetNotificationAsync(Timesheet ts)
        {
            var notificationUsers = new List<int>();


            // Get Employee
            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ts.UserId);


            if (employee == null)
                return;



            // Manager Notification
            if (ts.ManagerUserId.HasValue)
            {
                notificationUsers.Add(ts.ManagerUserId.Value);
            }



            // Reporting HR Notification
            if (employee.ReportingHr.HasValue)
            {
                notificationUsers.Add(employee.ReportingHr.Value);
            }



            // HR Emails from UI
            if (!string.IsNullOrWhiteSpace(ts.HrEmail))
            {
                var hrEmails = ts.HrEmail
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();


                var hrUserIds = await _hRMSContext.Users
                    .Where(x =>
                        hrEmails.Contains(x.Email) &&
                        x.CompanyId == ts.CompanyId &&
                        x.RegionId == ts.RegionId
                    )
                    .Select(x => x.UserId)
                    .ToListAsync();


                notificationUsers.AddRange(hrUserIds);
            }



            notificationUsers = notificationUsers
                .Distinct()
                .ToList();



            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Timesheet Submitted",
                    $"{ts.EmployeeName} submitted timesheet for {ts.TimesheetDate:dd-MMM-yyyy}.",
                    "Timesheet",
                    ts.TimesheetId
                );
            }
        }

        // ✅ EMAIL MANAGER LOGIC
        private async Task SendManagerEmailAsync(Timesheet ts)
        {
            if (!ts.ManagerUserId.HasValue)
                return;

            var manager = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ts.ManagerUserId.Value);

            if (manager == null || string.IsNullOrEmpty(manager.Email))
                return;

            var employee = await _unitOfWork.Repository<User>()
    .GetByIdAsync(ts.UserId);

            if (employee == null)
                return;

            string? reportingHrEmail = null;

            if (employee.ReportingHr.HasValue)
            {
                var reportingHrUser = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(employee.ReportingHr.Value);

                reportingHrEmail = reportingHrUser?.Email;
            }

            string subject = $"Timesheet Submitted - {ts.EmployeeName}";

            string body = $@"
    <html>
    <body style='font-family:Segoe UI'>
        <h3>Timesheet Submitted</h3>
        <p><b>Employee:</b> {ts.EmployeeName} ({ts.EmployeeCode})</p>
        <p><b>Date:</b> {ts.TimesheetDate:dd-MMM-yyyy}</p>
        <p><b>Status:</b> Submitted</p>
        <p><b>Comments:</b> {ts.Comments}</p>
        <hr/>
        <p>Please login to HRMS to review the timesheet.</p>
    </body>
    </html>";

            // ✅ CC LIST
            var ccList = new List<string>();

            // Reporting HR Email
            if (!string.IsNullOrWhiteSpace(reportingHrEmail))
            {
                ccList.Add(reportingHrEmail);
            }

            // CC Email entered in UI
            if (!string.IsNullOrWhiteSpace(ts.HrEmail))
            {
                ccList.AddRange(
                    ts.HrEmail
                      .Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => x.Trim())
                      .Where(x => !string.IsNullOrEmpty(x))
                );
            }

            try
            {
                await _emailService.SendEmailAsync(
                    manager.Email,
                    subject,
                    body,
                    ccList
                );

                Console.WriteLine("Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email FAILED: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ManagerTimesheetDto>> GetTimesheetsForManagerAsync(int managerUserId)
        {
            try
            {
                var hr = _hRMSContext.Users.Select(x => new { x.UserId, x.Email }).Where(x => x.UserId == managerUserId).ToList();

                // var timesheets = await _unitOfWork.Repository<Timesheet>().FindAsync(x => x.ManagerUserId == managerUserId);
                var timesheets = _hRMSContext.Timesheets.Select(x => new { x.TimesheetId, x.ManagerUserId, x.UserId, x.EmployeeName, x.EmployeeCode, x.TimesheetDate, x.Status, x.Comments, x.HrEmail })
                    .Where(x => x.ManagerUserId == managerUserId || x.HrEmail == hr.FirstOrDefault().Email).ToList();


                var timesheetIds = timesheets.Select(t => t.TimesheetId).ToList();

                var projects = await _unitOfWork.Repository<TimesheetProject>()
                    .FindAsync(p => timesheetIds.Contains(p.TimesheetId));

                return timesheets.Select(t => new ManagerTimesheetDto
                {
                    TimesheetId = t.TimesheetId,
                    UserId = t.UserId,
                    EmployeeName = t.EmployeeName,
                    EmployeeCode = t.EmployeeCode,
                    TimesheetDate = t.TimesheetDate.ToDateTime(TimeOnly.MinValue),
                    Status = t.Status,
                    Comments = t.Comments,

                    Projects = projects
                        .Where(p => p.TimesheetId == t.TimesheetId)
                        .Select(p => new TimesheetProjectDto
                        {
                            ProjectName = p.ProjectName,
                            Description = p.Description,
                            StartTime = p.StartTime.ToString(),
                            EndTime = p.EndTime.ToString(),
                            TotalMinutes = p.TotalMinutes,
                            TotalHoursText = p.TotalHoursText,
                            OTMinutes = p.Otminutes,
                            OTHoursText = p.OthoursText ?? "0 Hours"
                        }).ToList()
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ManagerTimesheetDto> GetTimesheetDetailAsync(int timesheetId)
        {
            var ts = await _unitOfWork.Repository<Timesheet>()
                .GetByIdAsync(timesheetId);

            if (ts == null) return null;

            var projects = await _unitOfWork.Repository<TimesheetProject>()
                .FindAsync(p => p.TimesheetId == timesheetId);
            var timesheetRequests = await _unitOfWork.Repository<Timesheet>()
     .FindAsync(r => r.TimesheetId == timesheetId);

            return new ManagerTimesheetDto
            {
                TimesheetId = ts.TimesheetId,
                UserId = ts.UserId,
                EmployeeName = ts.EmployeeName,
                EmployeeCode = ts.EmployeeCode,
                TimesheetDate = ts.TimesheetDate.ToDateTime(TimeOnly.MinValue),
                Status = ts.Status,
                Comments = ts.Comments,
                Projects = projects.Select(p => new TimesheetProjectDto
                {
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    //StartTime = p.StartTime.ToString(),
                    //EndTime = p.EndTime.ToString(),
                    StartTime = p.StartTime.ToString("HH:mm"),
                    EndTime = p.EndTime.ToString("HH:mm"),
                    TotalMinutes = p.TotalMinutes,
                    TotalHoursText = p.TotalHoursText,
                    OTMinutes = p.Otminutes,
                    OTHoursText = p.OthoursText
                }).ToList(),
                Requests = timesheetRequests
                .Select(r => new TimesheetRequestDto
                {
                    FileName = r.FileName,
                    FilePath = r.FilePath,

                }).ToList()
            };
        }
        public async Task<bool> ApproveTimesheetsAsync(List<int> ids, string comments)
        {
            var timesheets = await _unitOfWork.Repository<Timesheet>()
                .FindAsync(x => ids.Contains(x.TimesheetId));

            foreach (var ts in timesheets)
            {
                ts.Status = "Approved";
                ts.Comments = comments;
                ts.ModifiedAt = DateTime.Now;
                await SendTimesheetStatusNotificationAsync(ts, "Approved");

                await SendEmployeeStatusEmailAsync(ts, "Approved");
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RejectTimesheetsAsync(List<int> ids, string comments)
        {
            var timesheets = await _unitOfWork.Repository<Timesheet>()
                .FindAsync(x => ids.Contains(x.TimesheetId));

            foreach (var ts in timesheets)
            {
                ts.Status = "Rejected";
                ts.Comments = comments;
                ts.ModifiedAt = DateTime.Now;
                await SendTimesheetStatusNotificationAsync(ts, "Rejected");

                await SendEmployeeStatusEmailAsync(ts, "Rejected");
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
        private async Task SendTimesheetStatusNotificationAsync(
    Timesheet ts,
    string status)
        {
            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ts.UserId);

            if (employee == null)
                return;


            var notificationUsers = new List<int>();


            // Employee Notification
            notificationUsers.Add(ts.UserId);



            // Reporting HR Notification
            if (employee.ReportingHr.HasValue)
            {
                notificationUsers.Add(employee.ReportingHr.Value);
            }



            // HR Email users from UI
            if (!string.IsNullOrWhiteSpace(ts.HrEmail))
            {
                var hrEmails = ts.HrEmail
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();


                var hrUsers = await _unitOfWork.Repository<User>()
                    .FindAsync(x =>
                        hrEmails.Contains(x.Email) &&
                        x.CompanyId == ts.CompanyId &&
                        x.RegionId == ts.RegionId
                    );


                notificationUsers.AddRange(
                    hrUsers.Select(x => x.UserId)
                );
            }



            notificationUsers = notificationUsers
                .Distinct()
                .ToList();



            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Timesheet",
                    $"Your timesheet for {ts.TimesheetDate:dd-MMM-yyyy} has been {status} by Manager.",
                    "Timesheet",
                    ts.TimesheetId
                );
            }
        }
        private async Task SendEmployeeStatusEmailAsync(Timesheet ts, string status)
        {
            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ts.UserId);

            if (employee == null || string.IsNullOrEmpty(employee.Email))
                return;

            string? reportingHrEmail = null;

            if (employee.ReportingHr.HasValue)
            {
                var reportingHrUser = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(employee.ReportingHr.Value);

                reportingHrEmail = reportingHrUser?.Email;
            }

            string subject = $"Timesheet {status} - {ts.TimesheetDate:dd-MMM-yyyy}";

            string body = $@"
    <html>
    <body style='font-family:Segoe UI'>
        <h3>Your Timesheet Has Been {status}</h3>
        <p><b>Employee:</b> {ts.EmployeeName} ({ts.EmployeeCode})</p>
        <p><b>Date:</b> {ts.TimesheetDate:dd-MMM-yyyy}</p>
        <p><b>Status:</b> {status}</p>
        <p><b>Manager Comments:</b> {ts.Comments}</p>
        <hr/>
        <p>Please login to HRMS for details.</p>
    </body>
    </html>";
            var ccList = new List<string>();

            // Reporting HR
            if (!string.IsNullOrWhiteSpace(reportingHrEmail))
            {
                ccList.Add(reportingHrEmail);
            }

            // UI CC Emails
            if (!string.IsNullOrWhiteSpace(ts.HrEmail))
            {
                ccList.AddRange(
                    ts.HrEmail
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                );
            }

            // Remove duplicates
            ccList = ccList.Distinct().ToList();


            await _emailService.SendEmailAsync(employee.Email, subject, body,ccList);
        }

    }
}
