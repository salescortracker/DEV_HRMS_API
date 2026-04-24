using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ILateLoginPolicyService
    {
        Task<IEnumerable<LateLoginPolicyDto>> GetAllPoliciesAsync(int userId);
        Task<LateLoginPolicyDto?> GetPolicyByIdAsync(int id);
        Task<IEnumerable<LateLoginPolicyDto>> SearchPoliciesAsync(object filter);
        Task<LateLoginPolicyDto> AddPolicyAsync(object model);
        Task<LateLoginPolicyDto> UpdatePolicyAsync(int id, object model);
        Task<bool> DeletePolicyAsync(int id);
    }
}
