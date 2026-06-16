using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class CompanyPolicyDepartment
{
    public int Id { get; set; }

    public int PolicyId { get; set; }

    public int DepartmentId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual CompanyPoliciesMaster Policy { get; set; } = null!;
}
