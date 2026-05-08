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
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<CountryDto>>> GetAll(int userId)
        {
            try
            {
                var list = (await _unitOfWork.Repository<CountryMaster>()
                    .FindAsync(x => !x.IsDeleted && x.CreatedBy == userId))
                    .OrderByDescending(x => x.CountryId)
                    .Select(x => new CountryDto
                    {
                        CountryId = x.CountryId,
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        CountryName = x.CountryName,
                        IsActive = x.IsActive,
                        UserId = x.CreatedBy ?? 0
                    });

                return new ApiResponse<IEnumerable<CountryDto>>(list,
                    "Countries retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CountryDto>>(
                    null!,
                    $"Failed to retrieve countries. {ex.Message}",
                    false
                );
            }
        }

        public async Task<ApiResponse<CountryDto?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork
                    .Repository<CountryMaster>()
                    .GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<CountryDto?>(
                        null,
                        "Country not found.",
                        false
                    );

                var dto = new CountryDto
                {
                    CountryId = entity.CountryId,
                    CompanyId = entity.CompanyId,
                    RegionId = entity.RegionId,
                    CountryName = entity.CountryName,
                    IsActive = entity.IsActive,
                    UserId = entity.CreatedBy ?? 0
                };

                return new ApiResponse<CountryDto?>(
                    dto,
                    "Country retrieved successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<CountryDto?>(
                    null,
                    $"Failed to retrieve country. {ex.Message}",
                    false
                );
            }
        }

        public async Task<ApiResponse<string>> CreateAsync(CountryDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.CountryName))
                    return new ApiResponse<string>(
                        null!,
                        "Country Name is required.",
                        false
                    );

                var duplicate = (await _unitOfWork.Repository<CountryMaster>()
                    .FindAsync(x =>
                        !x.IsDeleted &&
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.CountryName.ToLower() == dto.CountryName.ToLower()
                    )).Any();

                if (duplicate)
                    return new ApiResponse<string>(
                        null!,
                        "Duplicate Country exists.",
                        false
                    );

                var entity = new CountryMaster
                {
                    CompanyId = dto.CompanyId,
                    RegionId = dto.RegionId,
                    CountryName = dto.CountryName,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.UserId
                };

                await _unitOfWork.Repository<CountryMaster>()
                    .AddAsync(entity);

                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Country created successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    $"Create failed. {ex.Message}",
                    false
                );
            }
        }

        public async Task<ApiResponse<string>> UpdateAsync(CountryDto dto)
        {
            try
            {
                var entity = await _unitOfWork
                    .Repository<CountryMaster>()
                    .GetByIdAsync(dto.CountryId);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<string>(
                        null!,
                        "Country not found.",
                        false
                    );

                var duplicate = (await _unitOfWork.Repository<CountryMaster>()
                    .FindAsync(x =>
                        !x.IsDeleted &&
                        x.CountryId != dto.CountryId &&
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.CountryName.ToLower() == dto.CountryName.ToLower()
                    )).Any();

                if (duplicate)
                    return new ApiResponse<string>(
                        null!,
                        "Duplicate Country exists.",
                        false
                    );

                entity.CompanyId = dto.CompanyId;
                entity.RegionId = dto.RegionId;
                entity.CountryName = dto.CountryName;
                entity.IsActive = dto.IsActive;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = dto.UserId;

                _unitOfWork.Repository<CountryMaster>()
                    .Update(entity);

                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Country updated successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    $"Update failed. {ex.Message}",
                    false
                );
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork
                .Repository<CountryMaster>()
                .GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(
                    null!,
                    "Country not found.",
                    false
                );

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = entity.CreatedBy;

            _unitOfWork.Repository<CountryMaster>()
                .Update(entity);

            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>(
                "Country deleted successfully."
            );
        }
        public async Task<ApiResponse<IEnumerable<CountryDto>>> GetByCompanyRegion(
    int companyId,
    int regionId)
        {
            try
            {
                var list = (await _unitOfWork.Repository<CountryMaster>()
                    .FindAsync(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.CompanyId == companyId &&
                        x.RegionId == regionId
                    ))
                    .OrderBy(x => x.CountryName)
                    .Select(x => new CountryDto
                    {
                        CountryId = x.CountryId,
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        CountryName = x.CountryName,
                        IsActive = x.IsActive,
                        UserId = x.CreatedBy ?? 0
                    });

                return new ApiResponse<IEnumerable<CountryDto>>(
                    list,
                    "Countries fetched successfully"
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CountryDto>>(
                    null!,
                    $"Failed: {ex.Message}",
                    false
                );
            }
        }
    }
}
