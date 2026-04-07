using BusinessLayer.Common;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface ICurrencyService
    {
        Task<ApiResponse<IEnumerable<CurrencyDto>>> GetAll(int userId);
        Task<ApiResponse<string>> CreateAsync(CurrencyDto dto);
        Task<ApiResponse<string>> UpdateAsync(CurrencyDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<CurrencyDto>>> CurrencyDropDown(int companyId, int regionId);
    }
}
