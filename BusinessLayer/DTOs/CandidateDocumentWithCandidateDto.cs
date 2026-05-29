using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CandidateDocumentWithCandidateDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }

        public int CandidateId { get; set; }
        public int OfferId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }

        public string Status { get; set; }

        public string AadharCard { get; set; }
        public string PanCard { get; set; }
        public string Passport { get; set; }
        public string IdProof { get; set; }
        public string OfferLetter { get; set; }
        public string ExperienceLetter { get; set; }
        public string RelievingLetter { get; set; }
        public string HikeLetter { get; set; }
    }
}