using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class CompanyEventDepartment
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int DepartmentId { get; set; }
}
