using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
       

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }

        public async Task<ApiResponse<IEnumerable<TaskDto>>> GetAll(int userId)
        {
            var list = (await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .FindAsync(x => x.IsDeleted == false && x.UserId == userId))
                .Select(x => new TaskDto
                {
                    TaskId = x.TaskId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,
                    TaskName = x.TaskName,
                    ProjectId = x.ProjectId,
                    AssignedTo = x.AssignedTo,
                    PriorityId = x.PriorityId,
                    StatusId = x.StatusId,
                    StartDate = x.StartDate,
                    DueDate = x.DueDate,
                    Comment = x.Comment
                });

            return new ApiResponse<IEnumerable<TaskDto>>(list);
        }

        public async Task<ApiResponse<string>> CreateAsync(TaskDto dto)
        {
            var entity = new DataAccessLayer.DBContext.TaskAssignment
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                UserId = dto.UserId,
                TaskName = dto.TaskName,
                ProjectId = dto.ProjectId,
                AssignedTo = dto.AssignedTo,
                PriorityId = dto.PriorityId,
                StatusId = dto.StatusId,
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now,
                CreatedBy = dto.UserId,
                IsDeleted = false
            };

            await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            // 🔥 FILE UPLOAD
           

            return new ApiResponse<string>("Task created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(TaskDto dto)
        {
            var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .GetByIdAsync(dto.TaskId);

            if (entity == null)
                return new ApiResponse<string>(null!, "Not found", false);

            entity.TaskName = dto.TaskName;
            entity.ProjectId = dto.ProjectId;      // ✅ Added
            entity.StartDate = dto.StartDate;
            entity.AssignedTo = dto.AssignedTo;
            entity.PriorityId = dto.PriorityId;
            entity.StatusId = dto.StatusId;
            entity.DueDate = dto.DueDate;
            entity.Comment = dto.Comment;
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = dto.UserId;

            _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .GetByIdAsync(id);

            entity.IsDeleted = true;
            _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>("Deleted successfully");
        }
        public async Task<ApiResponse<IEnumerable<TaskDto>>> GetMyTasks(int userId)
        {
            // 🔥 Step 1: Get logged-in user
            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(userId);

            if (user == null)
                return new ApiResponse<IEnumerable<TaskDto>>(null!, "User not found", false);

            // 🔥 Step 2: Get tasks where AssignedTo = employee name/code
            var list = (await _unitOfWork.Repository<TaskAssignment>()
                .FindAsync(x => x.IsDeleted == false &&
                                x.AssignedTo == user.FullName))   // 👈 KEY LINE
                .Select(x => new TaskDto
                {
                    TaskId = x.TaskId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,
                    TaskName = x.TaskName,
                    ProjectId = x.ProjectId,
                    AssignedTo = x.AssignedTo,
                    PriorityId = x.PriorityId,
                    StatusId = x.StatusId,
                    StartDate = x.StartDate,
                    DueDate = x.DueDate,
                    Comment = x.Comment
                });

            return new ApiResponse<IEnumerable<TaskDto>>(list);
        }
    }
}
