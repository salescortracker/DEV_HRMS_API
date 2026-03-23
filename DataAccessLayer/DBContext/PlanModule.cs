using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class PlanModule
{
    public int PlanModuleId { get; set; }

    public int? PlanId { get; set; }

    public int? AppModuleId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
