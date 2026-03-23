using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UserSubscriptionDto
    {
        public int SubscriptionId { get; set; }

        public int UserId { get; set; }

        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Status { get; set; }
    }
}
