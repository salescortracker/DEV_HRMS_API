using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IProjectMasterService
    {
        Task<List<ProjectMasterDto>> GetAllProjectsMasters(int userId);
        Task<ProjectMasterDto> CreateProject(ProjectMasterDto dto);
        Task<ProjectMasterDto> UpdateProject(ProjectMasterDto dto);
        Task<bool> DeleteProject(int id);
        Task<List<ProjectMasterDto>> GetProjectsByCompanyRegion(int companyId, int regionId);
    }
}
