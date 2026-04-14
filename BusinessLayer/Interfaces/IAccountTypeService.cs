using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAccountTypeService
    {
        Task<List<AccountTypeDto>> GetAccountTypeList(int userId);
        Task<bool> CreateAccountType(AccountTypeDto dto);
        Task<bool> UpdateAccountType(AccountTypeDto dto);
        Task<bool> DeleteAccountType(int id);

        Task<List<AccountTypeDto>> GetAccountTypesByCompanyRegion(int companyId, int regionId);
    }
}
