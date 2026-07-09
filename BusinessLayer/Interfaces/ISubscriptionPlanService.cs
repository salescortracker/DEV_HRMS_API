using BusinessLayer.DTOs;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ISubscriptionPlanService
    {
        Task<List<SubscriptionPlanDto>> GetPlansAsync();

        Task<SubscriptionPlanDto> GetPlanByIdAsync(int id);

        Task<SubscriptionPlanDto> CreatePlanAsync(SubscriptionPlanDto dto);

        Task<SubscriptionPlanDto> UpdatePlanAsync(int id, SubscriptionPlanDto dto);

        Task<bool> DeletePlanAsync(int id);
        Task<string> SavePlanMenus(PlanMenuRequestDto request);
        Task<List<MenuMaster>> GetAllMenus();
        Task<List<MenuMaster>> GetMenusByType(string type);
        Task<object?> GetUserSubscription(int userId);
        Task<List<object>> GetUserAllowedModules(int userId);

    }
}
