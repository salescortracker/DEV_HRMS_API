using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class LeaveTypeDesignation
{
    public int Id { get; set; }

    public int? LeaveTypeId { get; set; }

    public int? DesignationId { get; set; }

    public int? LeaveDays { get; set; }

    public int? CompanyId { get; set; }

    public int? RegionId { get; set; }
}
