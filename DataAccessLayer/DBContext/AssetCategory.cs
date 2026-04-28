using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class AssetCategory
{
    public int AssetCategoryId { get; set; }

    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public string AssetCategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<AssetType> AssetTypes { get; set; } = new List<AssetType>();

    public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();

    public virtual Company Company { get; set; } = null!;

    public virtual Region Region { get; set; } = null!;
}
