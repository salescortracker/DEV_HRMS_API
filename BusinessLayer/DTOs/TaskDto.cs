using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.DTOs
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public int UserId { get; set; }

        public string TaskName { get; set; } = string.Empty;
        public int? ProjectId { get; set; }
        public string? AssignedTo { get; set; }

        public int PriorityId { get; set; }
        public int StatusId { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? DueDate { get; set; }

        public string? Comment { get; set; }

        public List<IFormFile>? Files { get; set; }   // 🔥 important
        public List<TaskFileDto>? TaskFilesList { get; set; }
        public string? DeletedFileIds { get; set; }
    }
}
