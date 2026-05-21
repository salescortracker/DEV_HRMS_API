using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ITaskService
    {
        Task<ApiResponse<IEnumerable<TaskDto>>> GetAll(int userId);
        Task<ApiResponse<string>> CreateAsync(TaskDto dto);
        Task<ApiResponse<string>> UpdateAsync(TaskDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<TaskDto>>> GetMyTasks(int userId);
        Task<ApiResponse<IEnumerable<TaskDto>>> GetTaskReport(
     int companyId,
     int regionId,
     int? employeeId,
     int? statusId,
     int? priorityId,
     DateTime? fromDate,
     DateTime? toDate);

    }
}
