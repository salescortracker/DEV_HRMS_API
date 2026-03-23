using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class CompanyModule
{
    public int CompanyModuleId { get; set; }

    public int? CompanyId { get; set; }

    public int? AppModuleId { get; set; }

    public bool? IsEnabled { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
