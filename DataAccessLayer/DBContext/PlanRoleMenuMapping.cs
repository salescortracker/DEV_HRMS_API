using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class PlanRoleMenuMapping
{
    public int Id { get; set; }

    public int? PlanId { get; set; }

    public int? RoleId { get; set; }

    public int? MenuId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
