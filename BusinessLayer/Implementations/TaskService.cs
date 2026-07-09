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
        private readonly IEmailService _emailService;

        public TaskService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<ApiResponse<IEnumerable<TaskDto>>> GetAll(int userId)
        {
            // STEP 1: Get tasks
            var tasks = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .FindAsync(x => x.IsDeleted == false && x.UserId == userId);

            var taskList = tasks.ToList();

            // STEP 2: Get task ids
            var taskIds = taskList.Select(t => t.TaskId).ToList();

            // STEP 3: Load files
            var files = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskFile>()
                .FindAsync(f => taskIds.Contains(f.TaskId));

            // STEP 4: Map result
            var list = taskList.Select(x => new TaskDto
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
                Comment = x.Comment,

                // ✅ FILES
                TaskFilesList = files
                    .Where(f => f.TaskId == x.TaskId)
                    .Select(f => new TaskFileDto
                    {
                        TaskFileId = f.TaskFileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileType = f.FileType,
                        FileSize = f.FileSize
                    })
                    .ToList()
            });

            return new ApiResponse<IEnumerable<TaskDto>>(list);
        }

        public async Task<ApiResponse<string>> CreateAsync(TaskDto dto)
        {
            // =========================
            // SAVE TASK
            // =========================
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

            await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .AddAsync(entity);

            await _unitOfWork.CompleteAsync();

            // =========================
            // SEND EMAIL TO ASSIGNED EMPLOYEE
            // =========================
            var assignedUser = await _unitOfWork.Repository<User>()
                .FindAsync(x => x.FullName == dto.AssignedTo);
            var projectName = await _unitOfWork.Repository<ProjectMaster>()
    .FindAsync(x => x.ProjectMasterId == dto.ProjectId);

var project = projectName.FirstOrDefault()?.ProjectName ?? "N/A";

var priorityName = await _unitOfWork.Repository<Priority>()
    .FindAsync(x => x.PriorityId == dto.PriorityId);

var priority = priorityName.FirstOrDefault()?.PriorityName ?? "N/A";

            var emp = assignedUser.FirstOrDefault();

            if (emp != null && !string.IsNullOrEmpty(emp.Email))
            {
                var body = $@"
                <html>
                <body style='font-family: Arial, Helvetica, sans-serif; color:#333;'>

                    <h2 style='color:#0d6efd;'>New Task Assigned</h2>

                    <p>Dear <b>{emp.FullName}</b>,</p>

                    <p>
                        A new task has been assigned to you. Please find the details below:
                    </p>

                    <table style='border-collapse:collapse; width:100%; max-width:600px;'>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Task Name</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{dto.TaskName}</td>
                        </tr>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Project</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{project}</td>
                        </tr>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Priority</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{priority}</td>
                        </tr>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Start Date</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{dto.StartDate:dd-MMM-yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Due Date</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{dto.DueDate:dd-MMM-yyyy}</td>
                        </tr>
                        <tr>
                            <td style='padding:8px; border:1px solid #ddd;'><b>Comments</b></td>
                            <td style='padding:8px; border:1px solid #ddd;'>{dto.Comment}</td>
                        </tr>
                    </table>

                    <p style='margin-top:20px;'>
                        Please log in to the Task Management portal to review the task details and update the progress accordingly.
                    </p>

                    <p>
                        Thank you,<br/>
                        <b>Task Management System</b>
                    </p>

                </body>
                </html>";

                await _emailService.SendEmailAsync(
                    emp.Email,
                    "New Task Assigned",
                    body
                );
            }

            // =========================
            // SEND EMAIL FOR @MENTIONS
            // =========================
            if (!string.IsNullOrEmpty(dto.Comment))
            {
                var mentions = ExtractMentions(dto.Comment);

                if (mentions.Count > 0)
                {
                    var users = await _unitOfWork.Repository<User>()
     .FindAsync(u =>
         mentions.Any(m =>
             u.FullName.ToLower().Contains(m.ToLower())
         )
     );

                    foreach (var user in users)
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            var mentionBody = $@"
                        <h3>You were mentioned in a task comment</h3>
                        <p><b>Task:</b> {dto.TaskName}</p>
                        <p><b>Comment:</b> {dto.Comment}</p>
                    ";

                            await _emailService.SendEmailAsync(
                                user.Email,
                                "You were mentioned in a Task",
                                mentionBody
                            );
                        }
                    }
                }
            }

            // =========================
            // SAVE FILES (YOUR EXISTING CODE)
            // =========================
            if (dto.Files != null && dto.Files.Count > 0)
            {
                string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadPath = Path.Combine(root, "Uploads", "Tasks");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in dto.Files)
                {
                    string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    string fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var taskFile = new DataAccessLayer.DBContext.TaskFile
                    {
                        TaskId = entity.TaskId,
                        FileName = file.FileName,
                        FilePath = $"Uploads/Tasks/{fileName}",
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskFile>()
                        .AddAsync(taskFile);
                }

                await _unitOfWork.CompleteAsync();
            }

            return new ApiResponse<string>("Task created successfully");
        }
        private List<string> ExtractMentions(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return new List<string>();

            return comment
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.StartsWith("@"))
                .Select(x => x.TrimStart('@')
                              .Replace(",", "")
                              .Replace(".", "")
                              .Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        public async Task<ApiResponse<string>> UpdateAsync(TaskDto dto)
        {
            var entity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .GetByIdAsync(dto.TaskId);

            if (entity == null)
                return new ApiResponse<string>(null!, "Not found", false);

            // store old assigned user for comparison
            var oldAssigned = entity.AssignedTo;

            // =========================
            // UPDATE TASK
            // =========================
            entity.TaskName = dto.TaskName;
            entity.ProjectId = dto.ProjectId;
            entity.AssignedTo = dto.AssignedTo;
            entity.PriorityId = dto.PriorityId;
            entity.StatusId = dto.StatusId;
            entity.StartDate = dto.StartDate;
            entity.DueDate = dto.DueDate;
            entity.Comment = dto.Comment;
            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = dto.UserId;

            _unitOfWork.Repository<DataAccessLayer.DBContext.TaskAssignment>()
                .Update(entity);

            await _unitOfWork.CompleteAsync();

            // Get assigned employee
            var assignedEmployee = (await _unitOfWork.Repository<User>()
                .FindAsync(x => x.FullName == entity.AssignedTo))
                .FirstOrDefault();

            if (assignedEmployee != null && assignedEmployee.ReportingTo != null)
            {
                // Get Manager
                var manager = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(assignedEmployee.ReportingTo.Value);


                var statusName = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskStatus>()
    .FindAsync(x => x.TaskStatusId == dto.StatusId);

                var status = statusName.FirstOrDefault()?.TaskStatusName ?? "N/A";

                if (manager != null && !string.IsNullOrEmpty(manager.Email))
                {
                    var project = (await _unitOfWork.Repository<ProjectMaster>()
                        .FindAsync(x => x.ProjectMasterId == dto.ProjectId))
                        .FirstOrDefault()?.ProjectName ?? "N/A";

                    var priority = (await _unitOfWork.Repository<Priority>()
                        .FindAsync(x => x.PriorityId == dto.PriorityId))
                        .FirstOrDefault()?.PriorityName ?? "N/A";

                    var body = $@"
                    <html>
                    <body style='font-family: Arial, Helvetica, sans-serif; color:#333;'>

                        <h2 style='color:#198754;'>Task Update Notification</h2>

                        <p>Dear <b>{manager.FullName}</b>,</p>

                        <p>
                            This is to inform you that the following task has been updated by
                            <b>{assignedEmployee.FullName}</b>.
                        </p>

                        <table style='border-collapse:collapse; width:100%; max-width:650px;'>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Employee</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{assignedEmployee.FullName}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Task Name</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{dto.TaskName}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Project</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{project}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Priority</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{priority}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Status</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{status}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Comments</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{dto.Comment}</td>
                            </tr>
                            <tr>
                                <td style='padding:8px; border:1px solid #ddd;'><b>Updated On</b></td>
                                <td style='padding:8px; border:1px solid #ddd;'>{DateTime.Now:dd-MMM-yyyy hh:mm tt}</td>
                            </tr>
                        </table>

                        <p style='margin-top:20px;'>
                            Kindly review the updated task status and take any necessary action.
                        </p>

                        <p>
                            Regards,<br/>
                            <b>Task Management System</b>
                        </p>

                    </body>
                    </html>";

                    await _emailService.SendEmailAsync(
                        manager.Email,
                        "Task Updated by Employee",
                        body);
                }
            }


            // =========================
            // EMAIL IF TASK REASSIGNED
            // =========================
            if (oldAssigned != dto.AssignedTo)
            {
                var assignedUser = await _unitOfWork.Repository<User>()
                    .FindAsync(x => x.FullName == dto.AssignedTo);

                var emp = assignedUser.FirstOrDefault();

                if (emp != null && !string.IsNullOrEmpty(emp.Email))
                {
                    await _emailService.SendEmailAsync(
                        emp.Email,
                        "Task Reassigned",
                        $"You have been assigned a new task: <b>{dto.TaskName}</b>"
                    );
                }
            }

            // =========================
            // EMAIL FOR @MENTIONS
            // =========================
            if (!string.IsNullOrEmpty(dto.Comment))
            {
                var mentions = ExtractMentions(dto.Comment);

                if (mentions.Count > 0)
                {
                    var users = await _unitOfWork.Repository<User>()
      .FindAsync(u =>
          mentions.Any(m =>
              u.FullName.ToLower().Contains(m.ToLower())
          )
      );

                    foreach (var user in users)
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await _emailService.SendEmailAsync(
                                user.Email,
                                "You were mentioned in a Task",
                                $@"
                            <h3>Task Comment Mention</h3>
                            <p><b>Task:</b> {dto.TaskName}</p>
                            <p><b>Comment:</b> {dto.Comment}</p>
                        "
                            );
                        }
                    }
                }
            }

            // =========================
            // DELETE FILES (your existing logic)
            // =========================
            if (!string.IsNullOrEmpty(dto.DeletedFileIds))
            {
                var deletedIds = System.Text.Json.JsonSerializer
                    .Deserialize<List<int>>(dto.DeletedFileIds);

                if (deletedIds != null && deletedIds.Count > 0)
                {
                    foreach (var fileId in deletedIds)
                    {
                        var fileEntity = await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskFile>()
                            .GetByIdAsync(fileId);

                        if (fileEntity != null)
                        {
                            string fullPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                fileEntity.FilePath!
                            );

                            if (File.Exists(fullPath))
                                File.Delete(fullPath);

                            _unitOfWork.Repository<DataAccessLayer.DBContext.TaskFile>()
                                .Remove(fileEntity);
                        }
                    }

                    await _unitOfWork.CompleteAsync();
                }
            }

            // =========================
            // ADD NEW FILES (your existing logic)
            // =========================
            if (dto.Files != null && dto.Files.Count > 0)
            {
                string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadPath = Path.Combine(root, "Uploads", "Tasks");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                foreach (var file in dto.Files)
                {
                    string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    string fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var taskFile = new DataAccessLayer.DBContext.TaskFile
                    {
                        TaskId = entity.TaskId,
                        FileName = file.FileName,
                        FilePath = $"Uploads/Tasks/{fileName}",
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        CreatedAt = DateTime.Now
                    };

                    await _unitOfWork.Repository<DataAccessLayer.DBContext.TaskFile>()
                        .AddAsync(taskFile);
                }

                await _unitOfWork.CompleteAsync();
            }

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
            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(userId);

            if (user == null)
                return new ApiResponse<IEnumerable<TaskDto>>(null!, "User not found", false);

            // STEP 1: Get tasks
            var tasks = await _unitOfWork.Repository<TaskAssignment>()
                .FindAsync(x => x.IsDeleted == false &&
                                x.AssignedTo == user.FullName);

            var taskList = tasks.ToList();

            // STEP 2: Load TaskFiles separately (NO INCLUDE)
            var taskIds = taskList.Select(t => t.TaskId).ToList();

            var files = await _unitOfWork.Repository<TaskFile>()
                .FindAsync(f => taskIds.Contains(f.TaskId));

            // STEP 3: Map
            var result = taskList.Select(x => new TaskDto
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
                Comment = x.Comment,

                TaskFilesList = files
                    .Where(f => f.TaskId == x.TaskId)
                    .Select(f => new TaskFileDto
                    {
                        TaskFileId = f.TaskFileId,
                        FileName = f.FileName,
                        FilePath = f.FilePath,
                        FileType = f.FileType,
                        FileSize = f.FileSize
                    })
                    .ToList()
            });

            return new ApiResponse<IEnumerable<TaskDto>>(result);
        }

        public async Task<ApiResponse<IEnumerable<TaskDto>>> GetTaskReport(
      int companyId,
      int regionId,
      int? employeeId,
      int? statusId,
      int? priorityId,
      DateTime? fromDate,
      DateTime? toDate)
        {
            // STEP 1: Base query (company + region mandatory)
            var query = await _unitOfWork.Repository<TaskAssignment>()
                .FindAsync(x =>
                    x.IsDeleted == false &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId
                );

            var tasks = query.AsQueryable();

            // STEP 2: Filters
            if (employeeId.HasValue)
                tasks = tasks.Where(x => x.UserId == employeeId.Value);

            if (statusId.HasValue)
                tasks = tasks.Where(x => x.StatusId == statusId.Value);

            if (priorityId.HasValue)
                tasks = tasks.Where(x => x.PriorityId == priorityId.Value);

            if (fromDate.HasValue)
                tasks = tasks.Where(x =>
                    x.StartDate >= DateOnly.FromDateTime(fromDate.Value));

            if (toDate.HasValue)
                tasks = tasks.Where(x =>
                    x.DueDate <= DateOnly.FromDateTime(toDate.Value));

            // STEP 3: Map to DTO
            var result = tasks.Select(x => new TaskDto
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

            return new ApiResponse<IEnumerable<TaskDto>>(result);
        }


    }
}
