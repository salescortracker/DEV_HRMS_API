using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentController : ControllerBase
    {
        private readonly IRecruitmentService _service;
        private readonly IResumeParserHelper _parserHelper;
        public RecruitmentController(IRecruitmentService service, IResumeParserHelper parserHelper)
        {
            _service = service;
            _parserHelper = parserHelper;
        }
       
        //[HttpGet("GetDesignations/{companyId}/{regionId}")]
        //public async Task<IActionResult> GetDesignations(int companyId, int regionId)
        //{
        //    var data = await _service.GetDesignationsWithDepartmentAsync(companyId, regionId);
        //    return Ok(data);
        //}
        [HttpGet("GetNoticePeriods/{companyId}/{regionId}")]
        public async Task<IActionResult> GetNoticePeriods(int companyId, int regionId)
        {
            var data = await _service.GetNoticePeriodsAsync(companyId, regionId);
            return Ok(data);
        }
        [HttpGet("GetMaritalStatuses/{companyId}/{regionId}")]
        public async Task<IActionResult> GetMaritalStatuses(int companyId, int regionId)
        {
            var data = await _service.GetMaritalStatusesAsync(companyId, regionId);
            return Ok(data);
        }
        [HttpPost("SaveCandidate")]
        public async Task<IActionResult> SaveCandidate([FromForm] CandidateDto dto)
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = Path.Combine(root, "Uploads", "Resumes");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (dto.ResumeFile != null && dto.ResumeFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{dto.ResumeFile.FileName}";
                string fullPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await dto.ResumeFile.CopyToAsync(stream);

                dto.FileName = fileName;
                dto.FilePath = $"Uploads/Resumes/{fileName}";
            }

            int id = await _service.SaveCandidateAsync(dto);
            return Ok(new { message = "Candidate saved successfully", candidateId = id });
        }

        // 🔹 GET CANDIDATES
        [HttpGet("GetCandidates/{userId}/{companyId}/{regionId}")]

        public async Task<IActionResult> GetCandidates(int userId, int companyId, int regionId)
        {
            var data = await _service.GetCandidatesAsync(userId, companyId, regionId);
            return Ok(data);
        }

        // 🔹 MOVE STAGE
        [HttpPost("MoveStage")]
        public async Task<IActionResult> MoveStage(int candidateId, int stageId)
        {
            bool success = await _service.MoveStageAsync(candidateId, stageId);
            return success ? Ok() : BadRequest();
        }

        // 🔹 DELETE
        [HttpDelete("DeleteCandidate/{candidateId}")]
        public async Task<IActionResult> DeleteCandidate(int candidateId)
        {
            bool success = await _service.DeleteCandidateAsync(candidateId);
            return success ? Ok() : BadRequest();
        }

        [HttpPut("RejectCandidate/{candidateId}")]
        public async Task<IActionResult> RejectCandidate(int candidateId)
        {
            var result = await _service.RejectCandidateAsync(candidateId);

            if (!result)
                return NotFound(new { Message = "Candidate not found" });

            return Ok(new { Message = "Candidate rejected successfully" });
        }

        [HttpGet("GetCandidateById/{candidateId}")]
        public async Task<IActionResult> GetCandidateById(int candidateId)
        {
            var data = await _service.GetCandidateByIdAsync(candidateId);
            return data == null ? NotFound() : Ok(data);
        }
        [HttpPost("UpdateCandidate")]
        public async Task<IActionResult> UpdateCandidate([FromForm] CandidateDto dto)
        {
            var success = await _service.UpdateCandidateAsync(dto);
            return success ? Ok() : BadRequest();
        }


        [HttpGet("GetReferenceUsers/{companyId}/{regionId}")]
        public async Task<IActionResult> GetReferenceUsers(int companyId, int regionId)
        {
            var data = await _service.GetReferenceUsersAsync(companyId, regionId);
            return Ok(data);
        }

        ///////////Screening////////

        [HttpGet("GetRecruiters/{companyId}/{regionId}")]
        public async Task<IActionResult> GetRecruiters(int companyId, int regionId)
        {
            var data = await _service.GetRecruitersAsync(companyId, regionId);
            return Ok(data);
        }
        [HttpGet("GetScreeningCandidatesTopTable")]
        public async Task<IActionResult> GetScreeningCandidatesTopTable(int userId, string? department, string? designation)
        {

            var result = await _service
                .GetScreeningCandidatesTopTableAsync(
                    userId,
                    department,
                    designation);

            return Ok(result);
        }


        [HttpPost("SaveCandidateScreening")]
        public async Task<IActionResult> SaveScreening(
[FromBody] CandidateScreeningDto dto)
        {
            var result = await _service.SaveCandidateScreeningAsync(dto);

            if (!result)
                return BadRequest("Unable to save screening");

            return Ok(new { message = "Candidate moved to Interview stage" });
        }
        [HttpGet("GetScreeningRecords/{userId}/{companyId}/{regionId}")]
        public async Task<IActionResult> GetScreeningRecords(int userId, int companyId, int regionId)
        {
            var data = await _service.GetScreeningRecordsAsync(userId, companyId, regionId);
            return Ok(data);
        }
        [HttpPut("UpdateScreening")]
        public async Task<IActionResult> UpdateScreening([FromBody] CandidateScreeningDto dto)
        {
            var result = await _service.UpdateCandidateScreeningAsync(dto);
            return result ? Ok() : BadRequest("Unable to update screening");
        }


        ////////////Interview
        [HttpGet("GetScreeningCandidatesTopTableInterview")]
        public async Task<IActionResult> GetScreeningCandidatesTopTableInterview(int userId, string? department, string? designation)
        {
            var result = await _service
               .GetScreeningCandidatesTopTableInterviewAsync(userId, department, designation);

            return Ok(result);
        }

        [HttpGet("GetInterviewLevels/{companyId}/{regionId}")]
        public async Task<IActionResult> GetInterviewLevels(int companyId, int regionId)
        {
            var data = await _service.GetInterviewLevelsAsync(companyId, regionId);
            return Ok(data);
        }
        [HttpPost("SaveCandidateInterview")]
        public async Task<IActionResult> SaveCandidateInterview(
     [FromBody] CandidateInterviewDto dto)
        {
            var result = await _service.SaveCandidateInterviewAsync(dto);

            if (!result)
                return BadRequest("Unable to save interview");

            return Ok(new { message = "Interview scheduled successfully" });
        }
        [HttpGet("GetInterviewRecords/{userId}/{companyId}/{regionId}")]
        public async Task<IActionResult> GetInterviewRecords(int userId,
    int companyId,
    int regionId)
        {
            var data = await _service.GetInterviewRecordsAsync(userId, companyId, regionId);
            return Ok(data);
        }
        [HttpPost("UpdateCandidateInterview")]
        public async Task<IActionResult> UpdateCandidateInterview([FromBody] CandidateInterviewDto dto)
        {
            var result = await _service.UpdateCandidateInterviewAsync(dto);

            if (!result)
                return BadRequest("Unable to update interview");

            return Ok(new { message = "Interview updated successfully" });
        }




        /// Appointment screen

        [HttpGet("GetAppointments/{interviewerId}")]
        public async Task<IActionResult> GetAppointments(int interviewerId)
        {
            var data = await _service.GetAppointmentsForInterviewerAsync(interviewerId);

            return Ok(data);
        }
        [HttpGet("GetAppointmentCandidateDetails/{candidateId}")]
        public async Task<IActionResult> GetAppointmentCandidateDetails(int candidateId)
        {
            var data = await _service.GetAppointmentCandidateDetailsAsync(candidateId);
            return data == null ? NotFound() : Ok(data);
        }
        [HttpGet("GetDesignations/{companyId}/{regionId}")]
        public async Task<IActionResult> GetDesignations(int companyId, int regionId)
        {
            var data = await _service.GetDesignationsWithDepartmentAsync(companyId, regionId);
            return Ok(data);
        }
        [HttpGet("GetOfferCandidatesTopTable")]
        public async Task<IActionResult> GetOfferCandidatesTopTable(string department, string designation, int userId)
        {
            var result = await _service.GetOfferCandidatesTopTableAsync(department, designation, userId);

            return Ok(result);
        }
        [HttpPost("SaveCandidateOffer")]
        public async Task<IActionResult> SaveCandidateOffer(
    [FromBody] CandidateOfferDto dto)
        {
            var result = await _service.SaveCandidateOfferAsync(dto);

            if (!result)
                return BadRequest("Unable to save offer");

            return Ok(new { message = "Offer saved successfully" });
        }
        [HttpGet("GetOfferRecords/{userId}/{companyId}/{regionId}")]
        public async Task<IActionResult> GetOfferRecords(
    int userId,
    int companyId,
    int regionId)
        {
            var data = await _service.GetOfferRecordsAsync(userId, companyId, regionId);
            return Ok(data);
        }

        [HttpGet("GetHRUsers/{companyId}/{regionId}")]
        public async Task<IActionResult> GetHRUsers(int companyId, int regionId)
        {
            var data = await _service.GetHRUsersAsync(companyId, regionId);
            return Ok(data);
        }

        [HttpPost("SendOfferLetter/{offerId}")]
        public async Task<IActionResult> SendOfferLetter(int offerId)
        {
            await _service.SendOfferLetterAsync(offerId);
            return Ok(new { message = "Offer letter sent successfully" });
        }

        [HttpGet("DownloadOfferLetter/{offerId}")]
        public async Task<IActionResult> DownloadOfferLetter(int offerId)
        {
            var (bytes, fileName) = await _service.DownloadOfferLetterAsync(offerId);
            return File(bytes, "application/pdf", fileName);
        }

        //OnBoarding


        [HttpGet("GetOnboardingCandidatesTopTable")]
        public async Task<IActionResult> GetOnboardingCandidatesTopTable(int companyId, int regionId, string department, string designation)
        {
            var result = await _service.GetOnboardingCandidatesTopTableAsync(companyId, regionId, department, designation);

            return Ok(result);
        }

        [HttpPost("SaveCandidateOnboarding")]
        public async Task<IActionResult> SaveCandidateOnboarding([FromBody] CandidateOnboardingDTO dto)
        {
            int id = await _service.SaveCandidateOnboardingAsync(dto);
            return Ok(new { message = "Onboarding saved successfully", onboardingId = id });
        }
        [HttpGet("GetOnboardedCandidates")]
        public async Task<IActionResult> GetOnboardedCandidates(int companyId, int regionId)
        {
            var result = await _service.GetOnboardedCandidatesAsync(companyId, regionId);
            return Ok(result);
        }




        [HttpPost("ParseResumeCandidate")]
        [Consumes("multipart/form-data")]
        public IActionResult ParseResume([FromForm] ResumeUploadDto dto)
        {
            if (dto.ResumeFile == null || dto.ResumeFile.Length == 0)
                return BadRequest("No resume uploaded");

            string text = _parserHelper.ExtractText(dto.ResumeFile);
            var parsed = _parserHelper.ParseCandidate(text);

            return Ok(parsed);
        }
        [HttpPost("SubmitApplication")]
        public async Task<IActionResult> SubmitApplication([FromForm] JobApplicationDto dto)
        {
            var resume = Request.Form.Files.FirstOrDefault();

            var result = await _service.SubmitJobApplicationAsync(dto, resume);

            return Ok(new { applicationId = result });
        }
        [HttpGet("job-applications")]
        public async Task<IActionResult> GetJobApplications()
        {
            var data = await _service.GetJobApplicationsAsync();
            return Ok(data);
        }

        [HttpPost("assign-company-region")]
        public async Task<IActionResult> AssignCompanyRegion([FromBody] AssignCompanyRegionDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            var result = await _service.UpdateCompanyRegionAsync(
                dto.Email,
                dto.Mobile,
                dto.CompanyId,
                dto.RegionId,
                dto.UserId
            );

            if (!result)
                return NotFound("Candidate not found");

            return Ok(new
            {
                success = true,
                message = "Candidate, Experience, Qualification updated successfully"
            });
        }
        [HttpGet("recruitment-departments")]
        public async Task<IActionResult> GetRecruitmentDepartments(int companyId, int regionId)
        {
            var result = await _service
                .GetRecruitmentDepartmentsAsync(companyId, regionId);

            return Ok(result);
        }

        [HttpGet("recruitment-designations")]
        public async Task<IActionResult> GetRecruitmentDesignations(
            int companyId,
            int regionId)
        {
            var result = await _service
                .GetRecruitmentDesignationsAsync(companyId, regionId);

            return Ok(result);
        }

        [HttpPost("UploadCandidateDocuments")]
        public async Task<IActionResult> UploadCandidateDocuments([FromForm] UploadCandidateDocumentsDto dto)
        {
            await _service.UploadCandidateDocumentsAsync(dto);

            return Ok(new
            {
                message = "Documents uploaded successfully"
            });
        }
        [HttpGet("GetOfferById/{offerId}")]
        public async Task<IActionResult> GetOfferById(int offerId)
        {
            var result = await _service.GetOfferByIdAsync(offerId);

            if (result == null)
                return BadRequest("Invalid Offer Id");

            return Ok(result);
        }
        [HttpGet("GetAllCandidateDocuments")]
        public async Task<IActionResult> GetAllCandidateDocuments(int companyId, int regionId)
        {
            var result = await _service.GetAllCandidateDocuments(companyId, regionId);
            return Ok(result);
        }
        [HttpPost("UpdateChecklistStatus")]
        public async Task<IActionResult> UpdateChecklistStatus(int offerId, int companyId, int regionId, string status)
        {
            var result = await _service.UpdateChecklistStatusAsync(offerId, companyId, regionId, status);

            if (!result)
                return BadRequest("Update failed");

            return Ok("Updated successfully");
        }



    }
}
