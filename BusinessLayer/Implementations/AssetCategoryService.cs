using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class AssetCategoryService : IAssetCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssetCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ================= GET ALL =================
        public async Task<ApiResponse<IEnumerable<AssetCategoryDto>>> GetAll(int userId)
        {
            try
            {
                var list = (await _unitOfWork.Repository<AssetCategory>()
                    .FindAsync(x => !x.IsDeleted && x.UserId == userId))
                    .OrderByDescending(x => x.AssetCategoryId)
                    .Select(x => new AssetCategoryDto
                    {
                        AssetCategoryId = x.AssetCategoryId,
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        AssetCategoryName = x.AssetCategoryName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        UserId = x.UserId
                    });

                return new ApiResponse<IEnumerable<AssetCategoryDto>>(list, "Asset Categories retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<AssetCategoryDto>>(null!,
                    $"Failed to retrieve asset categories. {ex.Message}", false);
            }
        }

        // ================= GET BY ID =================
        public async Task<ApiResponse<AssetCategoryDto?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<AssetCategory>().GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<AssetCategoryDto?>(null, "Asset Category not found.", false);

                var dto = new AssetCategoryDto
                {
                    AssetCategoryId = entity.AssetCategoryId,
                    CompanyId = entity.CompanyId,
                    RegionId = entity.RegionId,
                    AssetCategoryName = entity.AssetCategoryName,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                    UserId = entity.UserId
                };

                return new ApiResponse<AssetCategoryDto?>(dto, "Asset Category retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<AssetCategoryDto?>(null,
                    $"Failed to retrieve asset category. {ex.Message}", false);
            }
        }

        // ================= CREATE =================
        public async Task<ApiResponse<string>> CreateAsync(AssetCategoryDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.AssetCategoryName))
                    return new ApiResponse<string>(null!, "Asset Category Name is required.", false);

                // ✅ Duplicate check
                var duplicate = (await _unitOfWork.Repository<AssetCategory>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.AssetCategoryName.ToLower() == dto.AssetCategoryName.ToLower()))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Asset Category exists.", false);

                var entity = new AssetCategory
                {
                    CompanyId = dto.CompanyId,
                    RegionId = dto.RegionId,
                    AssetCategoryName = dto.AssetCategoryName,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.UserId,
                    UserId = dto.UserId
                };

                await _unitOfWork.Repository<AssetCategory>().AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Asset Category created successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null!,
                    $"Create failed. {ex.Message}", false);
            }
        }

        // ================= UPDATE =================
        public async Task<ApiResponse<string>> UpdateAsync(AssetCategoryDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<AssetCategory>().GetByIdAsync(dto.AssetCategoryId);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<string>(null!, "Asset Category not found.", false);

                // ✅ Duplicate check
                var duplicate = (await _unitOfWork.Repository<AssetCategory>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.AssetCategoryId != dto.AssetCategoryId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.AssetCategoryName.ToLower() == dto.AssetCategoryName.ToLower()))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Asset Category exists.", false);
                entity.CompanyId = dto.CompanyId;   // ✅ ADD THIS
                entity.RegionId = dto.RegionId;     // ✅ ADD THIS

                entity.AssetCategoryName = dto.AssetCategoryName;
                entity.Description = dto.Description;
                entity.IsActive = dto.IsActive;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = dto.UserId;

                _unitOfWork.Repository<AssetCategory>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Asset Category updated successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null!,
                    $"Update failed. {ex.Message}", false);
            }
        }

        // ================= DELETE =================
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<AssetCategory>()
                    .GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                {
                    return new ApiResponse<string>(
                        null!,
                        "Asset Category not found.",
                        false);
                }

                // Check whether Asset Category is assigned
                var isAssigned = (await _unitOfWork.Repository<Asset>()
                    .FindAsync(x => x.AssetCategoryId == id))
                    .Any();

                if (isAssigned)
                {
                    return new ApiResponse<string>(
                        null!,
                        "You cannot delete this asset category. It is assigned to one or more assets.",
                        false);
                }

                // Soft Delete
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = entity.UserId;

                _unitOfWork.Repository<AssetCategory>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Asset Category deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    ex.Message,
                    false);
            }
        }

        public async Task<ApiResponse<IEnumerable<AssetCategoryDto>>> AssetCategoryDropDown(int companyId, int regionId)
        {
            var list = (await _unitOfWork.Repository<AssetCategory>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.IsActive &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId))
                .Select(x => new AssetCategoryDto
                {
                    AssetCategoryId = x.AssetCategoryId,
                    AssetCategoryName = x.AssetCategoryName,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId
                });

            return new ApiResponse<IEnumerable<AssetCategoryDto>>(list);
        }

    }
}
