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

        public async Task<ApiResponse<string>> DeleteAttachmentTypeAsync(int id)
        {
            try
            {
                var entity = await _context.AttachmentTypes
                    .FirstOrDefaultAsync(x => x.AttachmentTypeId == id && !x.IsDeleted);

                if (entity == null)
                {
                    return new ApiResponse<string>(
                        null!,
                        "Attachment Type not found.",
                        false);
                }

                // ✅ CHECK 1: EmployeeDocuments
                var usedInDocuments = await _context.EmployeeDocuments
                    .AnyAsync(x => x.DocumentTypeId == id);

                // ✅ CHECK 2: EmployeeForms
                var usedInForms = await _context.EmployeeForms
                    .AnyAsync(x => x.DocumentTypeId == id);

                // ✅ CHECK 3: EmployeeLetters
                var usedInLetters = await _context.EmployeeLetters
                    .AnyAsync(x => x.DocumentTypeId == id);

                if (usedInDocuments || usedInForms || usedInLetters)
                {
                    return new ApiResponse<string>(
                        null!,
                        "You cannot delete this attachment type. It is already used in employee documents/forms/letters.",
                        false);
                }

                // ✅ Soft Delete
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;

                _context.AttachmentTypes.Update(entity);
                await _context.SaveChangesAsync();

                return new ApiResponse<string>(
                    "Attachment Type deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    $"Error: {ex.Message}",
                    false);
            }
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
