using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class AssignCompanyRegionDto
    {
        public string Email { get; set; } = null!;
        public string Mobile { get; set; } = null!;
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int UserId { get; set; }
    }
}
