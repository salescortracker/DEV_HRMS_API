using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class MaritalStatusService: IMaritalStatusService
    {
        private readonly HRMSContext _context;

        public MaritalStatusService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<MaritalStatusDto>> GetAllAsync(int userId)
        {
            return await _context.MaritalStatuses
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.MaritalStatusId)
                .Select(x => new MaritalStatusDto
                {
                    MaritalStatusId = x.MaritalStatusId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    MaritalStatusName = x.MaritalStatusName,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAsync(MaritalStatusDto dto)
        {
            var exists = await _context.MaritalStatuses.AnyAsync(x =>
            x.MaritalStatusName.ToLower() == dto.MaritalStatusName.ToLower() &&
            x.CompanyId == dto.CompanyId &&
            x.RegionId == dto.RegionId &&
            !x.IsDeleted
            );

            if (exists)
                throw new Exception("Marital status already exists");

            var entity = new MaritalStatus
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                MaritalStatusName = dto.MaritalStatusName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                UserId = dto.UserId,
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now
            };

            _context.MaritalStatuses.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MaritalStatusDto dto)
        { 
        //{
        //    var entity = await _context.MaritalStatuses
        //        .FirstOrDefaultAsync(x => x.MaritalStatusId == dto.MaritalStatusId && !x.IsDeleted);

        //    if (entity == null) return false;

        //    entity.CompanyId = dto.CompanyId;
        //    entity.RegionId = dto.RegionId;
        //    entity.MaritalStatusName = dto.MaritalStatusName;
        //    entity.Description = dto.Description;
        //    entity.IsActive = dto.IsActive;
        //    entity.ModifiedBy = dto.UserId;
        //    entity.ModifiedAt = DateTime.Now;

        //    await _context.SaveChangesAsync();
        //    return true;
        var exists = await _context.MaritalStatuses.AnyAsync(x =>
            x.MaritalStatusName.ToLower() == dto.MaritalStatusName.ToLower() &&
            x.CompanyId == dto.CompanyId &&
            x.RegionId == dto.RegionId &&
            x.MaritalStatusId != dto.MaritalStatusId &&
            !x.IsDeleted
            );

                if (exists)
                throw new Exception("Duplicate marital status not allowed");

        var entity = await _context.MaritalStatuses
            .FirstOrDefaultAsync(x => x.MaritalStatusId == dto.MaritalStatusId && !x.IsDeleted);

            if (entity == null) return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.MaritalStatusName = dto.MaritalStatusName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                // Get Marital Status
                var maritalStatus = await _context.MaritalStatuses
                    .FirstOrDefaultAsync(x => x.MaritalStatusId == id && !x.IsDeleted);

                if (maritalStatus == null)
                {
                    return new ApiResponse<bool>(
                        false,
                        "Marital Status not found.",
                        false);
                }

                // Check whether it is assigned to any employee
                var isAssigned = _context.EmployeePersonalDetails
                    .Any(x => x.MaritalStatusId == id);

                if (isAssigned)
                {
                    return new ApiResponse<bool>(
                        false,
                        "You cannot delete this marital status. It is assigned to one or more employees.",
                        false);
                }

                maritalStatus.IsDeleted = true;
                maritalStatus.ModifiedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(
                    true,
                    "Marital Status deleted successfully.",
                    true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(
                    false,
                    $"Error deleting marital status: {ex.Message}",
                    false);
            }
        }
    }
}
