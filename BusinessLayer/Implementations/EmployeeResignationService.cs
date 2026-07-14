using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmployeeResignationService: IEmployeeResignationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;


        public EmployeeResignationService(IUnitOfWork unitOfWork, IEmailService emailService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        // ===================== OLD REQUIRED METHOD =====================
        public async Task<IEnumerable<EmployeeResignationDto>> GetAllResignationsAsync()
        {
            var list = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            return list.Select(MapToDto);
        }

        // ===================== OLD REQUIRED METHOD =====================
        public async Task<IEnumerable<EmployeeResignationDto>> SearchResignationsAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var all = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            var query = all.AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);
                if (value == null) continue;

                switch (name)
                {
                    case nameof(EmployeeResignation.EmployeeId):
                        query = query.Where(x => x.EmployeeId != null && x.EmployeeId.Contains(value.ToString()!));
                        break;

                    case nameof(EmployeeResignation.ResignationType):
                        query = query.Where(x => x.ResignationType != null && x.ResignationType.Contains(value.ToString()!));
                        break;

                    case nameof(EmployeeResignation.Status):
                        query = query.Where(x => x.Status != null &&
                            x.Status.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase));
                        break;

                    case nameof(EmployeeResignation.CompanyId):
                        query = query.Where(x => x.CompanyId == Convert.ToInt32(value));
                        break;

                    case nameof(EmployeeResignation.RegionId):
                        query = query.Where(x => x.RegionId == Convert.ToInt32(value));
                        break;
                }
            }

            return query.Select(MapToDto).ToList();
        }

        // ===================== OLD REQUIRED METHOD (Bulk Insert) =====================
        public async Task<IEnumerable<EmployeeResignation>> AddMultipleResignationsAsync(List<EmployeeResignationDto> dtos)
        {
            var entities = dtos.Select(dto => new EmployeeResignation
            {
                EmployeeId = dto.EmployeeId,
                ResignationType = dto.ResignationType,
                NoticePeriod = dto.NoticePeriod,
                LastWorkingDay = dto.LastWorkingDay,
                ResignationReason = dto.ResignationReason,
                Status = dto.Status,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.Now,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId = dto.UserId
            }).ToList();

            await _unitOfWork.Repository<EmployeeResignation>().AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();

            return entities;
        }

        // ===================== NEW FILTERED GET ALL =====================
        public async Task<IEnumerable<EmployeeResignationDto>> GetResignationsByCompanyRegionAsync(int companyId, int regionId)
        {
            var list = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();
            return list
                .Where(e => e.CompanyId == companyId && e.RegionId == regionId)
                .Select(MapToDto)
                .ToList();
        }

        // ===================== NEW FILTERED GET BY ID =====================
        public async Task<EmployeeResignationDto?> GetResignationByIdFilteredAsync(int id, int companyId, int regionId)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);

            if (entity == null) return null;
            if (entity.CompanyId != companyId || entity.RegionId != regionId) return null;

            return MapToDto(entity);
        }

        // ===================== GET BY ID =====================
        public async Task<EmployeeResignationDto?> GetResignationByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }
        private async Task<User?> GetManagerAsync(int userId)
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var employee = users.FirstOrDefault(u => u.UserId == userId);

            if (employee?.ReportingTo == null) return null;

            return users.FirstOrDefault(u => u.UserId == employee.ReportingTo);
        }
        private async Task<List<User>> GetHrUsersAsync(int companyId, int regionId)
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();

            return users
                .Where(u =>
                    u.CompanyId == companyId &&
                    u.RegionId == regionId &&
                    u.RoleId == 4 &&                 // HR ROLE
                    u.Status == "Active"             // ✅ already exists
                )
                .ToList();
        }


        // ===================== CREATE =====================
        public async Task<EmployeeResignationDto> AddResignationAsync(EmployeeResignationDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (!dto.UserId.HasValue || !dto.CompanyId.HasValue || !dto.RegionId.HasValue)
                throw new Exception("UserId, CompanyId and RegionId are required.");

            var existing = await _unitOfWork.Repository<EmployeeResignation>()
                .FindAsync(x =>
                    x.UserId == dto.UserId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.Status != "Rejected" &&
                    x.LastWorkingDay == dto.LastWorkingDay
                );

            if (existing.Any())
                throw new Exception("Resignation already exists for selected Last Working Day");
            var employeeUser = await _unitOfWork.Repository<User>()
    .GetByIdAsync(dto.UserId.Value);

            int? reportingHrId = employeeUser?.ReportingHr;
            string? reportingHrEmail = null;

            if (reportingHrId.HasValue)
            {
                var reportingHrUser = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(reportingHrId.Value);

                reportingHrEmail = reportingHrUser?.Email;
            }

            var entity = new EmployeeResignation
            {
                EmployeeId = dto.EmployeeId,
                ResignationType = dto.ResignationType,
                NoticePeriod = dto.NoticePeriod,
                LastWorkingDay = dto.LastWorkingDay,
                ResignationReason = dto.ResignationReason,
                Status = "Pending",
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                CompanyId = dto.CompanyId.Value,
                RegionId = dto.RegionId.Value,
                UserId = dto.UserId.Value,
                RoleId = dto.RoleId,

                ReportingHr = reportingHrId,
                HrEmail = reportingHrEmail
            };

            await _unitOfWork.Repository<EmployeeResignation>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            // ================= NOTIFICATION SECTION =================

            var employeeName = employeeUser?.FullName ?? dto.EmployeeId;

            var requestType = string.IsNullOrWhiteSpace(dto.ResignationType)
                                ? "Employee Exit"
                                : dto.ResignationType;


            var notificationTitle = $"{requestType} Request";

            var notificationMessage =
                $"{employeeName} has submitted a {requestType} request.";


            // Get Manager (needed for both notification and email)
            var manager = await GetManagerAsync(dto.UserId.Value);


            // ================= EMPLOYEE SUBMITTED =================
            // Employee creates resignation
            // Notify HR + Manager

            // ================= EMPLOYEE SUBMITTED =================

            if (int.TryParse(dto.CreatedBy, out int createdUserId)
                && createdUserId == dto.UserId.Value)
            {
                var notificationUsers = new List<int>();

                if (manager != null)
                {
                    notificationUsers.Add(manager.UserId);
                }

                if (reportingHrId.HasValue)
                {
                    notificationUsers.Add(reportingHrId.Value);
                }


                if (notificationUsers.Any())
                {
                    await _notificationService.CreateNotificationAsync(
                        notificationUsers,
                        notificationTitle,
                        notificationMessage,
                        "Employee Exit",
                        entity.ResignationId
                    );
                }
            }


            // ================= HR / MANAGER SUBMITTED =================

            else
            {
                string title = requestType;

                string message = requestType.ToLower() switch
                {
                    "resignation" => "You have been resigned.",
                    "termination" => "You have been terminated.",
                    "retirement" => "You have been retired.",
                    "voluntary retirement" => "You have been retired.",
                    _ => $"Your {requestType} process has been initiated."
                };

                await _notificationService.CreateNotificationAsync(
                    new List<int> { dto.UserId.Value },
                    title,
                    message,
                    "Employee Exit",
                    entity.ResignationId
                );
            }

            // ================= EMAIL SECTION =================

            if (manager == null || string.IsNullOrWhiteSpace(manager.Email))
                return MapToDto(entity);



            // UI CC emails


            List<string> finalCcList = new();

            if (!string.IsNullOrWhiteSpace(reportingHrEmail))
            {
                finalCcList.Add(reportingHrEmail);
            }

            // ================= SUBJECT =================
            var subject = GetEmailSubject(dto.ResignationType, dto.EmployeeId);

            // ================= BODY =================
            var body = BuildEmailBody(dto, manager.FullName);

            await _emailService.SendEmailAsync(
                manager.Email,
                subject,
                body,
                finalCcList
            );

            return MapToDto(entity);
        }
        private string GetEmailSubject(string type, string employeeId)
        {
            return type?.ToLower() switch
            {
                "resignation" => $"Resignation Request - {employeeId}",
                "termination" => $"Termination Notice - {employeeId}",
                "retirement" => $"Retirement Request - {employeeId}",
                "voluntary retirement" => $"Voluntary Retirement - {employeeId}",
                _ => $"Employee Exit Request - {employeeId}"
            };
        }
        private string BuildEmailBody(EmployeeResignationDto dto, string managerName)
        {
            var type = dto.ResignationType?.ToLower();

            string title = type switch
            {
                "resignation" => "Resignation Request Submitted",
                "termination" => "Employee Termination Notice",
                "retirement" => "Retirement Request Submitted",
                "voluntary retirement" => "Voluntary Retirement Request",
                _ => "Employee Exit Request"
            };

            string intro = type switch
            {
                "termination" =>
                    "An employee termination action has been initiated by HR.",

                "retirement" or "voluntary retirement" =>
                    "An employee retirement request has been submitted for processing.",

                _ =>
                    "A resignation request has been submitted by an employee."
            };

            return $@"
                <div style='font-family:Arial; font-size:14px; color:#333;'>

                    <h2 style='color:#2E86C1;'>{title}</h2>

                    <p>Dear <b>{managerName}</b>,</p>

                    <p>{intro}</p>

                    <div style='background:#f4f6f7; padding:12px; border-radius:6px;'>
                        <p><b>Employee Code:</b> {dto.EmployeeId}</p>
                        <p><b>Employee Separation Type:</b> {dto.ResignationType}</p>
                        <p><b>Notice Period:</b> {dto.NoticePeriod} days</p>
                        <p><b>Last Working Day:</b> {dto.LastWorkingDay:dd-MMM-yyyy}</p>
                        <p><b>Reason:</b> {dto.ResignationReason}</p>
                    </div>

                    <p style='margin-top:15px;'>
                        Please review this request in the HRMS system and take appropriate action.
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        <b>Cortracker HRMS System</b>
                    </p>

                </div>
                ";
        }


        // ===================== UPDATE =====================
        public async Task<EmployeeResignationDto> UpdateResignationAsync(int id, EmployeeResignationDto dto)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);
            if (entity == null) throw new Exception("Record not found.");

            if (entity.CompanyId != dto.CompanyId || entity.RegionId != dto.RegionId)
                throw new Exception("Not allowed to update this record.");

            entity.EmployeeId = dto.EmployeeId;
            entity.ResignationType = dto.ResignationType;
            entity.NoticePeriod = dto.NoticePeriod;
            entity.LastWorkingDay = dto.LastWorkingDay;
            entity.ResignationReason = dto.ResignationReason;
            entity.Status = dto.Status;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedAt = DateTime.UtcNow;
            if (entity.Status == "Approved by Manager")
                entity.ManagerApprovedDate = DateTime.UtcNow;
            if (entity.Status == "Rejected by Manager")
                entity.ManagerRejectedDate = DateTime.UtcNow;
            _unitOfWork.Repository<EmployeeResignation>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        // ===================== DELETE =====================
        public async Task<bool> DeleteResignationAsync(int id, int companyId, int regionId)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>().GetByIdAsync(id);

            if (entity == null) return false;
            if (entity.CompanyId != companyId || entity.RegionId != regionId) return false;

            _unitOfWork.Repository<EmployeeResignation>().Remove(entity);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ===================== MAPPER =====================
        private EmployeeResignationDto MapToDto(EmployeeResignation e)
        {
            return new EmployeeResignationDto
            {
                ResignationId = e.ResignationId,
                EmployeeId = e.EmployeeId,
                ResignationType = e.ResignationType,
                NoticePeriod = e.NoticePeriod,
                LastWorkingDay = e.LastWorkingDay,
                ResignationReason = e.ResignationReason,
                Status = e.Status,

                CompanyId = e.CompanyId,
                RegionId = e.RegionId,
                UserId = e.UserId,
                RoleId = e.RoleId,

                // Manager
                ManagerReason = e.ManagerReason,
                ManagerApprovedDate = e.ManagerApprovedDate,
                ManagerRejectedDate = e.ManagerRejectedDate,

                // HR
                HRReason = e.HrReason,
                HRApprovedDate = e.HrApprovedDate,
                HRRejectedDate = e.HrRejectedDate,

                // NEW
                HrEmail = e.HrEmail,
                ReportingHr = e.ReportingHr,

                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt,
                ModifiedBy = e.ModifiedBy,
                ModifiedAt = e.ModifiedAt
            };
        }

        public async Task<bool> UpdateResignationStatusAsync(
      int resignationId,
      string status,
      string? managerReason,
      bool isManagerApprove,
      bool isManagerReject,
          string? hrReason = null,
    bool isHRApprove = false,
    bool isHRReject = false)
        {
            var entity = await _unitOfWork.Repository<EmployeeResignation>()
                .GetByIdAsync(resignationId);

            if (entity == null) return false;

            entity.Status = status;
            entity.ManagerReason = managerReason;
            entity.ModifiedAt = DateTime.UtcNow;

            if (isManagerApprove)
                entity.ManagerApprovedDate = DateTime.UtcNow;

            if (isManagerReject)
                entity.ManagerRejectedDate = DateTime.UtcNow;
            if (isHRApprove)
            {
                entity.HrReason = hrReason;
                entity.HrApprovedDate = DateTime.UtcNow;
            }

            if (isHRReject)
            {
                entity.HrReason = hrReason;
                entity.HrRejectedDate = DateTime.UtcNow;
            }

            _unitOfWork.Repository<EmployeeResignation>().Update(entity);
            await _unitOfWork.CompleteAsync();
            // ================= NOTIFICATION SECTION =================

            var notificationUsers = new List<int>();

            var requestType = string.IsNullOrWhiteSpace(entity.ResignationType)
                ? "Employee Exit"
                : entity.ResignationType;

            string notificationTitle = "";
            string notificationMessage = "";


            // ================= MANAGER ACTION =================

            if (isManagerApprove || isManagerReject)
            {
                notificationTitle = $"{requestType} {status}";

                notificationMessage =
                    $"Employee {entity.EmployeeId} {requestType} request has been {status} by Manager.";

                // Employee
                if (entity.UserId.HasValue)
                    notificationUsers.Add(entity.UserId.Value);

                // Reporting HR
                if (entity.ReportingHr.HasValue)
                    notificationUsers.Add(entity.ReportingHr.Value);
            }


            // ================= HR ACTION =================

            else if (isHRApprove || isHRReject)
            {
                notificationTitle = $"{requestType} {status}";

                notificationMessage =
                    $"Employee {entity.EmployeeId} {requestType} request has been {status} by HR.";

                // Employee
                if (entity.UserId.HasValue)
                    notificationUsers.Add(entity.UserId.Value);

                // Manager
                if (entity.UserId.HasValue)
                {
                    var manager = await GetManagerAsync(entity.UserId.Value);

                    if (manager != null)
                    {
                        notificationUsers.Add(manager.UserId);
                    }
                }
            }


            // Remove duplicate users
            notificationUsers = notificationUsers
                .Distinct()
                .ToList();

            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    notificationTitle,
                    notificationMessage,
                    "Employee Exit",
                    resignationId
                );
            }

            // send email
            if (entity.UserId.HasValue)
            {
                var employee = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(entity.UserId.Value);

                if (employee != null && !string.IsNullOrWhiteSpace(employee.Email))
                {
                    List<User> hrUsers = new();
                    if (entity.CompanyId.HasValue && entity.RegionId.HasValue)
                    {
                        hrUsers = await GetHrUsersAsync(entity.CompanyId.Value, entity.RegionId.Value);
                    }

                    var finalCcList = new List<string>();

                    // ✅ HR Users
                    if (hrUsers != null && hrUsers.Any())
                    {
                        finalCcList.AddRange(
                            hrUsers
                            .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                            .Select(x => x.Email)
                        );
                    }

                    // ✅ UI Entered CC (IMPORTANT FIX)
                    if (!string.IsNullOrWhiteSpace(entity.HrEmail))
                    {
                        var uiCc = entity.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim());

                        finalCcList.AddRange(uiCc);
                    }

                    // ✅ Remove duplicates
                    finalCcList = finalCcList.Distinct().ToList();

                    string actionBy = "";
                    string comments = "";

                    if (isManagerApprove)
                    {
                        actionBy = "Manager";
                        comments = managerReason ?? "";
                    }
                    else if (isManagerReject)
                    {
                        actionBy = "Manager";
                        comments = managerReason ?? "";
                    }
                    else if (isHRApprove)
                    {
                        actionBy = "HR";
                        comments = hrReason ?? "";
                    }
                    else if (isHRReject)
                    {
                        actionBy = "HR";
                        comments = hrReason ?? "";
                    }

                    string actionStatus =
                        (isManagerApprove || isHRApprove)
                            ? "Approved"
                            : "Rejected";

                    var subject = $"Resignation {actionStatus} - {entity.EmployeeId}";

                          var body = $@"
                            <p>Dear {employee.FullName},</p>

                            <p>
                            This is to inform you that your resignation request has been
                            <b>{actionStatus}</b> by the <b>{actionBy}</b>.
                            </p>

                            <br/>

                            <p><b>Resignation Details</b></p>

                            <p>
                            <b>Employee Code:</b> {entity.EmployeeId}<br/>
                            <b>Resignation Type:</b> {entity.ResignationType}<br/>
                            <b>Notice Period:</b> {entity.NoticePeriod} Days<br/>
                            <b>Last Working Day:</b> {entity.LastWorkingDay:dd-MMM-yyyy}<br/>
                            <b>Current Status:</b> {entity.Status}
                            </p>

                            {(!string.IsNullOrWhiteSpace(comments)
                                                ? $@"<br/>
                            <p><b>{actionBy} Comments:</b></p>
                            <p>{comments}</p>"
                                                : "")}

                            <br/>

                            <p>
                            Please login to <b>HRMS</b> for further details and any required actions.
                            </p>

                            <br/>

                            <p>
                            Thank you for your contributions and cooperation throughout the process.
                            </p>

                            <br/>

                            <p>
                            Regards,<br/>
                            <b>Cortracker HRMS Team</b>
                            </p>

                            <hr/>

                            <p style='font-size:12px;color:#777;'>
                            This is an automated email from HRMS. Please do not reply to this email.
                            </p>";

                    await _emailService.SendEmailAsync(employee.Email, subject, body, finalCcList);
                }
            }

            return true;
        }


        public async Task<IEnumerable<EmployeeResignationDto>> GetResignationsForReportingManagerAsync(int managerUserId)
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var resignations = await _unitOfWork.Repository<EmployeeResignation>().GetAllAsync();

            var result =
                (from r in resignations
                 join u in users on r.UserId equals u.UserId

                 where
                    u.ReportingTo == managerUserId
                    || u.ReportingHr == managerUserId

                 orderby r.Status == "Pending" ? 0 : 1, r.CreatedAt descending

                 select new EmployeeResignationDto
                 {
                     ResignationId = r.ResignationId,
                     EmployeeId = u.EmployeeCode,
                     ResignationType = r.ResignationType,
                     NoticePeriod = r.NoticePeriod,
                     LastWorkingDay = r.LastWorkingDay,
                     ResignationReason = r.ResignationReason,
                     Status = r.Status,
                     ManagerReason = r.ManagerReason
                 }).ToList();

            return result;
        }
        public async Task<IEnumerable<EmployeeResignationDto>> GetResignationsForHRAsync(int companyId, int regionId)
        {
            var resignations = await _unitOfWork.Repository<EmployeeResignation>()
                .FindAsync(r =>
                    r.CompanyId == companyId &&
                    r.RegionId == regionId &&
                    (
                        r.Status == "Approved" ||          // Manager approved
                        r.Status == "Rejected" ||          // (optional)
                        r.Status == "HR Approved" ||
                        r.Status == "HR Rejected" 
                    )
                );

            return resignations
                .OrderByDescending(r => r.CreatedAt)
                .Select(MapToDto);
        }
    }
}
