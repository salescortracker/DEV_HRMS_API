using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class GeoLocationDto
    {
        public int GeoLocationId { get; set; }

        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int? UserId { get; set; }

        public string LocationName { get; set; }
        public string Address { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public int Radius { get; set; }

        public bool IsActive { get; set; }
    }
}