using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class AssetCategoryDto
    {
        public int AssetCategoryId { get; set; }

        public int CompanyId { get; set; }

        public int RegionId { get; set; }

        public string AssetCategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int UserId { get; set; }
    }
}
