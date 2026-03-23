using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IGradeService
    {
        Task<ApiResponse<IEnumerable<GradeDto>>> GetAllAsync(int companyId);
        Task<GradeDto?> GetByIdAsync(int id);
        Task<GradeDto> AddAsync(GradeDto dto);
        Task<GradeDto> UpdateAsync(GradeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
