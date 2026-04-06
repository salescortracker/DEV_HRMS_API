using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly HRMSContext _context;

        public LeaveTypeService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<LeaveTypeDto>> GetLeaveTypesAsync()
        {
            return await (
                from lt in _context.LeaveTypes
                join c in _context.Companies on lt.CompanyId equals c.CompanyId
                join r in _context.Regions on lt.RegionId equals r.RegionId
                where !lt.IsDeleted
                select new LeaveTypeDto
                {
                    LeaveTypeID = lt.LeaveTypeId,
                    CompanyID = lt.CompanyId,
                    RegionID = lt.RegionId,
                    LeaveTypeName = lt.LeaveTypeName,
                    Description = lt.Description,
                    LeaveDays = lt.LeaveDays,
                    IsActive = lt.IsActive,

                    CompanyName = c.CompanyName,
                    RegionName = r.RegionName
                }
            ).ToListAsync();
        }
        //public async Task<List<LeaveTypeDto>> GetLeaveTypesByuserIdAsync(int userId)
        //{
        //    return await (
        //        from lt in _context.LeaveTypes
        //        join c in _context.Companies on lt.CompanyId equals c.CompanyId
        //        join r in _context.Regions on lt.RegionId equals r.RegionId
        //        where !lt.IsDeleted && lt.UserId == userId
        //        select new LeaveTypeDto
        //        {
        //            LeaveTypeID = lt.LeaveTypeId,
        //            CompanyID = lt.CompanyId,
        //            RegionID = lt.RegionId,
        //            LeaveTypeName = lt.LeaveTypeName,
        //            Description = lt.Description,
        //            LeaveDays = lt.LeaveDays,
        //            IsActive = lt.IsActive,

        //            CompanyName = c.CompanyName,
        //            RegionName = r.RegionName
        //        }
        //    ).ToListAsync();
        //}

        public async Task<List<LeaveTypeDto>> GetLeaveTypesByuserIdAsync(int userId)
        {
            var data = await (
                from lt in _context.LeaveTypes
                join c in _context.Companies on lt.CompanyId equals c.CompanyId
                join r in _context.Regions on lt.RegionId equals r.RegionId
                where !lt.IsDeleted && lt.UserId == userId
                select new LeaveTypeDto
                {
                    LeaveTypeID = lt.LeaveTypeId,
                    CompanyID = lt.CompanyId,
                    RegionID = lt.RegionId,
                    LeaveTypeName = lt.LeaveTypeName,
                    Description = lt.Description,
                    LeaveDays = lt.LeaveDays,
                    IsActive = lt.IsActive,
                    CompanyName = c.CompanyName,
                    RegionName = r.RegionName,

                    //✅ THIS IS THE FIX
                    GradeAllocations = (
                from g in _context.LeaveTypeGrades
                where g.LeaveTypeId == lt.LeaveTypeId && g.IsActive == true
                select new LeaveTypeGradeDto
                {
                    GradeID = g.GradeId,
                    gradename = _context.Grades.Where(x => x.GradeId == g.GradeId).FirstOrDefault().GradeName,
                    LeaveDays = g.LeaveDays
                }
            ).ToList()
                }
    ).ToListAsync();

            return data;
        }

        public async Task<ApiResponse<IEnumerable<LeaveTypeDto>>> GetCRLeaveTypesAsync(
    int companyId,
    int regionId)
        {
            var list = await (
                from lt in _context.LeaveTypes
                join c in _context.Companies on lt.CompanyId equals c.CompanyId
                join r in _context.Regions on lt.RegionId equals r.RegionId
                where !lt.IsDeleted
                      && lt.CompanyId == companyId
                      && lt.RegionId == regionId
                select new LeaveTypeDto
                {
                    LeaveTypeID = lt.LeaveTypeId,
                    CompanyID = lt.CompanyId,
                    RegionID = lt.RegionId,
                    LeaveTypeName = lt.LeaveTypeName,
                    Description = lt.Description,
                    LeaveDays = lt.LeaveDays,
                    IsActive = lt.IsActive,

                    CompanyName = c.CompanyName,
                    RegionName = r.RegionName
                }
            ).ToListAsync();
            //            var data = await (
            //    from lt in _context.LeaveTypes
            //    join gmap in _context.LeaveTypeGrades on lt.LeaveTypeId equals gmap.LeaveTypeId
            //    join g in _context.Grades on gmap.GradeId equals g.GradeId
            //    where !lt.IsDeleted
            //    select new LeaveTypeDto
            //    {
            //        LeaveTypeID = lt.LeaveTypeId,
            //        LeaveTypeName = lt.LeaveTypeName,
            //        GradeAllocations = new List<LeaveTypeGradeDto>
            //        {
            //            new LeaveTypeGradeDto
            //            {
            //                GradeID = g.GradeId,
            //                LeaveDays = gmap.LeaveDays
            //            }
            //        }
            //    }
            //).ToListAsync();

            return new ApiResponse<IEnumerable<LeaveTypeDto>>(list);
        }


        //public async Task<bool> CreateLeaveTypeAsync(LeaveTypeDto dto)
        //{
        //    var entity = new LeaveType
        //    {
        //        CompanyId = dto.CompanyID,
        //        RegionId = dto.RegionID,
        //        LeaveTypeName = dto.LeaveTypeName,
        //        Description = dto.Description,
        //        LeaveDays = dto.LeaveDays,
        //        IsActive = dto.IsActive,
        //        IsDeleted = false,
        //        CreatedAt = DateTime.Now,
        //        UserId=dto.userId
        //    };

        //    _context.LeaveTypes.Add(entity);
        //    return await _context.SaveChangesAsync() > 0;
        //}
        public async Task<ApiResponse<bool>> CreateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var exists = await _context.LeaveTypes.AnyAsync(x =>
                !x.IsDeleted &&
                x.CompanyId == dto.CompanyID &&
                x.RegionId == dto.RegionID &&
                x.LeaveTypeName.Trim().ToLower() == dto.LeaveTypeName.Trim().ToLower()
            );

            if (exists)
                return new ApiResponse<bool>(false, "Leave Type already exists", false);

            var entity = new LeaveType
            {
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                LeaveTypeName = dto.LeaveTypeName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                UserId = dto.userId,
                LeaveDays = dto.LeaveDays
            };

            _context.LeaveTypes.Add(entity);
            await _context.SaveChangesAsync();

            // ✅ Insert Grade Mapping
            foreach (var g in dto.GradeAllocations)
            {
                _context.LeaveTypeGrades.Add(new LeaveTypeGrade
                {
                    LeaveTypeId = entity.LeaveTypeId,
                    GradeId = g.GradeID,
                    LeaveDays = g.LeaveDays
                });
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Leave Type created successfully", true);
        }
        //public async Task<bool> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        //{
        //    var entity = await _context.LeaveTypes
        //        .FirstOrDefaultAsync(x => x.LeaveTypeId == dto.LeaveTypeID && !x.IsDeleted);

        //    if (entity == null) return false;

        //    entity.LeaveTypeName = dto.LeaveTypeName;
        //    entity.Description = dto.Description;
        //    entity.CompanyId = dto.CompanyID;
        //    entity.RegionId = dto.RegionID;
        //    entity.LeaveDays = dto.LeaveDays;
        //    entity.IsActive = dto.IsActive;
        //    entity.UserId = dto.userId;
        //    entity.ModifiedAt = DateTime.Now;

        //    return await _context.SaveChangesAsync() > 0;
        //}

        public async Task<ApiResponse<bool>> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var entity = await _context.LeaveTypes
                .FirstOrDefaultAsync(x => x.LeaveTypeId == dto.LeaveTypeID && !x.IsDeleted);

            if (entity == null)
                return new ApiResponse<bool>(false, "Leave Type not found", false);

            var exists = await _context.LeaveTypes.AnyAsync(x =>
                !x.IsDeleted &&
                x.LeaveTypeId != dto.LeaveTypeID &&
                x.CompanyId == dto.CompanyID &&
                x.RegionId == dto.RegionID &&
                x.LeaveTypeName.Trim().ToLower() == dto.LeaveTypeName.Trim().ToLower()
            );

            if (exists)
                //return new ApiResponse<bool>(false, "Leave Type already exists");
                return new ApiResponse<bool>(false, "Leave Type already exists", false);
            entity.LeaveTypeName = dto.LeaveTypeName;
            entity.Description = dto.Description;
            entity.CompanyId = dto.CompanyID;
            entity.RegionId = dto.RegionID;
            entity.IsActive = dto.IsActive;
            entity.UserId = dto.userId;
            entity.ModifiedAt = DateTime.Now;

            var oldMappings = _context.LeaveTypeGrades
                .Where(x => x.LeaveTypeId == dto.LeaveTypeID);

            _context.LeaveTypeGrades.RemoveRange(oldMappings);


            foreach (var g in dto.GradeAllocations)
            {
                _context.LeaveTypeGrades.Add(new LeaveTypeGrade
                {
                    LeaveTypeId = dto.LeaveTypeID,
                    GradeId = g.GradeID,
                    LeaveDays = g.LeaveDays,
                    IsActive = true
                });
            }

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteLeaveTypeAsync(int id)
        {
            var entity = await _context.LeaveTypes
                .FirstOrDefaultAsync(x => x.LeaveTypeId == id && !x.IsDeleted);

            if (entity == null)
                return new ApiResponse<bool>(false, "Leave Type not found", false);

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.Now;

            var grades = await _context.LeaveTypeGrades
                .Where(x => x.LeaveTypeId == id)
                .ToListAsync();

            foreach (var g in grades)
                g.IsActive = false;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true, "Deleted successfully", true);
        }

        public async Task<List<UserLeaveAllocationDto>> GetUserLeaveAllocation(int userId)
        {
            var result = await (from u in _context.Users
                                join d in _context.Designations on u.DesignationId equals d.DesignationId
                                join g in _context.Grades on d.GradeId equals g.GradeId
                                join ltg in _context.LeaveTypeGrades on g.GradeId equals ltg.GradeId
                                join lt in _context.LeaveTypes on ltg.LeaveTypeId equals lt.LeaveTypeId

                                // LEFT JOIN LeaveRequests
                                join lr in _context.LeaveRequests
                                on new { UserId = u.UserId, LeaveTypeId = lt.LeaveTypeId }
                                equals new { lr.UserId, lr.LeaveTypeId }
                                into lrGroup

                                from lr in lrGroup.DefaultIfEmpty()

                                where u.UserId == userId
                                      && ltg.IsActive == true
                                      && lt.IsActive

                                group lr by new
                                {
                                    u.UserId,
                                    u.FullName,
                                    d.DesignationName,
                                    g.GradeName,
                                    lt.LeaveTypeName,
                                    ltg.LeaveDays
                                } into grp

                                select new UserLeaveAllocationDto
                                {
                                    UserId = grp.Key.UserId,
                                    FullName = grp.Key.FullName,
                                    DesignationName = grp.Key.DesignationName,
                                    GradeName = grp.Key.GradeName,
                                    LeaveTypeName = grp.Key.LeaveTypeName,

                                    AllocatedLeaves = grp.Key.LeaveDays,

                                    ApprovedLeaves = (int)(
    grp.Where(x => x != null && x.Status == "Approved")
       .Sum(x => (decimal?)x.TotalDays) ?? 0
),

                                    PendingLeaves = (int)(
    grp.Where(x => x != null && x.Status == "Pending")
       .Sum(x => (decimal?)x.TotalDays) ?? 0
),

                                    RemainingLeaves = (int)(
    grp.Key.LeaveDays -
    (grp.Where(x => x != null &&
        (x.Status == "Approved" || x.Status == "Pending"))
     .Sum(x => (decimal?)x.TotalDays) ?? 0))
                                }).ToListAsync();

            return result;
        }


        public async Task<List<DesignationDTO>> GetDesignationsAsync(int companyId, int regionId)
        {
            return await _context.Set<Designation>()
             .Where(d => d.CompanyId == companyId
            && d.RegionId == regionId
            && !d.IsDeleted
            && d.IsActive)
             .Select(d => new DesignationDTO
             {
                 DesignationID = d.DesignationId,
                 DesignationName = d.DesignationName
             })
             .ToListAsync();
        }
    }
}
