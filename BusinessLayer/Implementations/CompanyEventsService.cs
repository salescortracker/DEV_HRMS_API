using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class CompanyEventsService: ICompanyEventsService
    {

        private readonly HRMSContext _context;

        public CompanyEventsService(HRMSContext context)
        {
            _context = context;
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

        public async Task<int> CreateEvent(CompanyEventsDto dto)
        {
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

            return model.Id;
        }

        //public async Task<int> UpdateEvent(CompanyEvent model)
        //{
        //    model.IsActive = true;
        //    _context.CompanyEvents.Update(model);
        //    await _context.SaveChangesAsync();
        //    return model.Id;
        //}


        public async Task<int> UpdateEvent(CompanyEventsDto dto)
        {
            var model = await _context.CompanyEvents
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (model == null)
                return 0;

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

            // delete old departments
            var oldDepartments = _context.CompanyEventDepartments
                .Where(x => x.EventId == dto.Id);

            _context.CompanyEventDepartments.RemoveRange(oldDepartments);

            // add new departments
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
