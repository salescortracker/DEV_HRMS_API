using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class AdminDashboardCountDto
    {
        public int TotalCompanies { get; set; }
        public int TotalRegions { get; set; }
        public int TotalEmployees { get; set; }
    }
}
