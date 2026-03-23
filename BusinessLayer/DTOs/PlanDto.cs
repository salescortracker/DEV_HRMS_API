using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class PlanDto
    {
        public int PlanId { get; set; }

        public string PlanName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public int? MaxUsers { get; set; }

        public int? MaxEmployees { get; set; }

        public int? StorageLimitGB { get; set; }

        public bool Status { get; set; }
    }
}
