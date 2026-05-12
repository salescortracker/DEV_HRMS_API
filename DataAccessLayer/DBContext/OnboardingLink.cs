using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class OnboardingLink
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }
}
