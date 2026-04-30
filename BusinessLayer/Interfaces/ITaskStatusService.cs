using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ITaskStatusService
    {
        Task<ApiResponse<IEnumerable<TaskStatusDto>>> GetAll(int userId);
        Task<ApiResponse<TaskStatusDto?>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(TaskStatusDto dto);
        Task<ApiResponse<string>> UpdateAsync(TaskStatusDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<TaskStatusDto>>> GetByCompanyRegion(int companyId, int regionId);
    }
}
