using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class ChatbotKnowledge
{
    public int Id { get; set; }

    public string? Question { get; set; }

    public string? Keywords { get; set; }

    public string? Answer { get; set; }

    public string? CardType { get; set; }

    public string? FileUrl { get; set; }

    public bool? IsActive { get; set; }
}
