using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class LeaveTypeGrade
{
    public int LeaveTypeGradeId { get; set; }

    public int LeaveTypeId { get; set; }

    public int GradeId { get; set; }

    public int LeaveDays { get; set; }

    public bool? IsActive { get; set; }

    public virtual Grade Grade { get; set; } = null!;

    public virtual LeaveType LeaveType { get; set; } = null!;
}
