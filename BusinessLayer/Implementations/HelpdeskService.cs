using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Implementations
{
    public class HelpdeskService:IHelpdeskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly HRMSContext _hRMSContext;
        private readonly INotificationService _notificationService;

        public HelpdeskService(IUnitOfWork unitOfWork,
                               IEmailService emailService,
                               IConfiguration configuration,
                               HRMSContext hRMSContext,
                               INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _configuration = configuration;
            _hRMSContext = hRMSContext;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Priority>> GetActivePrioritiesAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<Priority>().GetAllAsync();

            return data
                .Where(x => x.CompanyId == companyId
                         && x.RegionId == regionId
                         && x.IsActive == true
                         && x.IsDeleted == false)
                .Select(x => new Priority
                {
                    PriorityId = x.PriorityId,
                    PriorityName = x.PriorityName,
                    Description = x.Description
                })
                .ToList();
        }
        public async Task<IEnumerable<HelpDeskCategory>> GetActivecategoryAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<HelpDeskCategory>().GetAllAsync();

            return data
                .Where(x => x.CompanyId == companyId
                         && x.RegionId == regionId
                         && x.IsActive == true
                         && x.IsDeleted == false)
                .Select(x => new HelpDeskCategory
                {
                    HelpDeskCategoryId = x.HelpDeskCategoryId,
                    HelpDeskCategoryName = x.HelpDeskCategoryName,
                    Description = x.Description
                })
                .ToList();
        }
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x => x.UserId == userId);

            var user = users.FirstOrDefault();
            if (user == null) return null;

            string? departmentName = null;

            if (user.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Repository<Department>()
                    .GetByIdAsync(user.DepartmentId.Value);

                if (dept != null)
                {
                    departmentName = dept.DepartmentName; // 🔴 change this to your real column
                }
            }

            return new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                DepartmentId = user.DepartmentId,
                DepartmentName = departmentName
            };
        }

        public async Task<int> SubmitTicketAsync(TicketRequestDto dto)
        {
            string ticketNumber = $"TK{DateTime.Now:yyyyMMddHHmmss}";

            var entity = new Ticket
            {
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                CategoryId = dto.CategoryId,
                Subject = dto.Subject,
                PriorityId = dto.PriorityId,
                Description = dto.Description,
                FileName = dto.FileName,
                FilePath = dto.FilePath,
                Status = "Open",
                TicketNumber = ticketNumber,
                CreatedAt = DateTime.Now,
                CreatedBy = dto.UserId,
                HrEmail = dto.HrEmail   // ✅ ADD THIS
            };

            await _unitOfWork.Repository<Ticket>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            var employee = await _hRMSContext.Users
    .FirstOrDefaultAsync(x => x.UserId == dto.UserId);


            var notificationUsers = new List<int>();


            // Reporting Manager
            if (employee?.ReportingTo.HasValue == true)
            {
                notificationUsers.Add(employee.ReportingTo.Value);
            }


            // Reporting HR
            if (employee?.ReportingHr.HasValue == true)
            {
                notificationUsers.Add(employee.ReportingHr.Value);
            }


            // Remove duplicate IDs
            notificationUsers = notificationUsers
                .Distinct()
                .ToList();


            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "New Ticket Raised",
                    $"{employee.FullName} raised a ticket - {entity.TicketNumber}",
                    "Helpdesk",
                    entity.TicketId
                );
            }
            return entity.TicketId;
        }

        public async Task SendTicketEmailToManagerAsync(int ticketId)
        {
            var ticket = await _unitOfWork.Repository<Ticket>()
                .GetByIdAsync(ticketId);

            if (ticket == null) return;

            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ticket.UserId);

            if (employee == null || employee.ReportingTo == null)
                return;

            string? reportingHrEmail = null;

            if (employee.ReportingHr.HasValue)
            {
                var reportingHrUser = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(employee.ReportingHr.Value);

                reportingHrEmail = reportingHrUser?.Email;
            }

            var manager = await _unitOfWork.Repository<User>()
                .GetByIdAsync(employee.ReportingTo.Value);

            if (manager == null) return;

            string subject = "New Helpdesk Ticket Raised";

            string body = $@"
            <html>
            <body style='font-family:Segoe UI'>
                <h3>New Ticket Raised</h3>
                <p><b>Employee:</b> {employee.FullName}</p>
                <p><b>Ticket No:</b> {ticket.TicketNumber}</p>
                <p><b>Subject:</b> {ticket.Subject}</p>
                <p><b>Description:</b> {ticket.Description}</p>
                <p><b>Status:</b> Open</p>
            </body>
            </html>";

            var ccList = new List<string>();

            // Reporting HR Email
            if (!string.IsNullOrWhiteSpace(reportingHrEmail))
            {
                ccList.Add(reportingHrEmail);
            }

            // CC Emails entered in UI
            if (!string.IsNullOrWhiteSpace(ticket.HrEmail))
            {
                ccList.AddRange(
                    ticket.HrEmail
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                );
            }

            // ✅ Send with CC
            await _emailService.SendEmailAsync(manager.Email, subject, body, ccList);
            //if (ccList != null && ccList.Any())
            //{
            //    foreach (var cc in ccList)
            //    {
            //        await _emailService.SendEmailAsync(cc, subject, body);
            //    }
            //}
        }

        public async Task<IEnumerable<object>> GetMyTicketsAsync(int userId)
        {
            var tickets = await _unitOfWork.Repository<Ticket>()
                .FindAsync(x => x.UserId == userId);

            var categories = await _unitOfWork.Repository<HelpDeskCategory>().GetAllAsync();
            var priorities = await _unitOfWork.Repository<Priority>().GetAllAsync();

            return tickets.Select(t => new
            {
                t.TicketId,
                t.TicketNumber,
                CategoryName = categories.FirstOrDefault(c => c.HelpDeskCategoryId == t.CategoryId)?.HelpDeskCategoryName,
                PriorityName = priorities.FirstOrDefault(p => p.PriorityId == t.PriorityId)?.PriorityName,
                t.Subject,
                t.Status,
                t.CreatedAt,
                t.FileName,
                t.FilePath
            }).OrderByDescending(x => x.CreatedAt).ToList();
        }
        public async Task<IEnumerable<ManagerTicketDto>> GetManagerTicketsAsync(int managerId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.ReportingTo == managerId);

            var userIds = users.Select(u => u.UserId).ToList();

            var tickets = await _unitOfWork.Repository<Ticket>()
              .FindAsync(t => userIds.Contains(t.UserId));

            var categories = await _unitOfWork.Repository<HelpDeskCategory>().GetAllAsync();
            var priorities = await _unitOfWork.Repository<Priority>().GetAllAsync();

            return tickets.Select(t => new ManagerTicketDto
            {
                TicketId = t.TicketId,
                TicketNumber = t.TicketNumber,
                EmployeeName = users.First(u => u.UserId == t.UserId).FullName,
                CategoryName = categories.First(c => c.HelpDeskCategoryId == t.CategoryId).HelpDeskCategoryName,
                Subject = t.Subject,
                Status = t.Status ?? "Pending",
                PriorityName = priorities.First(p => p.PriorityId == t.PriorityId).PriorityName,
                CreatedAt = t.CreatedAt,
                ManagerComments = t.ManagerComments
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
        }
        public async Task UpdateTicketStatusAsync(UpdateTicketStatusDto dto)
        {
            var ticket = await _unitOfWork.Repository<Ticket>()
                .GetByIdAsync(dto.TicketId);

            if (ticket == null) return;

            ticket.Status = dto.Status;
            ticket.ManagerComments = dto.ManagerComments;
            ticket.ApprovedBy = dto.ManagerId;
            ticket.ApprovedAt = DateTime.Now;
            ticket.ModifiedBy = dto.ManagerId;
            ticket.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Ticket>().Update(ticket);
            await _unitOfWork.CompleteAsync();
            var employee = await _hRMSContext.Users
        .FirstOrDefaultAsync(x => x.UserId == ticket.UserId);


            var notificationUsers = new List<int>();


            // Employee Notification
            if (ticket.UserId > 0)
            {
                notificationUsers.Add(ticket.UserId);
            }


            // Reporting HR Notification
            if (employee?.ReportingHr.HasValue == true)
            {
                notificationUsers.Add(employee.ReportingHr.Value);
            }


            notificationUsers = notificationUsers
                .Distinct()
                .ToList();


            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    $"Ticket {dto.Status}",
                    $"Your ticket {ticket.TicketNumber} has been {dto.Status}",
                    "Helpdesk",
                    ticket.TicketId
                );
            }

            // ✅ Send email to employee
            await SendTicketStatusEmailToEmployeeAsync(dto.TicketId);
        }
        public async Task SendTicketStatusEmailToEmployeeAsync(int ticketId)
        {
            var ticket = await _unitOfWork.Repository<Ticket>()
                .GetByIdAsync(ticketId);

            if (ticket == null) return;

            var employee = await _unitOfWork.Repository<User>()
                .GetByIdAsync(ticket.UserId);

            if (employee == null) return;

            string? reportingHrEmail = null;

            if (employee.ReportingHr.HasValue)
            {
                var reportingHrUser = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(employee.ReportingHr.Value);

                reportingHrEmail = reportingHrUser?.Email;
            }

            string subject = $"Helpdesk Ticket {ticket.Status}";
            string body = $@"
    <html>
    <body style='font-family:Segoe UI'>
        <h3>Your Ticket Has Been {ticket.Status}</h3>
        <p><b>Ticket No:</b> {ticket.TicketNumber}</p>
        <p><b>Subject:</b> {ticket.Subject}</p>
        <p><b>Status:</b> {ticket.Status}</p>
        <p><b>Manager Comments:</b> {ticket.ManagerComments}</p>
    </body>
    </html>";

            var ccList = new List<string>();

            // Reporting HR
            if (!string.IsNullOrWhiteSpace(reportingHrEmail))
            {
                ccList.Add(reportingHrEmail);
            }

            // UI CC Emails
            if (!string.IsNullOrWhiteSpace(ticket.HrEmail))
            {
                ccList.AddRange(
                    ticket.HrEmail
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))
                );
            }

            ccList = ccList.Distinct().ToList();

            await _emailService.SendEmailAsync(employee.Email, subject, body, ccList);
        }

        public async Task<IEnumerable<UserProfileDto>> GetEmployeesByManagerAsync(int managerId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.ReportingTo == managerId);

            return users.Select(u => new UserProfileDto
            {
                UserId = u.UserId,
                FullName = u.FullName
            }).ToList();
        }

        public async Task<IEnumerable<object>> GetAllTicketReportsAsync(int companyId, int regionId)
        {
            var tickets = await _unitOfWork.Repository<Ticket>()
                .FindAsync(x => x.CompanyId == companyId && x.RegionId == regionId);

            var users = await _unitOfWork.Repository<User>()
                .GetAllAsync();

            var categories = await _unitOfWork.Repository<HelpDeskCategory>()
                .GetAllAsync();

            var priorities = await _unitOfWork.Repository<Priority>()
                .GetAllAsync();

            return tickets.Select(t => new
            {
                t.TicketId,
                t.TicketNumber,
                Description = t.Description,
                Status = t.Status,
                Priority = priorities.FirstOrDefault(p => p.PriorityId == t.PriorityId)?.PriorityName,
                AssignedTo = users.FirstOrDefault(u => u.UserId == t.ApprovedBy)?.FullName,
                CreatedDate = t.CreatedAt,
                Category = categories.FirstOrDefault(c => c.HelpDeskCategoryId == t.CategoryId)?.HelpDeskCategoryName,
                EmployeeName = users.FirstOrDefault(u => u.UserId == t.UserId)?.FullName
            })
            .OrderByDescending(x => x.CreatedDate)
            .ToList();
        }

        public async Task<IEnumerable<object>> GetManagerTicketReportsAsync(int managerId)
        {
            var hr = _hRMSContext.Users
                .Where(x => x.UserId == managerId)
                .Select(x => new { x.UserId, x.Email })
                .FirstOrDefault();

            var employees = await _unitOfWork.Repository<User>()
                .FindAsync(x => x.ReportingTo == managerId);

            var employeeIds = employees.Select(x => x.UserId).ToList();

            var tickets = _hRMSContext.Tickets
                .Where(x => (employeeIds.Contains(x.UserId) || x.HrEmail == hr.Email))
                .ToList();

            var users = await _unitOfWork.Repository<User>().GetAllAsync();
            var categories = await _unitOfWork.Repository<HelpDeskCategory>().GetAllAsync();
            var priorities = await _unitOfWork.Repository<Priority>().GetAllAsync();

            return tickets.Select(t => new
            {
                t.TicketId,
                t.TicketNumber,
                Description = t.Description,
                Status = t.Status,
                Priority = priorities.FirstOrDefault(p => p.PriorityId == t.PriorityId)?.PriorityName,
                AssignedTo = users.FirstOrDefault(u => u.UserId == t.ApprovedBy)?.FullName,
                CreatedDate = t.CreatedAt,
                Category = categories.FirstOrDefault(c => c.HelpDeskCategoryId == t.CategoryId)?.HelpDeskCategoryName,
                EmployeeName = users.FirstOrDefault(u => u.UserId == t.UserId)?.FullName
            })
            .OrderByDescending(x => x.CreatedDate)
            .ToList();
        }


    }
}
