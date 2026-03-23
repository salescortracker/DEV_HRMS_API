using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
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

        public async Task<ApiResponse<IEnumerable<GradeDto>>> GetAllAsync(int companyId)
        {
            var list = await _unitOfWork.Repository<Grade>()
                .FindAsync(x => !x.IsDeleted);

            var data = list.Select(g => new GradeDto
            {
                gradeID = g.GradeId,
                gradeName = g.GradeName,
                companyID = g.CompanyId,
                regionId = g.RegionId,
                IsActive = g.IsActive,
                companyName = _context.Companies.FirstOrDefault(x => x.CompanyId == g.CompanyId)?.CompanyName,
                regionName = _context.Regions.FirstOrDefault(x => x.RegionId == g.RegionId)?.RegionName
            });

            return new ApiResponse<IEnumerable<GradeDto>>(data, "Grades fetched");
        }

        public async Task<GradeDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(id);
            if (entity == null) return null;

            return new GradeDto
            {
                gradeID = entity.GradeId,
                gradeName = entity.GradeName,
                companyID = entity.CompanyId,
                regionId = entity.RegionId,
                IsActive = entity.IsActive
            };
        }

        public async Task<GradeDto> AddAsync(GradeDto dto)
        {
            var exists = _context.Grades.Any(x =>
                x.GradeName.ToLower() == dto.gradeName.ToLower()
                && x.CompanyId == dto.companyID
                && x.RegionId == dto.regionId);

            if (exists) return null;

            var entity = new Grade
            {
                GradeName = dto.gradeName,
                CompanyId = dto.companyID,
                RegionId = dto.regionId,
                IsActive = dto.IsActive,
                UserId = dto.userId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.userId
            };

            await _unitOfWork.Repository<Grade>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            dto.gradeID = entity.GradeId;
            return dto;
        }

        public async Task<GradeDto> UpdateAsync(GradeDto dto)
        {
            var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(dto.gradeID);

            entity.GradeName = dto.gradeName;
            entity.CompanyId = dto.companyID;
            entity.RegionId = dto.regionId;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Grade>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Grade>().GetByIdAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true; // ✅ soft delete
            await _unitOfWork.CompleteAsync();

            return true;
        }
    
}
}
