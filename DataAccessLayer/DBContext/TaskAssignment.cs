using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class TaskAssignment
{
    public int TaskId { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int UserId { get; set; }

    public string TaskName { get; set; } = null!;

    public int? ProjectId { get; set; }

    public string? AssignedTo { get; set; }

    public int PriorityId { get; set; }

    public int StatusId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? Comment { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual TaskStatus Status { get; set; } = null!;

    public virtual ICollection<TaskFile> TaskFiles { get; set; } = new List<TaskFile>();
}
