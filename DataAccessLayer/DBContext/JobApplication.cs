using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class JobApplication
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

    public bool? IsActive { get; set; }
}
