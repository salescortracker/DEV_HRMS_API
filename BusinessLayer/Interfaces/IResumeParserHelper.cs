using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IResumeParserHelper
    {
        string ExtractText(IFormFile file);
        object ParseCandidate(string text);
    }
}
