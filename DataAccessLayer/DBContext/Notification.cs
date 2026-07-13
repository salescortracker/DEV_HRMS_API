using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Message { get; set; }

    public string? Type { get; set; }

    public int? ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }
}
