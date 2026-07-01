using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class ProjectMasterService : IProjectMasterService
    {
        private readonly HRMSContext _context;

        public ProjectMasterService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectMasterDto>> GetAllProjectsMasters(int userId)
        {
            return await _context.ProjectMasters
                .Where(x => x.UserId == userId && (x.IsDeleted == false || x.IsDeleted == null))
                .Select(x => new ProjectMasterDto
                {
                    ProjectMasterId = x.ProjectMasterId,
                    UserId = x.UserId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    ProjectName = x.ProjectName,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                    
                })
                .ToListAsync();
        }

        public async Task<ProjectMasterDto> CreateProject(ProjectMasterDto dto)
        {
            var entity = new ProjectMaster
            {
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                ProjectName = dto.ProjectName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.Now
            };
            _context.ProjectMasters.Add(entity);
            await _context.SaveChangesAsync();
            dto.ProjectMasterId = entity.ProjectMasterId;
            return dto;
        }
        public async Task<ProjectMasterDto> UpdateProject(ProjectMasterDto dto)
        {
            var entity = await _context.ProjectMasters.FirstOrDefaultAsync(x => x.ProjectMasterId == dto.ProjectMasterId);
            if (entity == null) return null;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.ProjectName = dto.ProjectName;
            entity.IsActive = dto.IsActive;
            entity.IsDeleted = dto.IsDeleted;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return dto;
        }
        public async Task<bool> DeleteProject(int id)
        {
            var entity = await _context.ProjectMasters
                .FirstOrDefaultAsync(x => x.ProjectMasterId == id && !x.IsDeleted);

            if (entity == null)
                return false;

            // 🔥 CHECK ASSIGNMENT IN TASKS
            var isAssigned = await _context.TaskAssignments
                .AnyAsync(x => x.ProjectId == id);

            if (isAssigned)
            {
                return false; // assigned → cannot delete
            }

            // ✅ SOFT DELETE
            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectMasterDto>> GetProjectsByCompanyRegion(int companyId, int regionId)
        {
            return await _context.ProjectMasters
                .Where(p => p.CompanyId == companyId && p.RegionId == regionId && p.IsActive)
                .Select(p => new ProjectMasterDto
                {
                    ProjectMasterId = p.ProjectMasterId,
                    ProjectName = p.ProjectName
                })
                .ToListAsync();
        }
    }
}
