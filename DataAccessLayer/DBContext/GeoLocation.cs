using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class GeoLocation
{
    public int GeoLocationId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int? UserId { get; set; }

    public string LocationName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public int Radius { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }
}
