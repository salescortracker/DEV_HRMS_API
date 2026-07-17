using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class DepartmentService:IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllAsync(int userId)
        {
            try
            {
                //var list = await _unitOfWork.Repository<Department>().FindAsync(d => !d.IsDeleted);
                var list = await _unitOfWork.Repository<Department>()
                    .FindAsync(d =>
                        !d.IsDeleted &&
                        d.UserId == userId 
                    );
                var dto = list.Where(x => x.UserId == userId).Select(d => new DepartmentDto
                {
                    departmentId = d.DepartmentId,

                    companyId = d.CompanyId,
                    regionId = d.RegionId,
                    departmentName = d.DepartmentName,
                    isActive = d.IsActive
                });
                return new ApiResponse<IEnumerable<DepartmentDto>>(dto, "Departments retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<DepartmentDto>>(null!, $"Failed to get departments. {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<DepartmentDto?>> GetByIdAsync(int id)
        {
            try
            {
                var d = await _unitOfWork.Repository<Department>().GetByIdAsync(id);
                if (d == null || d.IsDeleted)
                    return new ApiResponse<DepartmentDto?>(null, "Department not found.", false);

                var dto = new DepartmentDto
                {
                    departmentId = d.DepartmentId,
                    companyId = d.CompanyId,
                    regionId = d.RegionId,
                    //description = d.Description,
                    isActive = d.IsActive
                };
                return new ApiResponse<DepartmentDto?>(dto, "Department retrieved.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<DepartmentDto?>(null, $"Failed to get department. {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<DepartmentDto>> CreateAsync(CreateUpdateDepartmentDto dto)
        {
            try
            {
                var exists = (await _unitOfWork.Repository<Department>().FindAsync(d =>
                    !d.IsDeleted &&
                    d.CompanyId == dto.companyId &&
                    d.RegionId == dto.regionId &&
                    d.DepartmentName.ToLower() == dto.description.ToLower()))
                    .Any();

                if (exists)
                    return new ApiResponse<DepartmentDto>(null!, "Duplicate department exists.", false);

                var entity = new Department
                {
                    CompanyId = dto.companyId,
                    RegionId = dto.regionId,
                    DepartmentName = dto.departmentName,
                    IsActive = dto.isActive,
                    UserId = dto.userId,
                    // CreatedBy =dto.userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<Department>().AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                var resultDto = new DepartmentDto
                {
                    departmentId = entity.DepartmentId,
                    companyId = entity.CompanyId,
                    regionId = entity.RegionId,
                    departmentName = entity.DepartmentName,
                    isActive = entity.IsActive
                };

                return new ApiResponse<DepartmentDto>(resultDto, "Department created successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<DepartmentDto>(null!, $"Create failed. {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<DepartmentDto>> UpdateAsync(CreateUpdateDepartmentDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Department>().GetByIdAsync(dto.departmentId);
                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<DepartmentDto>(null!, "Department not found.", false);

                var dup = (await _unitOfWork.Repository<Department>().FindAsync(d =>
                    !d.IsDeleted &&
                    d.DepartmentId != dto.departmentId &&
                    d.CompanyId == dto.companyId &&
                    d.RegionId == dto.regionId &&
                    d.DepartmentName.ToLower() == dto.departmentName.ToLower())).Any();

                if (dup)
                    return new ApiResponse<DepartmentDto>(null!, "Duplicate department exists.", false);

                entity.CompanyId = dto.companyId;
                entity.RegionId = dto.regionId;
                //entity.Description = dto.description;
                entity.IsActive = dto.isActive;
                //entity.ModifiedBy = dto.userId;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.DepartmentName = dto.departmentName;

                _unitOfWork.Repository<Department>().Update(entity);
                await _unitOfWork.CompleteAsync();

                var resDto = new DepartmentDto
                {
                    departmentId = entity.DepartmentId,
                    companyId = entity.CompanyId,
                    regionId = entity.RegionId,
                    //description = entity.Description,
                    isActive = entity.IsActive
                };

                return new ApiResponse<DepartmentDto>(resDto, "Department updated successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<DepartmentDto>(null!, $"Update failed. {ex.Message}", false);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            try
            {
                // 🔹 Get Department using Repository
                var department = await _unitOfWork
                    .Repository<Department>()
                    .GetByIdAsync(id);

                if (department == null || department.IsDeleted)
                {
                    return new ApiResponse<object>(
                        null,
                        "Department not found",
                        false
                    );
                }

                // 🔹 CHECK DESIGNATION TABLE (using FindAsync)
                var designations = await _unitOfWork
                    .Repository<Designation>()
                    .FindAsync(d => d.DepartmentId == id && !d.IsDeleted);

                bool isUsed = designations.Any();

                if (isUsed)
                {
                    return new ApiResponse<object>(
                        null,
                        "Cannot delete department. It is mapped to Designations.",
                        false
                    );
                }

                // 🔹 HARD DELETE (since Delete() not available → use Remove pattern)
                department.IsDeleted = true; // fallback if physical delete not supported

                _unitOfWork.Repository<Department>().Update(department);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<object>(
                    null,
                    "Department deleted successfully",
                    true
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(
                    null,
                    $"Delete failed: {ex.Message}",
                    false
                );
            }
        }
        public async Task<ApiResponse<(int inserted, int duplicates, int failed)>> BulkInsertAsync(IEnumerable<CreateUpdateDepartmentDto> items, string createdBy)
        {
            int inserted = 0, duplicates = 0, failed = 0;
            try
            {
                foreach (var dto in items)
                {
                    try
                    {
                        var exists = (await _unitOfWork.Repository<Department>().FindAsync(d =>
                            !d.IsDeleted &&
                            d.CompanyId == dto.companyId &&
                            d.RegionId == dto.regionId &&
                            d.DepartmentName.ToLower() == dto.description.ToLower()))
                            .Any();

                        if (exists)
                        {
                            duplicates++;
                            continue;
                        }

                        var entity = new Department
                        {
                            CompanyId = dto.companyId,
                            RegionId = dto.regionId,
                            //Description = dto.description,
                            IsActive = dto.isActive,
                            // CreatedBy = createdBy,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.Repository<Department>().AddAsync(entity);
                        inserted++;
                    }
                    catch
                    {
                        failed++;
                    }
                }

                await _unitOfWork.CompleteAsync();
                return new ApiResponse<(int, int, int)>((inserted, duplicates, failed), $"{inserted} inserted, {duplicates} duplicates, {failed} failed");
            }
            catch (Exception ex)
            {
                return new ApiResponse<(int, int, int)>((inserted, duplicates, failed), $"Bulk insert failed. {ex.Message}", false);
            }
        }
    }
}
