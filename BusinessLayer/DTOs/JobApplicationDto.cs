using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class JobApplicationDto
    {
        public int ApplicationId { get; set; }

        public string? CandidateName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? JobTitle { get; set; }

        public decimal? ExperienceYears { get; set; }
        public string? Technology { get; set; }
        public string? ResumeUrl { get; set; }

        public string? Status { get; set; }

        public DateTime? AppliedDate { get; set; }
    }
}
