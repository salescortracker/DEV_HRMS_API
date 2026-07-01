using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class NotificationDto
    {
        public string Type { get; set; }   
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
    }
}
