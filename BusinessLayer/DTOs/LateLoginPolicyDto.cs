using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class LateLoginPolicyDto
    {
        public int PolicyId { get; set; }

        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int UserId { get; set; }

        public int LateLoginCount { get; set; }

        public decimal Lopdays { get; set; }

        public string Loptype { get; set; } = string.Empty;

        public bool? IsActive { get; set; }
    }
}
