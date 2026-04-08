using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class VisaTypeMasterDto
    {
        public int VisaTypeId { get; set; }

        public int CompanyId { get; set; }

        public int RegionId { get; set; }

        public string VisaTypeName { get; set; } = null!;

        public string? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
