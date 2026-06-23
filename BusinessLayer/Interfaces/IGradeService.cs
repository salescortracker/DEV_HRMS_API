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
        Task<ApiResponse<IEnumerable<GradeDto>>> GetAllAsync(int userId);
        Task<ApiResponse<GradeDto>> GetByIdAsync(int id);
        Task<ApiResponse<GradeDto>> AddAsync(GradeDto dto);
        Task<ApiResponse<GradeDto>> UpdateAsync(GradeDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<List<GradeDto>> GetGradesByCompanyRegionAsync(int companyId, int regionId);
    }
}
