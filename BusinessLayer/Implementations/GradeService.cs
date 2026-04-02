using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class GradeService: IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HRMSContext _context;

        public GradeService(IUnitOfWork unitOfWork, HRMSContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        // ✅ GET ALL
        public async Task<ApiResponse<IEnumerable<GradeDto>>> GetAllAsync(int userId)
        {
            try
            {
                var data = await (from g in _context.Grades
                                  join c in _context.Companies on g.CompanyId equals c.CompanyId into comp
                                  from c in comp.DefaultIfEmpty()
                                  join r in _context.Regions on g.RegionId equals r.RegionId into reg
                                  from r in reg.DefaultIfEmpty()
                                  where !g.IsDeleted && g.UserId == userId
                                  select new GradeDto
                                  {
                                      gradeID = g.GradeId,
                                      gradeName = g.GradeName,
                                      companyID = g.CompanyId,
                                      regionId = g.RegionId,
                                      IsActive = g.IsActive,
                                      companyName = c.CompanyName,
                                      regionName = r.RegionName
                                  }).ToListAsync();

                return new ApiResponse<IEnumerable<GradeDto>>(data, "Grades fetched successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<GradeDto>>(null, $"Error fetching grades: {ex.Message}", false);
            }
        }

        // ✅ GET BY ID
        public async Task<ApiResponse<GradeDto>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(id);

                if (entity == null)
                    return new ApiResponse<GradeDto>(null, "Grade not found", false);

                var dto = new GradeDto
                {
                    gradeID = entity.GradeId,
                    gradeName = entity.GradeName,
                    companyID = entity.CompanyId,
                    regionId = entity.RegionId,
                    IsActive = entity.IsActive
                };

                return new ApiResponse<GradeDto>(dto, "Grade fetched successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<GradeDto>(null, $"Error fetching grade: {ex.Message}", false);
            }
        }

        // ✅ ADD
        public async Task<ApiResponse<GradeDto>> AddAsync(GradeDto dto)
        {
            try
            {
                var gradeName = dto.gradeName.Trim().ToLower();

                var exists = await _context.Grades.AnyAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.companyID &&
                    x.RegionId == dto.regionId &&
                    x.GradeName.Trim().ToLower() == gradeName
                );

                if (exists)
                {
                    return new ApiResponse<GradeDto>(null, "Grade already exists for this company and region", false);
                }

                var entity = new Grade
                {
                    GradeName = dto.gradeName.Trim(),
                    CompanyId = dto.companyID,
                    RegionId = dto.regionId,
                    IsActive = dto.IsActive,
                    UserId = dto.userId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.userId,
                    IsDeleted = false
                };

                await _unitOfWork.Repository<Grade>().AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                dto.gradeID = entity.GradeId;

                return new ApiResponse<GradeDto>(dto, "Grade created successfully");
            }
            catch (Exception)
            {
                return new ApiResponse<GradeDto>(null, "Error creating grade", false);
            }
        }
        // ✅ UPDATE
        public async Task<ApiResponse<GradeDto>> UpdateAsync(GradeDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(dto.gradeID);

                if (entity == null)
                {
                    return new ApiResponse<GradeDto>(null, "Grade not found", false);
                }

                var gradeName = dto.gradeName.Trim().ToLower();

                var exists = await _context.Grades.AnyAsync(x =>
                    !x.IsDeleted &&
                    x.GradeId != dto.gradeID && // 🔥 exclude current record
                    x.CompanyId == dto.companyID &&
                    x.RegionId == dto.regionId &&
                    x.GradeName.Trim().ToLower() == gradeName
                );

                if (exists)
                {
                    return new ApiResponse<GradeDto>(null, "Duplicate grade exists for this company and region", false);
                }

                entity.GradeName = dto.gradeName.Trim();
                entity.CompanyId = dto.companyID;
                entity.RegionId = dto.regionId;
                entity.IsActive = dto.IsActive;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = dto.userId;

                _unitOfWork.Repository<Grade>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<GradeDto>(dto, "Grade updated successfully");
            }
            catch (Exception)
            {
                return new ApiResponse<GradeDto>(null, "Error updating grade", false);
            }
        }
        // ✅ DELETE (SOFT DELETE)
        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(id);

                if (entity == null)
                {
                    return new ApiResponse<bool>(false, "Grade not found", false);
                }

                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<Grade>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<bool>(true, "Grade deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, $"Error deleting grade: {ex.Message}", false);
            }
        }

    }
}
