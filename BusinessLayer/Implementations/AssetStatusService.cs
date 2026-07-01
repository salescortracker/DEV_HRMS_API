using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class AssetStatusService : IAssetStatusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<AssetStatusDto>>> GetAll(int userId)
        {
            var list = (await _unitOfWork.Repository<AssetStatus>()
                .FindAsync(x => !x.IsDeleted && x.CreatedBy == userId))
                .Select(x => new AssetStatusDto
                {
                    AssetStatusId = x.AssetStatusId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    AssetStatusName = x.AssetStatusName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    UserId = x.CreatedBy
                });

            return new ApiResponse<IEnumerable<AssetStatusDto>>(list);
        }

        public async Task<ApiResponse<string>> CreateAsync(AssetStatusDto dto)
        {
            var duplicate = (await _unitOfWork.Repository<AssetStatus>().FindAsync(x =>
                !x.IsDeleted &&
                x.CompanyId == dto.CompanyId &&
                x.RegionId == dto.RegionId &&
                x.AssetStatusName.ToLower() == dto.AssetStatusName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Asset Status exists", false);

            var entity = new AssetStatus
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                AssetStatusName = dto.AssetStatusName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.UserId
            };

            await _unitOfWork.Repository<AssetStatus>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(AssetStatusDto dto)
        {
            var entity = await _unitOfWork.Repository<AssetStatus>()
                .GetByIdAsync(dto.AssetStatusId);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(null!, "Not found", false);

            var duplicate = (await _unitOfWork.Repository<AssetStatus>().FindAsync(x =>
                !x.IsDeleted &&
                x.AssetStatusId != dto.AssetStatusId &&
                x.CompanyId == dto.CompanyId &&
                x.RegionId == dto.RegionId &&
                x.AssetStatusName.ToLower() == dto.AssetStatusName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate exists", false);
            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;

            entity.AssetStatusName = dto.AssetStatusName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = dto.UserId;

            _unitOfWork.Repository<AssetStatus>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<AssetStatus>()
                    .GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                {
                    return new ApiResponse<string>(
                        null!,
                        "Asset Status not found.",
                        false);
                }

                // Check whether Asset Status is assigned to any Asset
                var isAssigned = (await _unitOfWork.Repository<Asset>()
                    .FindAsync(x => x.AssetStatusId == id))
                    .Any();

                if (isAssigned)
                {
                    return new ApiResponse<string>(
                        null!,
                        "You cannot delete this asset status. It is assigned to one or more assets.",
                        false);
                }

                // Soft Delete
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<AssetStatus>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Asset Status deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    ex.Message,
                    false);
            }
        }

    }
}
