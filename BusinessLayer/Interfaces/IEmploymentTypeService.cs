using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEmploymentTypeService
    {
        Task<ApiResponse<IEnumerable<EmploymentTypeDto>>> GetAll(int userId);
        Task<ApiResponse<EmploymentTypeDto?>> GetByIdAsync(int id);
        Task<ApiResponse<string>> CreateAsync(EmploymentTypeDto dto);
        Task<ApiResponse<string>> UpdateAsync(EmploymentTypeDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
