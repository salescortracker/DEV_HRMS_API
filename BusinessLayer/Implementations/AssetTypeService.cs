using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class AssetTypeService : IAssetTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET ALL
        public async Task<ApiResponse<IEnumerable<AssetTypeDto>>> GetAll(int userId)
        {
            var list = (await _unitOfWork.Repository<AssetType>()
                .FindAsync(x => !x.IsDeleted && x.UserId == userId))
                .OrderByDescending(x => x.AssetTypeId)
                .Select(x => new AssetTypeDto
                {
                    AssetTypeId = x.AssetTypeId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    AssetTypeName = x.AssetTypeName,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    UserId = x.UserId,
                    AssetCategoryId = x.AssetCategoryId ?? 0
                });

            return new ApiResponse<IEnumerable<AssetTypeDto>>(list, "Asset Types fetched");
        }

        // GET BY ID
        public async Task<ApiResponse<AssetTypeDto?>> GetById(int id)
        {
            var x = await _unitOfWork.Repository<AssetType>().GetByIdAsync(id);

            if (x == null || x.IsDeleted)
                return new ApiResponse<AssetTypeDto?>(null, "Not found", false);

            return new ApiResponse<AssetTypeDto?>(new AssetTypeDto
            {
                AssetTypeId = x.AssetTypeId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                AssetTypeName = x.AssetTypeName,
                Description = x.Description,
                IsActive = x.IsActive,
                UserId = x.UserId
            });
        }

        // CREATE
        public async Task<ApiResponse<string>> CreateAsync(AssetTypeDto dto)
        {
            // Duplicate check
            var duplicate = (await _unitOfWork.Repository<AssetType>().FindAsync(x =>
                !x.IsDeleted &&
                x.CompanyId == dto.CompanyId &&
                x.RegionId == dto.RegionId &&
                x.AssetTypeName.ToLower() == dto.AssetTypeName.ToLower()
            )).Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Asset Type exists", false);

            var entity = new AssetType
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                AssetTypeName = dto.AssetTypeName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = dto.UserId,
                UserId = dto.UserId,
                AssetCategoryId = dto.AssetCategoryId,   // ✅ ADD
            };

            await _unitOfWork.Repository<AssetType>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Created successfully");
        }

        // UPDATE
        public async Task<ApiResponse<string>> UpdateAsync(AssetTypeDto dto)
        {
            var entity = await _unitOfWork.Repository<AssetType>().GetByIdAsync(dto.AssetTypeId);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(null!, "Not found", false);

            var duplicate = (await _unitOfWork.Repository<AssetType>().FindAsync(x =>
                !x.IsDeleted &&
                x.AssetTypeId != dto.AssetTypeId &&
                x.CompanyId == dto.CompanyId &&
                x.RegionId == dto.RegionId &&
                x.AssetTypeName.ToLower() == dto.AssetTypeName.ToLower()
            )).Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Asset Type exists", false);
            entity.CompanyId = dto.CompanyId;   // ✅ ADD THIS
            entity.RegionId = dto.RegionId;     // ✅ ADD THIS

            entity.AssetTypeName = dto.AssetTypeName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = dto.UserId;
            entity.AssetCategoryId = dto.AssetCategoryId;   // ✅ ADD

            _unitOfWork.Repository<AssetType>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Updated successfully");
        }

        // DELETE
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<AssetType>()
                    .GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                {
                    return new ApiResponse<string>(
                        null!,
                        "Asset Type not found.",
                        false);
                }

                // Check whether Asset Type is assigned
                var isAssigned = (await _unitOfWork.Repository<Asset>()
                    .FindAsync(x => x.AssetTypeId == id))
                    .Any();

                if (isAssigned)
                {
                    return new ApiResponse<string>(
                        null!,
                        "You cannot delete this asset type. It is assigned to one or more assets.",
                        false);
                }

                // Soft Delete
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = entity.UserId;

                _unitOfWork.Repository<AssetType>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Asset Type deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    ex.Message,
                    false);
            }
        }

        public async Task<ApiResponse<IEnumerable<AssetTypeDto>>> GetByCompanyRegion(int companyId, int regionId, int assetCategoryId)
        {
            var list = (await _unitOfWork.Repository<AssetType>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                   (assetCategoryId == 0 || x.AssetCategoryId == assetCategoryId)))
                .Select(x => new AssetTypeDto
                {
                    AssetTypeId = x.AssetTypeId,
                    AssetTypeName = x.AssetTypeName,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    AssetCategoryId = x.AssetCategoryId ?? 0
                });

            return new ApiResponse<IEnumerable<AssetTypeDto>>(list);
        }

        public async Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAssetCategoriestype(int userId)
        {
            var list = (await _unitOfWork.Repository<AssetCategory>()
                .FindAsync(x => !x.IsDeleted && x.UserId == userId))
                .Select(x => new AssetCategoryDto
                {
                    AssetCategoryId = x.AssetCategoryId,
                    AssetCategoryName = x.AssetCategoryName,
                    IsActive = x.IsActive
                });

            return new ApiResponse<IEnumerable<AssetCategoryDto>>(list);
        }

    }
}
