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
        Task<int> CreateEarlyLogoutRequest(CreateEarlyLogoutRequestDto dto);
        Task<IEnumerable<EarlyLogoutRequest>> GetEarlyLogoutRequest(int companyId, int? regionId, int userId);
        Task<IEnumerable<EarlyLogoutApprovalListDto>> GetApprovalEarlyLogoutRequest(int companyId, int? regionId, int managerId);
        Task<bool> UpdateEarlyLogout(UpdateEarlyLogoutDto dto); 
        Task<int> BulkApproveRejectEarlyLogout(BulkApproveRejectEarlyLogoutDto dto);


        #region

        Task<int> CreateLateArrivalRequest(CreateLateArrivalRequestDto dto);

        Task<IEnumerable<LateLogin>> GetLateArrivalRequest(
            int companyId,
            int? regionId,
            int userId);

        Task<IEnumerable<LateArrivalApprovalListDto>> GetApprovalLateArrivalRequest(
            int companyId,
            int? regionId,
            int managerId);

        Task<bool> UpdateLateArrival(UpdateLateArrivalDto dto);

        Task<int> BulkApproveRejectLateArrival(
            BulkApproveRejectLateArrivalDto dto);
        #endregion

    }
}
