using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class AttachmentTypeService : IAttachmentTypeService
    {
        private readonly HRMSContext _context;

        public AttachmentTypeService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttachmentTypeDto>> GetAllByUserAttachmentTypeAsync(int userId)
        {
            // Step 1: Get user
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                return new List<AttachmentTypeDto>();

            // Step 2: Filter attachment types
            return await _context.AttachmentTypes
                .Where(x => !x.IsDeleted &&
                            x.CompanyId == user.CompanyId &&
                            x.RegionId == user.RegionId)
                .Select(x => new AttachmentTypeDto
                {
                    AttachmentTypeId = x.AttachmentTypeId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    AttachmentCategory = x.AttachmentCategory,
                    AttachmentTypeName = x.AttachmentTypeName,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<bool> CreateAttachmentTypeAsync(AttachmentTypeDto dto)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == dto.UserId);

            if (user == null)
                return false;

            var entity = new AttachmentType
            {
                CompanyId = user.CompanyId,
                RegionId = user.RegionId,
                AttachmentCategory = dto.AttachmentCategory,
                AttachmentTypeName = dto.AttachmentTypeName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = user.UserId,
                CreatedAt = DateTime.Now
            };

            _context.AttachmentTypes.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAttachmentTypeAsync(AttachmentTypeDto dto)
        {
            var entity = await _context.AttachmentTypes
                .FirstOrDefaultAsync(x => x.AttachmentTypeId == dto.AttachmentTypeId);

            if (entity == null) return false;

            entity.AttachmentCategory = dto.AttachmentCategory;
            entity.AttachmentTypeName = dto.AttachmentTypeName;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = 1;
            entity.ModifiedAt = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAttachmentTypeAsync(int id)
        {
            var entity = await _context.AttachmentTypes
                .FirstOrDefaultAsync(x => x.AttachmentTypeId == id);

            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.ModifiedBy = 1;
            entity.ModifiedAt = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<IEnumerable<AttachmentTypeDto>> GetByCategoryAsync(string category)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync();

            if (user == null)
                return new List<AttachmentTypeDto>();

            return await _context.AttachmentTypes
                .Where(x => !x.IsDeleted &&
                            x.CompanyId == user.CompanyId &&
                            x.RegionId == user.RegionId &&
                            x.AttachmentCategory == category &&
                            x.IsActive)
                .Select(x => new AttachmentTypeDto
                {
                    AttachmentTypeId = x.AttachmentTypeId,
                    AttachmentTypeName = x.AttachmentTypeName
                })
                .ToListAsync();
        }

    }
}
