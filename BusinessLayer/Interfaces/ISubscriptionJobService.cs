using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.DBContext;

namespace BusinessLayer.Interfaces
{
    public interface ISubscriptionJobService
    {
        Task ProcessExpiredSubscriptions();
        Task<Invoice> CreateInvoiceAsync(int userId, int planId, string paymentId, string orderId);

    }
}
