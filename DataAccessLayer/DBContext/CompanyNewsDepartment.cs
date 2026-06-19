using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class CompanyNewsDepartment
{
    public int NewsDepartmentId { get; set; }

    public int NewsId { get; set; }

    public int DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual CompanyNewsMaster News { get; set; } = null!;
}
