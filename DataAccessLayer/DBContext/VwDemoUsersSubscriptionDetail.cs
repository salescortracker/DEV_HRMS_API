using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class VwDemoUsersSubscriptionDetail
{
    public int UserId { get; set; }

    public string? Company { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? DemoStart { get; set; }

    public DateTime? DemoExpiry { get; set; }

    public string? Status { get; set; }

    public int? TotalCompanies { get; set; }

    public int? TotalUsers { get; set; }
}
