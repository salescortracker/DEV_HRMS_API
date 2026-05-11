using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class EmailTemplateVariable
{
    public int Id { get; set; }

    public int? TemplateId { get; set; }

    public string? VariableName { get; set; }

    public string? DisplayName { get; set; }

    public string? SampleValue { get; set; }

    public bool? IsRequired { get; set; }

    public virtual EmailTemplate1? Template { get; set; }
}
