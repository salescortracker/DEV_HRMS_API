using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DTOs
{
    public class LoginResponseDto
    {
        public string? Error { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }

        public object? User { get; set; }

        public List<object> AllowedModules { get; set; } = new();
        public Guid SessionId { get; set; }
        public Guid BrowserSessionId { get; set; }
    }
}
