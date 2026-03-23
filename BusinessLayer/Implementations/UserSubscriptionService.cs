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
    public class UserSubscriptionService: IUserSubscriptionService
    {
        private readonly HRMSContext _context;

        public UserSubscriptionService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<UserSubscriptionDto> ApplyPlanAsync(UserSubscriptionDto dto)
        {

            var plan = await _context.SubscriptionPlans1
                        .FirstOrDefaultAsync(x => x.PlanId == dto.PlanId);

            if (plan == null)
                throw new Exception("Plan not found");

            DateTime start = DateTime.Now;

            DateTime end = start.AddDays(plan.DurationDays);

            var entity = new UserSubscription
            {
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                StartDate = start,
                EndDate = end,
                Status = "true",
                CreatedDate = DateTime.Now
            };

            _context.UserSubscriptions.Add(entity);

            await _context.SaveChangesAsync();

            dto.SubscriptionId = entity.SubscriptionId;
            dto.StartDate = start;
            dto.EndDate = end;

            return dto;
        }
    }
}
