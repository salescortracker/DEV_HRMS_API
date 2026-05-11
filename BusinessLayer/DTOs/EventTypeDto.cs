using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
   public class EventTypeDto
    {
        public int eventTypeID { get; set; }

        public string eventTypeName { get; set; }

        public string? description { get; set; }

        public bool isActive { get; set; }

        public int companyID { get; set; }

        public int regionId { get; set; }

        public string? companyName { get; set; }

        public string? regionName { get; set; }

        public int userId { get; set; }
    }
}
