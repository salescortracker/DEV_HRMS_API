using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public string? Route { get; set; }

    public string? Icon { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ParentModuleId { get; set; }

    public int? DisplayOrder { get; set; }

    public string? ModuleType { get; set; }
}
