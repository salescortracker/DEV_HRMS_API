using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class VisatypeService : IVisatypeService
    {
        private readonly HRMSContext _context;

        public VisatypeService(HRMSContext context)
        {
            _context = context;
        }

        // ===============================
        // GET LIST
        // ===============================
        public async Task<List<VisaTypeDto>> GetVisaTypeList(int userId)
        {
            return await _context.VisaTypes
                .Where(x => x.UserId == userId && (x.IsDeleted == false || x.IsDeleted == null))
                .Select(x => new VisaTypeDto
                {
                    VisaTypeId = x.VisaTypeId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    VisaType1 = x.VisaType1,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt,
                    UserId = x.UserId
                })
                .ToListAsync();
        }

        // ===============================
        // CREATE
        // ===============================
        public async Task<bool> CreateVisaType(VisaTypeDto dto)
        {
            var entity = new VisaType
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                VisaType1 = dto.VisaType1,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now,
                UserId = dto.UserId
            };

            _context.VisaTypes.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ===============================
        // UPDATE
        // ===============================
        public async Task<bool> UpdateVisaType(VisaTypeDto dto)
        {
            var entity = await _context.VisaTypes
                .FirstOrDefaultAsync(x => x.VisaTypeId == dto.VisaTypeId);

            if (entity == null)
                return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.VisaType1 = dto.VisaType1;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        // ===============================
        // DELETE
        // ===============================
        public async Task<bool> DeleteVisaType(int id)
        {
            var entity = await _context.VisaTypes
                .FirstOrDefaultAsync(x => x.VisaTypeId == id);

            if (entity == null)
                return false;

            entity.IsDeleted = true;
            entity.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}