using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IEarlyLogoutService
    {
        Task<EarlyLogoutRequest> CreateEarlyLogoutRequest(CreateEarlyLogoutRequestDto dto);
        Task<IEnumerable<EarlyLogoutRequest>> GetEarlyLogoutRequest(int companyId, int? regionId, int userId);
        Task<IEnumerable<EarlyLogoutApprovalListDto>> GetApprovalEarlyLogoutRequest(int companyId, int? regionId, int managerId);
        //Task<bool> UpdateEarlyLogout(UpdateEarlyLogoutDto dto);
        Task<bool> UpdateEarlyLogout(UpdateEarlyLogoutDto dto); // ✅ new
        Task<int> BulkApproveRejectEarlyLogout(BulkApproveRejectEarlyLogoutDto dto);

    }
}
