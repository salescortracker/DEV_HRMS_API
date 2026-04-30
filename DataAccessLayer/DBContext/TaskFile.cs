using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class TaskFile
{
    public int TaskFileId { get; set; }

    public int TaskId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TaskAssignment Task { get; set; } = null!;
}
