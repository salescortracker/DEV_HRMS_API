using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class JobApplicationDto
    {
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string JobTitle { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public decimal ExperienceYears { get; set; }

        public string Technology { get; set; } // ✅ Comma separated

        public string ResumeUrl { get; set; }
    }
}
