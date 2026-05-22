using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class AdminMenuMasterDto
    {
        public int MenuID { get; set; }

        public string MenuName { get; set; } = string.Empty;

        public int? ParentMenuID { get; set; }

        public string? Url { get; set; }

        public string? Icon { get; set; }

        public int? OrderNo { get; set; }

        public bool? IsActive { get; set; }
        public bool? CanView { get; set; }

        public bool? CanAdd { get; set; }

        public bool? CanEdit { get; set; }

        public bool? CanDelete { get; set; }

        public bool? CanApprove { get; set; }
    }
}
