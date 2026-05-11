using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmailTemplate
{
    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string TemplateCode { get; set; } = null!;

    public string? TemplateType { get; set; }

    public string? ChannelType { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public string? Description { get; set; }

    public int? CompanyId { get; set; }

    public int? RegionId { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
