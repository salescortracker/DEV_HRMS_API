using Microsoft.AspNetCore.Http;


namespace BusinessLayer.DTOs
{
    public class TicketRequestDto
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int CategoryId { get; set; }
        public string Subject { get; set; } = null!;
        public int PriorityId { get; set; }
        public string Description { get; set; } = null!;
        public IFormFile? Attachment { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? HrEmail { get; set; }
    }
}
