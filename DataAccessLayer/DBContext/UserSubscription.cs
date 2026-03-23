using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class UserSubscription
{
    public int SubscriptionId { get; set; }

    public int UserId { get; set; }

    public int PlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual SubscriptionPlan1 Plan { get; set; } = null!;
}
