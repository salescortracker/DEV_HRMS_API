using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
namespace BusinessLayer.DTOs
{
    public class EmployeeFormDto
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentName { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
        public DateOnly IssueDate { get; set; }
        [JsonIgnore]
        public string? FileName { get; set; }

        public string? FilePath { get; set; }
        public string? Remarks { get; set; }
        public bool IsConfidential { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // File Upload (Like Education)
        public IFormFile? UploadFile { get; set; }
        public string? EmployeeName { get; set; }
        public List<IFormFile>? DocumentFiles { get; set; }
        public List<string>? FilePaths { get; set; }
        public List<string>? FileNames { get; set; }
        public List<string>? EmployeeUploadedFiles { get; set; }
        public List<EmployeeUploadedFileDto> EmployeeUploads { get; set; } = new();

    }
}
