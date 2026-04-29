using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface IGeoLocationService
    {
        Task<IEnumerable<GeoLocationDto>> GetAllLocationsAsync(int userId);
        Task<GeoLocationDto?> GetLocationByIdAsync(int id);
        Task<IEnumerable<GeoLocationDto>> SearchLocationsAsync(object filter);
        Task<GeoLocationDto> AddLocationAsync(object model);
        Task<GeoLocationDto> UpdateLocationAsync(int id, object model);
        Task<bool> DeleteLocationAsync(int id);
        Task<IEnumerable<GeoLocationDto>> GetAllLocationsByCompanyRegionAsync(int companyId, int regionId);
    }
}
