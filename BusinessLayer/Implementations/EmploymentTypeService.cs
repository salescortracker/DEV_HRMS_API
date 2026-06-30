using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;


namespace BusinessLayer.Implementations
{
    public class EmploymentTypeService: IEmploymentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmploymentTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<EmploymentTypeDto>>> GetAll(int userId)
        {
            var list = (await _unitOfWork.Repository<Employmenttype>()
                .FindAsync(x => !x.IsDeleted && x.UserId == userId))
                .OrderByDescending(x => x.EmploymenttypeId)
                .ToList();

            var dto = list.Select(x => new EmploymentTypeDto
            {
                EmploymenttypeID = x.EmploymenttypeId,
                CompanyID = x.CompanyId,
                RegionID = x.RegionId,
                EmploymenttypeName = x.EmploymenttypeName,
                Description = x.Description,
                IsActive = x.IsActive
            });

            return new ApiResponse<IEnumerable<EmploymentTypeDto>>(dto, "Employment Type retrieved successfully.");
        }

        public async Task<ApiResponse<EmploymentTypeDto?>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Employmenttype>().GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<EmploymentTypeDto?>(null, "Not found", false);

            var dto = new EmploymentTypeDto
            {
                EmploymenttypeID = entity.EmploymenttypeId,
                CompanyID = entity.CompanyId,
                RegionID = entity.RegionId,
                EmploymenttypeName = entity.EmploymenttypeName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };

            return new ApiResponse<EmploymentTypeDto?>(dto, "Success");
        }

        public async Task<ApiResponse<string>> CreateAsync(EmploymentTypeDto dto)
        {
            var duplicate = (await _unitOfWork.Repository<Employmenttype>().FindAsync(x =>
                !x.IsDeleted &&
                x.CompanyId == dto.CompanyID &&
                x.RegionId == dto.RegionID &&
                x.EmploymenttypeName.ToLower() == dto.EmploymenttypeName.ToLower()))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!, "Duplicate Employment Type exists.", false);

            var entity = new Employmenttype
            {
                CompanyId = dto.CompanyID,
                RegionId = dto.RegionID,
                EmploymenttypeName = dto.EmploymenttypeName,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = dto.UserId ?? 0,
                CreatedBy = dto.UserId ?? 0
            };

            await _unitOfWork.Repository<Employmenttype>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Employment Type created successfully.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(EmploymentTypeDto dto)
        {
            var entity = await _unitOfWork.Repository<Employmenttype>()
                .GetByIdAsync(dto.EmploymenttypeID);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(null!, "Not found", false);

            // ✅ DUPLICATE CHECK (EXCLUDE CURRENT RECORD)
            var duplicate = (await _unitOfWork.Repository<Employmenttype>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyID &&
                    x.RegionId == dto.RegionID &&
                    x.EmploymenttypeName.ToLower() == dto.EmploymenttypeName.ToLower() &&
                    x.EmploymenttypeId != dto.EmploymenttypeID))
                .Any();

            if (duplicate)
                return new ApiResponse<string>(null!,
                    "Duplicate Employment Type exists.", false);

            // ✅ UPDATE
            entity.CompanyId = dto.CompanyID;
            entity.RegionId = dto.RegionID;
            entity.EmploymenttypeName = dto.EmploymenttypeName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = dto.UserId;

            _unitOfWork.Repository<Employmenttype>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Employment Type updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Employmenttype>()
                    .GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                {
                    return new ApiResponse<string>(
                        null!,
                        "Employment Type not found.",
                        false);
                }

                // Check whether Employment Type is assigned
                var isAssigned = (await _unitOfWork.Repository<EmployeePersonalDetail>()
    .FindAsync(x => x.EmployeeType == entity.EmploymenttypeName))
    .Any();

                if (isAssigned)
                {
                    return new ApiResponse<string>(
                        null!,
                        "You cannot delete this employment type. It is assigned to one or more employees.",
                        false);
                }

                // Soft Delete
                entity.IsDeleted = true;
                entity.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<Employmenttype>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>(
                    "Employment Type deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(
                    null!,
                    ex.Message,
                    false);
            }
        }

        public async Task<ApiResponse<IEnumerable<EmploymentTypeDto>>> GetByCompanyRegion(
       int companyId, int regionId)
        {
            var list = (await _unitOfWork.Repository<Employmenttype>()
                .FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive))
                .OrderBy(x => x.EmploymenttypeName)
                .ToList();

            var dto = list.Select(x => new EmploymentTypeDto
            {
                EmploymenttypeID = x.EmploymenttypeId,
                CompanyID = x.CompanyId,
                RegionID = x.RegionId,
                EmploymenttypeName = x.EmploymenttypeName,
                Description = x.Description,
                IsActive = x.IsActive
            });

            return new ApiResponse<IEnumerable<EmploymentTypeDto>>(dto, "Filtered Employment Types");
        }
    }
}
