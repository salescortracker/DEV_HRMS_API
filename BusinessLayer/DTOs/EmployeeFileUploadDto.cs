using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class EmployeeFileUploadDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public List<IFormFile>? DocumentFiles { get; set; }
    }
}
