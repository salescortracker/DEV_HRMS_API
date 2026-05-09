using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ICountryService
    {
        Task<ApiResponse<IEnumerable<CountryDto>>> GetAll(int userId);

        Task<ApiResponse<CountryDto?>> GetByIdAsync(int id);

        Task<ApiResponse<string>> CreateAsync(CountryDto dto);

        Task<ApiResponse<string>> UpdateAsync(CountryDto dto);

        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<IEnumerable<CountryDto>>> GetByCompanyRegion(int companyId, int regionId);
    }
}
