using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class DeleteComponentDto
    {
        public int ComponentId { get; set; }
        public int UserId { get; set; }
    }
}
