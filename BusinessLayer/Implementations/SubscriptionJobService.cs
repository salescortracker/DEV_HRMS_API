using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class SubscriptionJobService : ISubscriptionJobService
    {
        private readonly HRMSContext _context;

        public SubscriptionJobService(HRMSContext context)
        {
            _context = context;
        }

        public async Task ProcessExpiredSubscriptions()
        {
            var now = DateTime.UtcNow;

            var expiredSubs = await _context.UserSubscriptions
                .Where(x => x.IsActive && x.EndDate < now)
                .ToListAsync();

            foreach (var sub in expiredSubs)
            {
                sub.IsActive = false;
                sub.Status = "EXPIRED";
            }

            await _context.SaveChangesAsync();
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }

        public async Task<Invoice> CreateInvoiceAsync(int userId, int planId, string paymentId, string orderId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(x => x.PlanId == planId);

            if (user == null || plan == null)
                throw new Exception("Invalid user or plan");

            decimal amount = plan.Price;
            decimal tax = 18;
            decimal taxAmount = (amount * tax) / 100;
            decimal total = amount + taxAmount;

            var invoice = new Invoice
            {
                InvoiceNumber = GenerateInvoiceNumber(),
                UserId = userId,
                CompanyId = user.CompanyId,
                RegionId = user.RegionId,
                PlanId = planId,

                Amount = amount,
                TaxPercentage = tax,
                TaxAmount = taxAmount,
                TotalAmount = total,

                Currency = "INR",
                PaymentId = paymentId,
                OrderId = orderId,

                BillingDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = plan.Price == 0 ? DateTime.UtcNow.AddDays(14) : DateTime.UtcNow.AddMonths(1),

                Status = "PAID",
                PaymentMethod = "Razorpay",

                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }


    }
}
