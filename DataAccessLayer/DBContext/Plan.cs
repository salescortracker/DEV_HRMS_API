using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Plan
{
    public int PlanId { get; set; }

    public string PlanName { get; set; } = null!;

    public int? MaxUsers { get; set; }

    public int? DurationInDays { get; set; }

    public int? DurationInMonths { get; set; }

    public decimal? Price { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
