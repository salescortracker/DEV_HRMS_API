using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class PlanMenuRequestDto
    {
        public int PlanId { get; set; }
        public int RoleId { get; set; }
        public List<int> MenuIds { get; set; }
    }
}
