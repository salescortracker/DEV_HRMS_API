

namespace BusinessLayer.DTOs
{
    public class AttachmentTypeDto
    {
        public int AttachmentTypeId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }

        public string AttachmentCategory { get; set; } = null!;
        public string AttachmentTypeName { get; set; } = null!;

        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
