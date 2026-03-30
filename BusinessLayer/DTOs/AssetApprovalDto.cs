namespace BusinessLayer.DTOs
{
    public class AssetApprovalDto
    {
        public int AssetID { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string AssetCode { get; set; } = string.Empty;
        public string AssetLocation { get; set; } = string.Empty;
        public decimal AssetCost { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string ApprovalStatus { get; set; } = string.Empty; // Pending / Approved / Rejected
        public string EmployeeName { get; set; } = string.Empty;
        public int AssetType { get; internal set; }
        public string AssetTypeName { get; set; } = string.Empty;   // ✅ NEW
        public int? AssetCategory { get; internal set; }
        public int? Priority { get; internal set; }
        public string PriorityName { get; set; } = string.Empty;    // ✅ NEW

        public DateTime RequiredDate { get; internal set; }
    }
}
