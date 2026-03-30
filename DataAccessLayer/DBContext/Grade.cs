using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Grade
{
    public int GradeId { get; set; }

    public string GradeName { get; set; } = null!;

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Designation> Designations { get; set; } = new List<Designation>();

    public virtual ICollection<LeaveTypeGrade> LeaveTypeGrades { get; set; } = new List<LeaveTypeGrade>();
}
