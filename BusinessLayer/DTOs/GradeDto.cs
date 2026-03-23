using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class GradeDto
    {
        public int gradeID { get; set; }
        public string gradeName { get; set; }

        public int companyID { get; set; }
        public int regionId { get; set; }

        public bool IsActive { get; set; }

        public string? companyName { get; set; }
        public string? regionName { get; set; }

        public int? userId { get; set; }
    }
}
