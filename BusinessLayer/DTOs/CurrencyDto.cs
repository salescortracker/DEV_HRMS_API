using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class CurrencyDto
    {
        public int CurrencyId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
    }
}
