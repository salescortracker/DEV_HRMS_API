using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class CandidateDocumentChecklist
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int RegionId { get; set; }

    public int OfferId { get; set; }

    public int CandidateId { get; set; }

    public string? AadharCard { get; set; }

    public string? PanCard { get; set; }

    public string? Passport { get; set; }

    public string? IdProof { get; set; }

    public string? OfferLetter { get; set; }

    public string? ExperienceLetter { get; set; }

    public string? RelievingLetter { get; set; }

    public string? HikeLetter { get; set; }

    public string Status { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
