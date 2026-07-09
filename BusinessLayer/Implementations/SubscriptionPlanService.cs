using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class SubscriptionPlanService: ISubscriptionPlanService
    {
        private readonly HRMSContext _context;

        public SubscriptionPlanService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<object?> GetUserSubscription(int userId)
        {
            return await _context.UserSubscriptions
                .Include(x => x.Plan)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.EndDate)
                .Select(x => new
                {
                    x.SubscriptionId,
                    x.Status,
                    x.StartDate,
                    x.EndDate,
                    x.PaymentStatus,

                    Plan = new
                    {
                        x.Plan.PlanId,
                        x.Plan.PlanName,
                        x.Plan.Price
                    }
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<object>> GetUserAllowedModules(int userId)
        {
            var subscriptionUserId = userId;

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);


            if (user.UserCompanyId.HasValue)
            {
                subscriptionUserId = user.UserCompanyId.Value;
            }


            var subscription = await _context.UserSubscriptions
                .Where(x => x.UserId == subscriptionUserId
                     && x.EndDate >= DateTime.UtcNow.Date)
                .OrderByDescending(x => x.EndDate)
                .FirstOrDefaultAsync();


            if (subscription == null)
                return new List<object>();


            var modules = await (
                from spm in _context.SubscriptionPlanModules
                join m in _context.Modules
                on spm.ModuleId equals m.ModuleId

                where spm.PlanId == subscription.PlanId
                && spm.IsAllowed == true
                && m.IsActive == true

                select new
                {
                    m.ModuleId,
                    m.ModuleName,
                    m.Route,
                    m.Icon
                }

            ).ToListAsync();


            return modules.Cast<object>().ToList();
        }
        public async Task<List<SubscriptionPlanDto>> GetPlansAsync()
        {
            return await _context.SubscriptionPlans1
                .Select(p => new SubscriptionPlanDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    Description = p.Description,
                    Price = p.Price,
                    DurationDays = p.DurationDays,
                    MaxUsers = p.MaxUsers,
                    MaxEmployees = p.MaxEmployees,
                    StorageLimitGB = p.StorageLimitGb,
                    Status = p.Status
                }).ToListAsync();
        }


        public async Task<SubscriptionPlanDto> GetPlanByIdAsync(int id)
        {
            var p = await _context.SubscriptionPlans1
                .FirstOrDefaultAsync(x => x.PlanId == id);

            if (p == null) return null;

            return new SubscriptionPlanDto
            {
                PlanId = p.PlanId,
                PlanName = p.PlanName,
                Description = p.Description,
                Price = p.Price,
                DurationDays = p.DurationDays,
                MaxUsers = p.MaxUsers,
                MaxEmployees = p.MaxEmployees,
                StorageLimitGB = p.StorageLimitGb,
                Status = p.Status
            };
        }

        public async Task<SubscriptionPlanDto> CreatePlanAsync(SubscriptionPlanDto dto)
        {
            var entity = new SubscriptionPlan1
            {
                PlanName = dto.PlanName,
                Description = dto.Description,
                Price = dto.Price,
                DurationDays = dto.DurationDays,
                MaxUsers = dto.MaxUsers,
                MaxEmployees = dto.MaxEmployees,
                StorageLimitGb = dto.StorageLimitGB,
                Status = true,
                CreatedDate = DateTime.Now
            };

            _context.SubscriptionPlans1.Add(entity);

            await _context.SaveChangesAsync();

            dto.PlanId = entity.PlanId;

            return dto;
        }

        public async Task<SubscriptionPlanDto> UpdatePlanAsync(int id, SubscriptionPlanDto dto)
        {
            var entity = await _context.SubscriptionPlans1.FindAsync(id);

            if (entity == null) return null;

            entity.PlanName = dto.PlanName;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.DurationDays = dto.DurationDays;
            entity.MaxUsers = dto.MaxUsers;
            entity.MaxEmployees = dto.MaxEmployees;
            entity.StorageLimitGb = dto.StorageLimitGB;
            entity.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<bool> DeletePlanAsync(int id)
        {
            var entity = await _context.SubscriptionPlans.FindAsync(id);

            if (entity == null) return false;

            _context.SubscriptionPlans.Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> SavePlanMenus(PlanMenuRequestDto request)
        {
            // Remove old mappings
            var old = _context.PlanRoleMenuMappings
                .Where(x => x.PlanId == request.PlanId && x.RoleId == request.RoleId);

            _context.PlanRoleMenuMappings.RemoveRange(old);
            await _context.SaveChangesAsync();

            // Insert new mappings
            foreach (var menuId in request.MenuIds)
            {
                _context.PlanRoleMenuMappings.Add(new PlanRoleMenuMapping
                {
                    PlanId = request.PlanId,
                    RoleId = request.RoleId,
                    MenuId = menuId
                });
            }

            await _context.SaveChangesAsync();

            return "Menus mapped successfully";
        }
        public async Task<List<MenuMaster>> GetAllMenus()
        {
            return await _context.MenuMasters
                .OrderBy(m => m.MenuId)
                .ToListAsync();
        }
        public async Task<List<MenuMaster>> GetMenusByType(string type)
        {
            if (type.ToLower() == "admin")
            {
                return await _context.AdminMenuMasters
                    .Where(x => x.IsActive==true)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new MenuMaster
                    {
                        MenuId = x.MenuId,
                        MenuName = x.MenuName,
                        ParentMenuId = x.ParentMenuId,
                        Url = x.Url
                    })
                    .ToListAsync();
            }
            else
            {
                return await _context.MenuMasters
                    .Where(x => x.IsActive == true)
                    .OrderBy(x => x.OrderNo)
                    .Select(x => new MenuMaster
                    {
                        MenuId = x.MenuId,
                        MenuName = x.MenuName,
                        ParentMenuId = x.ParentMenuId,
                        Url = x.Url
                    })
                    .ToListAsync();
            }
        }
    }
}
