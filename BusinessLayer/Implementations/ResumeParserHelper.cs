using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Packaging;
using ExcelDataReader;
//using ExcelDataReader.DataSet;
using UglyToad.PdfPig;
using System.Data;
using System.Text.RegularExpressions;

namespace BusinessLayer.Implementations
{
    public class ResumeParserHelper : IResumeParserHelper
    {
        public string ExtractText(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            using var stream = file.OpenReadStream();

            return ext switch
            {
                ".pdf" => ReadPdf(stream),
                ".docx" => ReadDocx(stream),
                ".xlsx" or ".xls" => ReadExcel(stream),
                _ => ""
            };
        }
        string ReadPdf(Stream stream)
        {
            using var pdf = PdfDocument.Open(stream);
            return string.Join(" ", pdf.GetPages().Select(p => p.Text));
        }

        string ReadDocx(Stream stream)
        {
            using var doc = WordprocessingDocument.Open(stream, false);
            return doc.MainDocumentPart!.Document.Body!.InnerText;
        }

        string ReadExcel(Stream stream)
        {
            System.Text.Encoding.RegisterProvider(
                System.Text.CodePagesEncodingProvider.Instance);

            using var reader = ExcelReaderFactory.CreateReader(stream);
            var ds = reader.AsDataSet();

            return string.Join(" ",
                ds.Tables.Cast<DataTable>()
                  .SelectMany(t => t.Rows.Cast<DataRow>())
                  .SelectMany(r => r.ItemArray)
            );
        }

        public object ParseCandidate(string text)
        {
            return new
            {
                firstName = ExtractName(text).first,
                lastName = ExtractName(text).last,
                email = Regex.Match(text, @"[\w\.-]+@[\w\.-]+\.\w+").Value,
                mobile = Regex.Match(text, @"\b\d{10}\b").Value,
                skills = ExtractSkills(text),
                experience = ExtractExperience(text),
                qualification = ExtractEducation(text),
                dateOfBirth = ExtractDOB(text)
            };
        }

        (string first, string last) ExtractName(string text)
        {
            var match = Regex.Match(text, @"Name[:\s]+([A-Za-z]+)\s+([A-Za-z]+)");
            return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : ("", "");
        }

        string ExtractSkills(string text)
        {
            string[] known = { "Angular", "React", "C#", ".NET", "SQL", "Java", "Python" };
            return string.Join(", ", known.Where(s => text.Contains(s, StringComparison.OrdinalIgnoreCase)));
        }

        string ExtractExperience(string text)
        {
            var match = Regex.Match(text, @"(\d+)\+?\s+years");
            return match.Value;
        }

        string ExtractEducation(string text)
        {
            var match = Regex.Match(text, @"(B\.?Tech|M\.?Tech|BCA|MCA|MBA)");
            return match.Value;
        }
       
        string ExtractDOB(string text)
        {
            // Common date formats
            var patterns = new[]
            {
                 @"\b\d{1,2}(st|nd|rd|th)\s+(January|February|March|April|May|June|July|August|September|October|November|December)\s+\d{4}\b",
                @"\b\d{1,2}\s+(January|February|March|April|May|June|July|August|September|October|November|December)\s+\d{4}\b",
                @"\b\d{2}[/-]\d{2}[/-]\d{4}\b",              // 12/05/1998 or 12-05-1998
                @"\b\d{4}[/-]\d{2}[/-]\d{2}\b",              // 1998-05-12
                @"\b\d{2}\s+(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)[a-z]*\s+\d{4}\b", // 12 May 1998
                @"Date of Birth[:\s]+([^\n\r]+)"            // fallback
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var raw = match.Value.Replace("Date of Birth:", "").Trim();

                    // Remove st, nd, rd, th
                    raw = Regex.Replace(raw, @"(st|nd|rd|th)", "", RegexOptions.IgnoreCase);

                    if (DateTime.TryParse(raw, out DateTime dob))
                    {
                        return dob.ToString("yyyy-MM-dd"); // ✅ FORMAT FOR UI
                    }
                    return raw; 
                }
            }

            return "";
        }
    }
}
