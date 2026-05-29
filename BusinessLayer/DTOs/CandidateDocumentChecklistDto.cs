using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class UploadCandidateDocumentsDto
    {
        public int OfferId { get; set; }

        public int CandidateId { get; set; }

        public int CompanyId { get; set; }

        public int RegionId { get; set; }

        // ================= PERSONAL DOCUMENTS =================

        public IFormFile? AadharCard { get; set; }

        public IFormFile? PanCard { get; set; }

        public IFormFile? Passport { get; set; }

        public IFormFile? IdProof { get; set; }

        // ================= EMPLOYMENT DOCUMENTS =================

        public IFormFile? OfferLetter { get; set; }

        public IFormFile? ExperienceLetter { get; set; }

        public IFormFile? RelievingLetter { get; set; }

        public IFormFile? HikeLetter { get; set; }
    }
}
