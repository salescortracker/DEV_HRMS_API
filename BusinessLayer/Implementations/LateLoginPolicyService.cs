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
    public class LateLoginPolicyService : ILateLoginPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LateLoginPolicyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /* ================= GET ALL ================= */
        public async Task<IEnumerable<LateLoginPolicyDto>> GetAllPoliciesAsync(int userId)
        {
            var data = await _unitOfWork.Repository<LateLoginPolicy>().GetAllAsync();

            return data
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt) // 🔥 best practice
                .Select(MapToDto);
        }

        /* ================= GET BY ID ================= */
        public async Task<LateLoginPolicyDto?> GetPolicyByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<LateLoginPolicy>().GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        /* ================= SEARCH ================= */
        public async Task<IEnumerable<LateLoginPolicyDto>> SearchPoliciesAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var all = await _unitOfWork.Repository<LateLoginPolicy>().GetAllAsync();
            var query = all.AsQueryable();

            foreach (var prop in props)
            {
                var value = prop.GetValue(filter);
                if (value == null) continue;

                switch (prop.Name)
                {
                    case nameof(LateLoginPolicy.LateLoginCount):
                        query = query.Where(x => x.LateLoginCount == Convert.ToInt32(value));
                        break;

                    case nameof(LateLoginPolicy.CompanyId):
                        query = query.Where(x => x.CompanyId == Convert.ToInt32(value));
                        break;

                    case nameof(LateLoginPolicy.RegionId):
                        query = query.Where(x => x.RegionId == Convert.ToInt32(value));
                        break;

                    case nameof(LateLoginPolicy.Loptype):
                        query = query.Where(x => x.Loptype.Contains(value.ToString()!));
                        break;
                }
            }

            return query.ToList().Select(MapToDto);
        }

        /* ================= ADD ================= */
        public async Task<LateLoginPolicyDto> AddPolicyAsync(object model)
        {
            var entity = MapFromDynamic(model);
            entity.CreatedAt = DateTime.Now;

            var repo = _unitOfWork.Repository<LateLoginPolicy>();

            // 🔥 STEP 1: Get existing records for same User + Company + Region
            var existingPolicies = (await repo.GetAllAsync())
                .Where(x =>
                    x.UserId == entity.UserId &&
                    x.CompanyId == entity.CompanyId &&
                    x.RegionId == entity.RegionId &&
                    x.IsActive == true
                ).ToList();

            // 🔥 STEP 2: Deactivate old records
            foreach (var old in existingPolicies)
            {
                old.IsActive = false;
                old.ModifiedAt = DateTime.Now;
                repo.Update(old);
            }

            // 🔥 STEP 3: Always insert new record as ACTIVE
            entity.IsActive = true;

            await repo.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        /* ================= UPDATE ================= */
        public async Task<LateLoginPolicyDto> UpdatePolicyAsync(int id, object model)
        {
            var existing = await _unitOfWork.Repository<LateLoginPolicy>().GetByIdAsync(id);

            if (existing == null)
                throw new Exception("Policy not found");

            var data = MapFromDynamic(model);

            var repo = _unitOfWork.Repository<LateLoginPolicy>();

            // 🔥 STEP 1: Deactivate other active records (except current)
            var otherPolicies = (await repo.GetAllAsync())
                .Where(x =>
                    x.PolicyId != id &&
                    x.UserId == data.UserId &&
                    x.CompanyId == data.CompanyId &&
                    x.RegionId == data.RegionId &&
                    x.IsActive == true
                ).ToList();

            foreach (var old in otherPolicies)
            {
                old.IsActive = false;
                old.ModifiedAt = DateTime.Now;
                repo.Update(old);
            }

            // 🔥 STEP 2: Update current record
            existing.CompanyId = data.CompanyId;
            existing.RegionId = data.RegionId;
            existing.UserId = data.UserId;
            existing.LateLoginCount = data.LateLoginCount;
            existing.Lopdays = data.Lopdays;
            existing.Loptype = data.Loptype;
            existing.IsActive = true; // always active
            existing.ModifiedAt = DateTime.Now;

            repo.Update(existing);
            await _unitOfWork.CompleteAsync();

            return MapToDto(existing);
        }

        /* ================= DELETE ================= */
        public async Task<bool> DeletePolicyAsync(int id)
        {
            var entity = await _unitOfWork.Repository<LateLoginPolicy>().GetByIdAsync(id);
            if (entity == null) return false;

            _unitOfWork.Repository<LateLoginPolicy>().Remove(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /* ================= MAP FROM DYNAMIC ================= */
        private LateLoginPolicy MapFromDynamic(object model)
        {
            var json = JsonSerializer.Serialize(model);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var entity = new LateLoginPolicy();

            foreach (var kvp in dict!)
            {
                var prop = typeof(LateLoginPolicy).GetProperty(kvp.Key,
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
        private LateLoginPolicyDto MapToDto(LateLoginPolicy x)
        {
            return new LateLoginPolicyDto
            {
                PolicyId = x.PolicyId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                UserId = x.UserId,
                LateLoginCount = x.LateLoginCount,
                Lopdays = x.Lopdays,
                Loptype = x.Loptype,
                IsActive = x.IsActive
            };
        }
    }
}
