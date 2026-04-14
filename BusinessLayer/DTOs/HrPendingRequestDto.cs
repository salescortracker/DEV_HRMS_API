using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class HrPendingRequestDto
    {
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public string Email { get; set; }
    }
}
