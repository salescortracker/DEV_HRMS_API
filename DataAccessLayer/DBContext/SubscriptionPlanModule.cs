using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class SubscriptionPlanModule
{
    public int SubscriptionPlanModuleId { get; set; }

    public int PlanId { get; set; }

    public int ModuleId { get; set; }

    public bool IsAllowed { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
