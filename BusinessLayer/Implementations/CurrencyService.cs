using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HRMSContext _context;

        public CurrencyService(IUnitOfWork unitOfWork, HRMSContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAll(int userId)
        {
            var list = (await _unitOfWork.Repository<CurrencyMaster>()
                .FindAsync(x => !x.IsDeleted.Value))
                .Select(x => new CurrencyDto
                {
                    CurrencyId = x.CurrencyId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    CurrencyCode = x.CurrencyCode,
                    CurrencyName = x.CurrencyName,
                    IsActive = x.IsActive
                });

            return new ApiResponse<IEnumerable<CurrencyDto>>(list);
        }

        public async Task<ApiResponse<string>> CreateAsync(CurrencyDto dto)
        {
            var duplicate = (await _unitOfWork.Repository<CurrencyMaster>()
                .FindAsync(x =>
                    !x.IsDeleted.Value &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.CurrencyName.ToLower() == dto.CurrencyName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Currency", false);

            var entity = new CurrencyMaster
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                CurrencyCode = dto.CurrencyCode,
                CurrencyName = dto.CurrencyName,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.UserId
            };

            await _unitOfWork.Repository<CurrencyMaster>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Currency created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CurrencyDto dto)
        {
            var entity = await _unitOfWork.Repository<CurrencyMaster>()
                .GetByIdAsync(dto.CurrencyId);

            if (entity == null || entity.IsDeleted == true)
                return new ApiResponse<string>(null!, "Not found", false);

            var duplicate = (await _unitOfWork.Repository<CurrencyMaster>()
                .FindAsync(x =>
                    !x.IsDeleted.Value &&
                    x.CurrencyId != dto.CurrencyId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.CurrencyName.ToLower() == dto.CurrencyName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Currency", false);

            entity.CurrencyCode = dto.CurrencyCode;
            entity.CurrencyName = dto.CurrencyName;
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = dto.UserId;

            _unitOfWork.Repository<CurrencyMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<CurrencyMaster>()
                .GetByIdAsync(id);

            if (entity == null || entity.IsDeleted == true)
                return new ApiResponse<string>(null!, "Not found", false);

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CurrencyMaster>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Deleted successfully");
        }
        public async Task<ApiResponse<IEnumerable<CurrencyDto>>> CurrencyDropDown(int companyId, int regionId)
        {
            var list = (await _unitOfWork.Repository<CurrencyMaster>()
                .FindAsync(x =>
                    x.IsDeleted == false&&
                    x.IsActive == true &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId))
                .Select(x => new CurrencyDto
                {
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.CurrencyCode,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId
                });

            return new ApiResponse<IEnumerable<CurrencyDto>>(list);
        }
        public async Task<List<CurrencyMaster>> GetByCompanyAndRegion(int companyId, int regionId)
        {
            return await _context.CurrencyMasters
                .Where(x => x.CompanyId == companyId
                         && x.RegionId == regionId
                         && x.IsActive == true
                         && x.IsDeleted == false)
                .OrderBy(x => x.CurrencyName)
                .ToListAsync();
        }

    }
}
