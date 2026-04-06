

namespace BusinessLayer.DTOs
{
    public class AssetTypeDto
    {
        public int AssetTypeId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public string AssetTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public int AssetCategoryId { get; set; }   // ✅ ADD
    }
}
