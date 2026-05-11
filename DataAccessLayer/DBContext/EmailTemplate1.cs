using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmailTemplate1
{
    public int TemplateId { get; set; }

    public string? TemplateName { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public bool? IsActive { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<EmailTemplateVariable> EmailTemplateVariables { get; set; } = new List<EmailTemplateVariable>();
}
