using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class CompanyEventsService: ICompanyEventsService
    {

        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;

        public CompanyEventsService(HRMSContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IEnumerable<CompanyEventsDto>> GetAllEvents(int userId)
        {
           var data = await _context.CompanyEvents
           .Where(x => x.IsActive == true && x.UserId == userId)
            .Select(e => new CompanyEventsDto
            {
                Id = e.Id,
                CompanyId = e.CompanyId,
                RegionId = e.RegionId,

                EventTitle = e.EventTitle,
                EventDescription = e.EventDescription,

                EventDate = e.EventDate,

                StartTime = e.StartTime,
                EndTime = e.EndTime,

                MeetingLink = e.MeetingLink,
                EventLocation = e.EventLocation,

                EventType = e.EventType,

                IsMeeting = e.IsMeeting,

                DepartmentIds =
                    _context.CompanyEventDepartments
                    .Where(x => x.EventId == e.Id)
                    .Select(x => x.DepartmentId)
                    .ToList()
            })
            .ToListAsync();

          return data;
        }

        public async Task<IEnumerable<CompanyEventsDto>> GetDepartmentEvents( int departmentId)
        {
                    var data =
             from e in _context.CompanyEvents
             join ed in _context.CompanyEventDepartments
                 on e.Id equals ed.EventId
             where ed.DepartmentId == departmentId
                   && e.IsActive == true
             select new CompanyEventsDto
             {
                 Id = e.Id,
                 EventTitle = e.EventTitle,
                 EventDate = e.EventDate,
                 StartTime = e.StartTime,
                 EndTime = e.EndTime,
                 MeetingLink = e.MeetingLink,
                 EventLocation = e.EventLocation
             };

            return await data.ToListAsync();
            return await data.ToListAsync();
        }

        //public async Task<int> CreateEvent(CompanyEvent model)
        //{
        //    _context.CompanyEvents.Add(model);
        //    await _context.SaveChangesAsync();
        //    return model.Id;
        //}

        //public async Task<int> CreateEvent(CompanyEventsDto dto)
        //{
        //    var model = new CompanyEvent
        //    {
        //        CompanyId = dto.CompanyId,
        //        RegionId = dto.RegionId,

        //        EventTitle = dto.EventTitle,
        //        EventDescription = dto.EventDescription,

        //        EventDate = dto.EventDate,

        //        StartTime = dto.StartTime,
        //        EndTime = dto.EndTime,

        //        MeetingLink = dto.MeetingLink,
        //        EventLocation = dto.EventLocation,

        //        EventType = dto.EventType,

        //        IsMeeting = dto.IsMeeting,
        //        UserId = dto.CreatedBy,
        //        CreatedBy = dto.CreatedBy,

        //        IsActive = true
        //    };

        //    _context.CompanyEvents.Add(model);

        //    await _context.SaveChangesAsync();

        //    foreach (var deptId in dto.DepartmentIds)
        //    {
        //        _context.CompanyEventDepartments.Add(
        //            new CompanyEventDepartment
        //            {
        //                EventId = model.Id,
        //                DepartmentId = deptId
        //            });
        //    }

        //    await _context.SaveChangesAsync();

        //    return model.Id;
        //}

        public async Task<int> CreateEvent(CompanyEventsDto dto)
        {
            //=========================================
            // Save Event
            //=========================================

            var model = new CompanyEvent
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                EventTitle = dto.EventTitle,
                EventDescription = dto.EventDescription,
                EventDate = dto.EventDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingLink = dto.MeetingLink,
                EventLocation = dto.EventLocation,
                EventType = dto.EventType,
                IsMeeting = dto.IsMeeting,
                UserId = dto.CreatedBy,
                CreatedBy = dto.CreatedBy,
                IsActive = true
            };

            _context.CompanyEvents.Add(model);

            await _context.SaveChangesAsync();

            //=========================================
            // Save Departments
            //=========================================

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    _context.CompanyEventDepartments.Add(
                        new CompanyEventDepartment
                        {
                            EventId = model.Id,
                            DepartmentId = deptId
                        });
                }

                await _context.SaveChangesAsync();
            }

            //=========================================
            // Get Employee Emails
            //=========================================

            List<string> emails = new();

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=========================================
            // Send Email
            //=========================================

            if (emails.Any())
            {
                string subject = $"New Company Event - {dto.EventTitle}";

                string body = $@"
<html>

<body style='font-family:Calibri;'>

<h2 style='color:#0d6efd;'>Company Event Invitation</h2>

<p>Dear Employee,</p>

<p>A new company event has been scheduled.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
    <td><b>Event Title</b></td>
    <td>{dto.EventTitle}</td>
</tr>

<tr>
    <td><b>Event Type</b></td>
    <td>{dto.EventType}</td>
</tr>

<tr>
    <td><b>Event Date</b></td>
    <td>{dto.EventDate:dd-MMM-yyyy}</td>
</tr>

<tr>
    <td><b>Start Time</b></td>
    <td>{dto.StartTime}</td>
</tr>

<tr>
    <td><b>End Time</b></td>
    <td>{dto.EndTime}</td>
</tr>

<tr>
    <td><b>Location</b></td>
    <td>{dto.EventLocation}</td>
</tr>

<tr>
    <td><b>Meeting Link</b></td>
    <td>{dto.MeetingLink}</td>
</tr>

<tr>
    <td><b>Description</b></td>
    <td>{dto.EventDescription}</td>
</tr>

</table>

<br/>

<p>Please login to the HRMS Portal for more details.</p>

<br/>

Regards,<br/>

<b>HR Team</b>

</body>

</html>";

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                    }
                }
            }

            return model.Id;
        }

        //public async Task<int> UpdateEvent(CompanyEvent model)
        //{
        //    model.IsActive = true;
        //    _context.CompanyEvents.Update(model);
        //    await _context.SaveChangesAsync();
        //    return model.Id;
        //}


        //public async Task<int> UpdateEvent(CompanyEventsDto dto)
        //{
        //    var model = await _context.CompanyEvents
        //        .FirstOrDefaultAsync(x => x.Id == dto.Id);

        //    if (model == null)
        //        return 0;

        //    model.CompanyId = dto.CompanyId;
        //    model.RegionId = dto.RegionId;

        //    model.EventTitle = dto.EventTitle;
        //    model.EventDescription = dto.EventDescription;

        //    model.EventDate = dto.EventDate;

        //    model.StartTime = dto.StartTime;
        //    model.EndTime = dto.EndTime;

        //    model.MeetingLink = dto.MeetingLink;
        //    model.EventLocation = dto.EventLocation;

        //    model.EventType = dto.EventType;
        //    model.IsMeeting = dto.IsMeeting;

        //    model.IsActive = true;

        //    await _context.SaveChangesAsync();

        //    // delete old departments
        //    var oldDepartments = _context.CompanyEventDepartments
        //        .Where(x => x.EventId == dto.Id);

        //    _context.CompanyEventDepartments.RemoveRange(oldDepartments);

        //    // add new departments
        //    foreach (var deptId in dto.DepartmentIds)
        //    {
        //        _context.CompanyEventDepartments.Add(
        //            new CompanyEventDepartment
        //            {
        //                EventId = dto.Id,
        //                DepartmentId = deptId
        //            });
        //    }

        //    await _context.SaveChangesAsync();

        //    return dto.Id;
        //}
        public async Task<int> UpdateEvent(CompanyEventsDto dto)
        {
            //=========================================
            // Get Existing Event
            //=========================================

            var model = await _context.CompanyEvents
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (model == null)
                return 0;

            //=========================================
            // Update Event
            //=========================================

            model.CompanyId = dto.CompanyId;
            model.RegionId = dto.RegionId;

            model.EventTitle = dto.EventTitle;
            model.EventDescription = dto.EventDescription;

            model.EventDate = dto.EventDate;

            model.StartTime = dto.StartTime;
            model.EndTime = dto.EndTime;

            model.MeetingLink = dto.MeetingLink;
            model.EventLocation = dto.EventLocation;

            model.EventType = dto.EventType;
            model.IsMeeting = dto.IsMeeting;

            model.IsActive = true;

            await _context.SaveChangesAsync();

            //=========================================
            // Delete Old Departments
            //=========================================

            var oldDepartments = await _context.CompanyEventDepartments
                .Where(x => x.EventId == dto.Id)
                .ToListAsync();

            _context.CompanyEventDepartments.RemoveRange(oldDepartments);

            //=========================================
            // Add New Departments
            //=========================================

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    _context.CompanyEventDepartments.Add(
                        new CompanyEventDepartment
                        {
                            EventId = dto.Id,
                            DepartmentId = deptId
                        });
                }

                await _context.SaveChangesAsync();
            }

            //=========================================
            // Get Employee Emails
            //=========================================

            List<string> emails = new();

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=========================================
            // Send Email
            //=========================================

            if (emails.Any())
            {
                string subject = $"Updated Company Event - {dto.EventTitle}";

                string body = $@"
<html>

<body style='font-family:Calibri;'>

<h2 style='color:#0d6efd;'>Company Event Updated</h2>

<p>Dear Employee,</p>

<p>An existing company event has been updated.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
<td><b>Event Title</b></td>
<td>{dto.EventTitle}</td>
</tr>

<tr>
<td><b>Event Type</b></td>
<td>{dto.EventType}</td>
</tr>

<tr>
<td><b>Event Date</b></td>
<td>{dto.EventDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Start Time</b></td>
<td>{dto.StartTime}</td>
</tr>

<tr>
<td><b>End Time</b></td>
<td>{dto.EndTime}</td>
</tr>

<tr>
<td><b>Location</b></td>
<td>{dto.EventLocation}</td>
</tr>

<tr>
<td><b>Meeting Link</b></td>
<td>{dto.MeetingLink}</td>
</tr>

<tr>
<td><b>Description</b></td>
<td>{dto.EventDescription}</td>
</tr>

</table>

<br/>

<p>Please login to the HRMS Portal to view the updated event details.</p>

<br/>

Regards,<br/>

<b>HR Team</b>

</body>

</html>";

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                    }
                }
            }

            return dto.Id;
        }
        public async Task<int> DeleteEvent(int id)
        {
            var data = await _context.CompanyEvents.FindAsync(id);

            data.IsActive = false;

            await _context.SaveChangesAsync();

            return id;
        }
    }
}
