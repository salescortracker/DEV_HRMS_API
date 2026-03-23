using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class UpdateDemoExpiryDto
    {
        public int UserID { get; set; }

        public DateTime DemoExpiryDate { get; set; }
    }
}
