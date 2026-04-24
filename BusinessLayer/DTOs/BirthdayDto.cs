using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class BirthdayDto
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public int? RegionId { get; set; }
        public int? EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public bool? IsActive { get; set; }
    }
}
