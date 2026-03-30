using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class AssetAssignmentDto
    {
        public int? AssignmentId { get; set; }

        public int CompanyId { get; set; }
        public int RegionId { get; set; }

        public int RequestId { get; set; }
        public int AssetId { get; set; }

        public string? EmployeeName { get; set; }
        public string? AssetType { get; set; }

        public string? AssetName { get; set; }
        public string? AssetCode { get; set; }

        public DateTime AssignDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string? Remarks { get; set; }
    }
}
