using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class BirthdayEmployeeDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? IsActive { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
