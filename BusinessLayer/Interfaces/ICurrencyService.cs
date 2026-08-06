using BusinessLayer.Common;
using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;

namespace BusinessLayer.Interfaces
{
    public interface ICurrencyService
    {
        Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAll(int userId);
        Task<ApiResponse<string>> CreateAsync(CurrencyDto dto);
        Task<ApiResponse<string>> UpdateAsync(CurrencyDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<CurrencyDto>>> CurrencyDropDown(int companyId, int regionId);
        Task<List<CurrencyMaster>> GetByCompanyAndRegion(int companyId, int regionId);
    }
}
