using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.Repositories.GeneralRepository;
using DataAccessLayer.DBContext;


namespace BusinessLayer.Implementations
{
    public class TaskStatusService : ITaskStatusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskStatusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<TaskStatusDto>>> GetAll(int userId)
        {
            try
            {
                var list = (await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>()
                    .FindAsync(x => !x.IsDeleted && x.UserId == userId))
                    .OrderByDescending(x => x.TaskStatusId)
                    .Select(x => new TaskStatusDto
                    {
                        TaskStatusId = x.TaskStatusId,
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        TaskStatusName = x.TaskStatusName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        UserId = x.UserId
                    });

                return new ApiResponse<IEnumerable<TaskStatusDto>>(list, "Task statuses retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TaskStatusDto>>(null!, $"Failed to retrieve task statuses. {ex.Message}", false);
            }
        }
        public async Task<ApiResponse<TaskStatusDto?>> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().GetByIdAsync(id);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<TaskStatusDto?>(null, "Task status not found.", false);

                var dto = new TaskStatusDto
                {
                    TaskStatusId = entity.TaskStatusId,
                    CompanyId = entity.CompanyId,
                    RegionId = entity.RegionId,
                    TaskStatusName = entity.TaskStatusName,
                    Description = entity.Description,
                    IsActive = entity.IsActive,
                    UserId = entity.UserId
                };

                return new ApiResponse<TaskStatusDto?>(dto, "Task status retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<TaskStatusDto?>(null, $"Failed to retrieve task status. {ex.Message}", false);
            }
        }
        public async Task<ApiResponse<string>> CreateAsync(TaskStatusDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.TaskStatusName))
                    return new ApiResponse<string>(null!, "Task Status Name is required.", false);

                var duplicate = (await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.TaskStatusName.ToLower() == dto.TaskStatusName.ToLower()))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Task Status exists.", false);

                var entity = new DataAccessLayer.DBContext.TaskStatus
                {
                    CompanyId = dto.CompanyId,
                    RegionId = dto.RegionId,
                    TaskStatusName = dto.TaskStatusName,
                    Description = dto.Description,
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.UserId,
                    UserId = dto.UserId
                };

                await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().AddAsync(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Task status created successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null!, $"Create failed. {ex.Message}", false);
            }
        }
        public async Task<ApiResponse<string>> UpdateAsync(TaskStatusDto dto)
        {
            try
            {
                var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().GetByIdAsync(dto.TaskStatusId);

                if (entity == null || entity.IsDeleted)
                    return new ApiResponse<string>(null!, "Task status not found.", false);

                var duplicate = (await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().FindAsync(x =>
                    !x.IsDeleted &&
                    x.TaskStatusId != dto.TaskStatusId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.TaskStatusName.ToLower() == dto.TaskStatusName.ToLower()))
                    .Any();

                if (duplicate)
                    return new ApiResponse<string>(null!, "Duplicate Task Status exists.", false);

                entity.TaskStatusName = dto.TaskStatusName;
                entity.Description = dto.Description;
                entity.CompanyId = dto.CompanyId;
                entity.RegionId = dto.RegionId;
                entity.IsActive = dto.IsActive;
                entity.ModifiedAt = DateTime.UtcNow;
                entity.ModifiedBy = dto.UserId;

                _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().Update(entity);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<string>("Task status updated successfully.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(null!, $"Update failed. {ex.Message}", false);
            }
        }
        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().GetByIdAsync(id);

            if (entity == null || entity.IsDeleted)
                return new ApiResponse<string>(null!, "Task status not found.", false);

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = entity.UserId;

            _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Task status deleted successfully.");
        }
        public async Task<ApiResponse<IEnumerable<TaskStatusDto>>> GetByCompanyRegion(int companyId, int regionId)
        {
            try
            {
                var list = (await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>()
                    .FindAsync(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.CompanyId == companyId &&
                        x.RegionId == regionId
                    ))
                    .OrderBy(x => x.TaskStatusName)
                    .Select(x => new TaskStatusDto
                    {
                        TaskStatusId = x.TaskStatusId,
                        CompanyId = x.CompanyId,
                        RegionId = x.RegionId,
                        TaskStatusName = x.TaskStatusName,
                        Description = x.Description,
                        IsActive = x.IsActive,
                        UserId = x.UserId
                    });

                return new ApiResponse<IEnumerable<TaskStatusDto>>(list, "Task statuses fetched successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<TaskStatusDto>>(null!, $"Failed: {ex.Message}", false);
            }
        }

    }
}
