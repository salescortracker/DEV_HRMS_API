using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class LateLoginPolicy
{
    public int PolicyId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int UserId { get; set; }

    public int LateLoginCount { get; set; }

    public decimal Lopdays { get; set; }

    public string Loptype { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedBy { get; set; }
}
