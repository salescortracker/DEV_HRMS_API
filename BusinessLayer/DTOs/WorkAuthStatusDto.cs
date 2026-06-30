using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class WorkAuthStatusDto
    {
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public string StatusName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int UserId { get; set; }
    }
}
