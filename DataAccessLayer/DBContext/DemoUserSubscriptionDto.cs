using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBContext
{
    public class DemoUserSubscriptionDto
    {
        public int UserID { get; set; }

        public string Company { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? DemoStart { get; set; }

        public DateTime? DemoExpiry { get; set; }

        public string Status { get; set; }

        public int TotalCompanies { get; set; }

        public int TotalUsers { get; set; }
    }
}
