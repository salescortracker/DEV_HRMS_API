using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class LeaveTypeService:ILeaveTypeService
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

                    // ✅ THIS IS THE FIX
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
            var data = await (
    from lt in _context.LeaveTypes
    join gmap in _context.LeaveTypeGrades on lt.LeaveTypeId equals gmap.LeaveTypeId
    join g in _context.Grades on gmap.GradeId equals g.GradeId
    where !lt.IsDeleted
    select new LeaveTypeDto
    {
        LeaveTypeID = lt.LeaveTypeId,
        LeaveTypeName = lt.LeaveTypeName,
        GradeAllocations = new List<LeaveTypeGradeDto>
        {
                        new LeaveTypeGradeDto
                        {
                            GradeID = g.GradeId,
                            LeaveDays = gmap.LeaveDays
                        }
        }
    }
).ToListAsync();

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
        //public async Task<bool> CreateLeaveTypeAsync(LeaveTypeDto dto)
        //{
        //    var entity = new LeaveType
        //    {
        //        CompanyId = dto.CompanyID,
        //        RegionId = dto.RegionID,
        //        LeaveTypeName = dto.LeaveTypeName,
        //        IsActive = dto.IsActive,
        //        IsDeleted = false,
        //        CreatedAt = DateTime.Now,
        //        UserId = dto.userId,
        //        LeaveDays=dto.LeaveDays
        //    };

        //    _context.LeaveTypes.Add(entity);
        //   return await _context.SaveChangesAsync()>0;

        //    // 🔥 Insert Grade Mapping
        //    //foreach (var g in dto.GradeAllocations)
        //    //{
        //    //    _context.LeaveTypeGrades.Add(new LeaveTypeGrade
        //    //    {
        //    //        LeaveTypeId = entity.LeaveTypeId,
        //    //        GradeId = g.GradeID,
        //    //        LeaveDays = g.LeaveDays
        //    //    });
        //    //}

        //   // return await _context.SaveChangesAsync() > 0;
        //}
        public async Task<bool> CreateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var exists = await _context.LeaveTypes.AnyAsync(x =>
      x.CompanyId == dto.CompanyID &&
      x.RegionId == dto.RegionID &&
      x.LeaveTypeName.ToLower() == dto.LeaveTypeName.ToLower() &&
      !x.IsDeleted);

            if (exists)
            {
                throw new Exception("Leave Type already exists.");
            }

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

            // 🔥 Insert Grade Mapping
            foreach (var g in dto.GradeAllocations)
            {
                _context.LeaveTypeGrades.Add(new LeaveTypeGrade
                {
                    LeaveTypeId = entity.LeaveTypeId,
                    GradeId = g.GradeID,
                    LeaveDays = g.LeaveDays
                });
            }

            return await _context.SaveChangesAsync() > 0;
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

        //public async Task<bool> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        //{
        //    var entity = await _context.LeaveTypes
        //        .FirstOrDefaultAsync(x => x.LeaveTypeId == dto.LeaveTypeID && !x.IsDeleted);

        //    if (entity == null) return false;

        //    // ✅ Update main table
        //    entity.LeaveTypeName = dto.LeaveTypeName;
        //    entity.Description = dto.Description;
        //    entity.CompanyId = dto.CompanyID;
        //    entity.RegionId = dto.RegionID;
        //    entity.IsActive = dto.IsActive;
        //    entity.UserId = dto.userId;
        //    entity.ModifiedAt = DateTime.Now;

        //    // 🔥 STEP 1: Remove old mappings
        //    var oldMappings = _context.LeaveTypeGrades
        //        .Where(x => x.LeaveTypeId == dto.LeaveTypeID);

        //    _context.LeaveTypeGrades.RemoveRange(oldMappings);

        //    // 🔥 STEP 2: Insert new mappings
        //    //foreach (var g in dto.GradeAllocations)
        //    //{
        //    //    _context.LeaveTypeGrades.Add(new LeaveTypeGrade
        //    //    {
        //    //        LeaveTypeId = dto.LeaveTypeID,
        //    //        GradeId = g.GradeID,
        //    //        LeaveDays = g.LeaveDays,
        //    //        IsActive = true
        //    //    });
        //    //}

        //    // ✅ Save all changes
        //    return await _context.SaveChangesAsync() > 0;
        //}
        public async Task<bool> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        {
            // ✅ CHECK DUPLICATE (Exclude Current Record)
            var duplicateExists = await _context.LeaveTypes.AnyAsync(x =>
                x.LeaveTypeId != dto.LeaveTypeID &&
                x.CompanyId == dto.CompanyID &&
                x.RegionId == dto.RegionID &&
                x.LeaveTypeName.ToLower() == dto.LeaveTypeName.ToLower() &&
                !x.IsDeleted);

            if (duplicateExists)
            {
                throw new Exception("Leave Type already exists.");
            }

            // ✅ GET EXISTING RECORD
            var entity = await _context.LeaveTypes
                .FirstOrDefaultAsync(x =>
                    x.LeaveTypeId == dto.LeaveTypeID &&
                    !x.IsDeleted);

            if (entity == null)
                return false;

            // ✅ UPDATE MAIN TABLE
            entity.LeaveTypeName = dto.LeaveTypeName;
            entity.Description = dto.Description;
            entity.CompanyId = dto.CompanyID;
            entity.RegionId = dto.RegionID;
            entity.IsActive = dto.IsActive;
            entity.UserId = dto.userId;
            entity.ModifiedAt = DateTime.Now;
            entity.LeaveDays = dto.LeaveDays;

            // ✅ REMOVE OLD GRADE MAPPINGS
            var oldMappings = _context.LeaveTypeGrades
                .Where(x => x.LeaveTypeId == dto.LeaveTypeID);

            _context.LeaveTypeGrades.RemoveRange(oldMappings);

            // 🔥 STEP 2: Insert new mappings
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

            // ✅ Save all changes
            return await _context.SaveChangesAsync() > 0;
        }


        //public async Task<bool> DeleteLeaveTypeAsync(int id)
        //{
        //    var entity = await _context.LeaveTypes
        //        .FirstOrDefaultAsync(x => x.LeaveTypeId == id && !x.IsDeleted);

        //    if (entity == null)
        //        return false; // Already deleted or not found

        //    entity.IsDeleted = true;
        //    entity.ModifiedAt = DateTime.Now;

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<ApiResponse<bool>> DeleteLeaveTypeAsync(int id)
        {
            try
            {
                var entity = await _context.LeaveTypes
                    .FirstOrDefaultAsync(x => x.LeaveTypeId == id && !x.IsDeleted);

                if (entity == null)
                {
                    return new ApiResponse<bool>(
                        false,
                        "Leave Type not found.",
                        false);
                }

                // Check whether Leave Type is assigned
                var isAssigned = await _context.LeaveRequests
                    .AnyAsync(x => x.LeaveTypeId == id);

                if (isAssigned)
                {
                    return new ApiResponse<bool>(
                        false,
                        "You cannot delete this leave type. It is assigned to one or more leave requests.",
                        false);
                }

                // Soft Delete Leave Type
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;

                // Disable related Leave Type Grades
                var grades = await _context.LeaveTypeGrades
                    .Where(x => x.LeaveTypeId == id)
                    .ToListAsync();

                foreach (var g in grades)
                {
                    g.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(
                    true,
                    "Leave Type deleted successfully.",
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
