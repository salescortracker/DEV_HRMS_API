using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IAdminMenuMasterService
    {
        Task<IEnumerable<AdminMenuMasterDto>> GetAllMenusAsync();

        Task<AdminMenuMasterDto?> GetMenuByIdAsync(int id);

        Task<AdminMenuMasterDto> AddMenuAsync(AdminMenuMasterDto dto, int createdBy);

        Task<AdminMenuMasterDto> UpdateMenuAsync(int id, AdminMenuMasterDto dto, int modifiedBy);

        Task<bool> DeleteMenuAsync(int id);
    }
}
