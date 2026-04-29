using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GeoLocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /* ================= GET ALL ================= */
        public async Task<IEnumerable<GeoLocationDto>> GetAllLocationsAsync(int userId)
        {
            var data = await _unitOfWork.Repository<GeoLocation>().GetAllAsync();

            return data
                .Where(x => x.UserId == userId)   // 🔥 CHANGE HERE
                .OrderByDescending(x => x.CreatedAt)
                .Select(MapToDto);
        }

        /* ================= GetAllLocationsByCompanyRegion ================= */

        public async Task<IEnumerable<GeoLocationDto>> GetAllLocationsByCompanyRegionAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<GeoLocation>().GetAllAsync();

            return data
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive == true   // optional but recommended
                )
                .OrderByDescending(x => x.CreatedAt)
                .Select(MapToDto);
        }

        /* ================= GET BY ID ================= */
        public async Task<GeoLocationDto?> GetLocationByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<GeoLocation>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        /* ================= SEARCH ================= */
        public async Task<IEnumerable<GeoLocationDto>> SearchLocationsAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var all = await _unitOfWork.Repository<GeoLocation>().GetAllAsync();
            var query = all.AsQueryable();

            foreach (var prop in props)
            {
                var value = prop.GetValue(filter);
                if (value == null) continue;

                switch (prop.Name)
                {
                    case nameof(GeoLocation.CompanyId):
                        query = query.Where(x => x.CompanyId == Convert.ToInt32(value));
                        break;

                    case nameof(GeoLocation.RegionId):
                        query = query.Where(x => x.RegionId == Convert.ToInt32(value));
                        break;

                    case nameof(GeoLocation.LocationName):
                        query = query.Where(x => x.LocationName.Contains(value.ToString()!));
                        break;
                }
            }

            return query.ToList().Select(MapToDto);
        }

        /* ================= ADD ================= */
        public async Task<GeoLocationDto> AddLocationAsync(object model)
        {
            var entity = MapFromDynamic(model);
            entity.CreatedAt = DateTime.Now;
            entity.IsActive = true;

            await _unitOfWork.Repository<GeoLocation>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        /* ================= UPDATE ================= */
        public async Task<GeoLocationDto> UpdateLocationAsync(int id, object model)
        {
            var existing = await _unitOfWork.Repository<GeoLocation>().GetByIdAsync(id);

            if (existing == null)
                throw new Exception("Location not found");

            var data = MapFromDynamic(model);

            existing.CompanyId = data.CompanyId;
            existing.RegionId = data.RegionId;
            existing.UserId = data.UserId;
            existing.LocationName = data.LocationName;
            existing.Address = data.Address;
            existing.Latitude = data.Latitude;
            existing.Longitude = data.Longitude;
            existing.Radius = data.Radius;
            existing.IsActive = true;
            existing.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<GeoLocation>().Update(existing);
            await _unitOfWork.CompleteAsync();

            return MapToDto(existing);
        }

        /* ================= DELETE ================= */
        public async Task<bool> DeleteLocationAsync(int id)
        {
            var entity = await _unitOfWork.Repository<GeoLocation>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<GeoLocation>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /* ================= MAP FROM DYNAMIC ================= */
        private GeoLocation MapFromDynamic(object model)
        {
            var json = JsonSerializer.Serialize(model);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var entity = new GeoLocation();

            foreach (var kvp in dict!)
            {
                var prop = typeof(GeoLocation).GetProperty(kvp.Key,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);

                if (prop != null && kvp.Value != null)
                {
                    try
                    {
                        object value = kvp.Value;

                        if (value is JsonElement el)
                        {
                            switch (el.ValueKind)
                            {
                                case JsonValueKind.String:
                                    value = el.GetString();
                                    break;
                                case JsonValueKind.Number:
                                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                                        value = el.GetInt32();
                                    else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?))
                                        value = el.GetDecimal();
                                    break;
                                case JsonValueKind.True:
                                case JsonValueKind.False:
                                    value = el.GetBoolean();
                                    break;
                            }
                        }

                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        prop.SetValue(entity, Convert.ChangeType(value, targetType));
                    }
                    catch { }
                }
            }

            return entity;
        }

        /* ================= MAP TO DTO ================= */
        private GeoLocationDto MapToDto(GeoLocation x)
        {
            return new GeoLocationDto
            {
                GeoLocationId = x.GeoLocationId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                UserId = x.UserId,
                LocationName = x.LocationName,
                Address = x.Address,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Radius = x.Radius,
                IsActive = x.IsActive
            };
        }
    }
}
