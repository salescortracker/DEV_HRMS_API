using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class EmployeeUploadedFileDto
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public int FileId { get; set; }
    }
}
