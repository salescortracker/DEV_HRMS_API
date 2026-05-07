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
            return await _context.AttachmentTypes
                .Where(x => !x.IsDeleted &&
                            x.CreatedBy == userId)   // ✅ KEY CHANGE
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
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                AttachmentCategory = dto.AttachmentCategory,
                AttachmentTypeName = dto.AttachmentTypeName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.UserId,
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
            if (entity.CreatedBy != dto.UserId)
                return false;

            entity.AttachmentCategory = dto.AttachmentCategory;
            entity.AttachmentTypeName = dto.AttachmentTypeName;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<AttachmentTypeDto>> GetDocumentsAsync(int companyId, int regionId)
        {
            var data = await _context.AttachmentTypes
                .Where(x => x.CompanyId == companyId
                         && x.RegionId == regionId
                         && x.IsActive)
                .Select(x => new AttachmentTypeDto
                {
                    AttachmentTypeId = x.AttachmentTypeId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    AttachmentCategory = x.AttachmentCategory,
                    AttachmentTypeName = x.AttachmentTypeName,
                    IsActive = x.IsActive
                })
                .OrderBy(x => x.AttachmentTypeName)
                .ToListAsync();

            return data;
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




        public async Task<IEnumerable<AttachmentTypeDto>> GetByCategoryAsync(
      string category,
      int companyId,
      int regionId)
        {
            return await _context.AttachmentTypes
                .Where(x => !x.IsDeleted &&
                            x.CompanyId == companyId &&
                            x.RegionId == regionId &&
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
