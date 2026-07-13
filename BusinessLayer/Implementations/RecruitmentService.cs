using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using DocumentFormat.OpenXml.Packaging;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf.Event;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.HWPF;
using NPOI.HWPF.Extractor;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Text;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace BusinessLayer.Implementations
{
    public class RecruitmentService : IRecruitmentService
    {
        private readonly ICompanyService _companyService;
        private readonly HRMSContext _hRMSContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;
        public RecruitmentService(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration, HRMSContext hRMSContext, ICompanyService companyService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _hRMSContext = hRMSContext;
            _companyService = companyService;
        }

        //public async Task<IEnumerable<object>> GetDesignationsWithDepartmentAsync(int companyId, int regionId)
        //{
        //    // Get designations
        //    var designations = await _unitOfWork.Repository<Designation>()
        //        .FindAsync(x =>
        //            x.CompanyId == companyId &&
        //            x.RegionId == regionId &&
        //            x.IsActive &&
        //            !x.IsDeleted);

        //    // Get departments
        //    var departments = await _unitOfWork.Repository<Department>()
        //        .FindAsync(x => x.IsActive && !x.IsDeleted);

        //    // Join manually
        //    var result = from d in designations
        //                 join dep in departments
        //                 on d.DepartmentId equals dep.DepartmentId into deptGroup
        //                 from dep in deptGroup.DefaultIfEmpty()
        //                 select new
        //                 {
        //                     designationId = d.DesignationId,
        //                     designationName = d.DesignationName,
        //                     departmentId = d.DepartmentId,
        //                     departmentName = dep != null ? dep.DepartmentName : ""
        //                 };

        //    return result;
        //}
        public async Task<IEnumerable<RecruitmentNoticePeriodDto>> GetNoticePeriodsAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<RecruitmentNoticePeriod>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive &&
                    !x.IsDeleted);

            return data.Select(x => new RecruitmentNoticePeriodDto
            {
                RecruitmentNoticePeriodID = x.RecruitmentNoticePeriodId,
                CompanyID = x.CompanyId,
                RegionID = x.RegionId,
                NoticePeriod = x.NoticePeriod,
                IsActive = x.IsActive,
                UserId = x.UserId
            });
        }
        public async Task<IEnumerable<MaritalStatusDto>> GetMaritalStatusesAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<MaritalStatus>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive &&
                    !x.IsDeleted);

            return data.Select(x => new MaritalStatusDto
            {
                MaritalStatusId = x.MaritalStatusId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                MaritalStatusName = x.MaritalStatusName,
                Description = x.Description,
                IsActive = x.IsActive,
                UserId = x.UserId ?? 0
            });
        }

        public async Task<int> SaveCandidateAsync(CandidateDto dto)
        {
            int year = DateTime.Now.Year;

            var lastSeq = (await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.CompanyId == dto.CompanyId && x.CreatedAt.Year == year))
                .OrderByDescending(x => x.CandidateId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastSeq != null && lastSeq.SeqNo != null)
            {
                var parts = lastSeq.SeqNo.Split('_'); // REC_2026_0005
                if (parts.Length == 3)
                    nextNumber = int.Parse(parts[2]) + 1;
            }

            dto.SeqNo = $"Seq_{year}_{nextNumber.ToString("D4")}";

            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var candidate = new Candidate
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    SeqNo = dto.SeqNo,
                    StageId = dto.StageId,
                    AppliedDate = dto.AppliedDate.HasValue
                        ? DateOnly.FromDateTime(dto.AppliedDate.Value)
                        : null,

                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Mobile = dto.Mobile,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth.HasValue
                        ? DateOnly.FromDateTime(dto.DateOfBirth.Value)
                        : null,

                    MaritalStatus = dto.MaritalStatus,
                    CurrentSalary = dto.CurrentSalary,
                    ExpectedSalary = dto.ExpectedSalary,
                    ReferenceSource = dto.ReferenceSource,
                    Department = dto.Department,
                    Designation = dto.Designation,
                    Skills = dto.Skills,
                    NoticePeriod = dto.NoticePeriod,
                    AnyOffers = dto.AnyOffers,
                    Location = dto.Location,
                    Reason = dto.Reason,

                    FileName = dto.FileName,
                    FilePath = dto.FilePath,

                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId
                };

                await _unitOfWork.Repository<Candidate>().AddAsync(candidate);
                await _unitOfWork.CompleteAsync();

                // 🔹 EXPERIENCE
                if (!string.IsNullOrEmpty(dto.ExperiencesJson))
                {
                    var experiences = JsonConvert
                        .DeserializeObject<List<CandidateExperienceDto>>(dto.ExperiencesJson);

                    var expEntities = experiences!.Select(e => new CandidateExperience
                    {
                        CandidateId = candidate.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromDate = DateOnly.FromDateTime(e.FromDate),
                        ToDate = DateOnly.FromDateTime(e.ToDate),
                        Designation = e.Designation,
                        Organization = e.Organization,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    });

                    await _unitOfWork.Repository<CandidateExperience>()
                        .AddRangeAsync(expEntities);
                }

                // 🔹 QUALIFICATION
                if (!string.IsNullOrEmpty(dto.QualificationsJson))
                {
                    var qualifications = JsonConvert
                        .DeserializeObject<List<CandidateQualificationDto>>(dto.QualificationsJson);

                    var qualEntities = qualifications!.Select(q => new CandidateQualification
                    {
                        CandidateId = candidate.CandidateId,
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        FromYear = q.FromYear,
                        ToYear = q.ToYear,
                        Qualification = q.Qualification,
                        BoardUniversity = q.BoardUniversity,
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    });

                    await _unitOfWork.Repository<CandidateQualification>()
                        .AddRangeAsync(qualEntities);
                }

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return candidate.CandidateId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // 🔹 GET CANDIDATES
        public async Task<IEnumerable<object>> GetCandidatesAsync(int userId, int companyId, int regionId)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => x.UserId == userId && x.IsActive);

            var stages = await _unitOfWork.Repository<StageMaster>().GetAllAsync();

            return candidates.Select(c =>
            {
                var stage = stages.FirstOrDefault(s => s.StageId == c.StageId);

                return new
                {
                    c.CandidateId,
                    c.SeqNo,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.Mobile,
                    c.Designation,
                    c.AppliedDate,

                    // IMPORTANT FIX
                    FileName = !string.IsNullOrWhiteSpace(c.FileName)
        ? c.FileName
        : c.FilePath,

                    c.FilePath,

                    StageName = stage?.StageName ?? "Unknown",
                    Progress = stage?.ProgressPct ?? 0
                };
            });
        }

        // 🔹 MOVE STAGE
        public async Task<bool> MoveStageAsync(int candidateId, int stageId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return false;

            candidate.StageId = stageId;
            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // 🔹 DELETE (SOFT DELETE)
        public async Task<bool> DeleteCandidateAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null)
                return false;

            // 🔥 HARD DELETE
            _unitOfWork.Repository<Candidate>().Remove(candidate);

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RejectCandidateAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null)
                return false;

            // Update stage to Rejected
            candidate.StageId = 12;

            // Optional
            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);

            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<CandidateDto?> GetCandidateByIdAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return null;

            var experiences = await _unitOfWork.Repository<CandidateExperience>()
                .FindAsync(x => x.CandidateId == candidateId);

            var qualifications = await _unitOfWork.Repository<CandidateQualification>()
                .FindAsync(x => x.CandidateId == candidateId);

            return new CandidateDto
            {
                SeqNo = candidate.SeqNo,
                CandidateId = candidate.CandidateId,
                AppliedDate = candidate.AppliedDate?.ToDateTime(TimeOnly.MinValue),
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Email = candidate.Email,
                Mobile = candidate.Mobile,
                Gender = candidate.Gender,
                DateOfBirth = candidate.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                MaritalStatus = candidate.MaritalStatus,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                ReferenceSource = candidate.ReferenceSource,
                Department = candidate.Department,
                Designation = candidate.Designation,
                Skills = candidate.Skills,
                NoticePeriod = candidate.NoticePeriod,
                AnyOffers = candidate.AnyOffers,
                Location = candidate.Location,
                Reason = candidate.Reason,
                Experiences = experiences.Select(e => new CandidateExperienceDto
                {
                    FromDate = e.FromDate.HasValue
? e.FromDate.Value.ToDateTime(TimeOnly.MinValue)
: DateTime.MinValue,

                    ToDate = e.ToDate.HasValue
? e.ToDate.Value.ToDateTime(TimeOnly.MinValue)
: DateTime.MinValue,
                    Designation = e.Designation,
                    Organization = e.Organization
                }).ToList(),
                Qualifications = qualifications.Select(q => new CandidateQualificationDto
                {
                    FromYear = q.FromYear,
                    ToYear = q.ToYear,
                    Qualification = q.Qualification,
                    BoardUniversity = q.BoardUniversity
                }).ToList()
            };
        }

        public async Task<bool> UpdateCandidateAsync(CandidateDto dto)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(dto.CandidateId);

            if (candidate == null) return false;
            candidate.AppliedDate = dto.AppliedDate.HasValue
               ? DateOnly.FromDateTime(dto.AppliedDate.Value)
               : null;

            candidate.FirstName = dto.FirstName;
            candidate.LastName = dto.LastName;
            candidate.Email = dto.Email;
            candidate.Mobile = dto.Mobile;
            candidate.Gender = dto.Gender;

            candidate.DateOfBirth = dto.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(dto.DateOfBirth.Value)
                : null;

            candidate.MaritalStatus = dto.MaritalStatus;

            candidate.CurrentSalary = dto.CurrentSalary;
            candidate.ExpectedSalary = dto.ExpectedSalary;

            candidate.ReferenceSource = dto.ReferenceSource;
            candidate.Designation = dto.Designation;
            candidate.Department = dto.Department;
            candidate.Skills = dto.Skills;
            candidate.NoticePeriod = dto.NoticePeriod;
            candidate.AnyOffers = dto.AnyOffers;
            candidate.Location = dto.Location;
            candidate.Reason = dto.Reason;
            candidate.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<Candidate>().Update(candidate);

            // 🔹 REMOVE OLD EXPERIENCES
            var oldExp = await _unitOfWork.Repository<CandidateExperience>()
                .FindAsync(x => x.CandidateId == dto.CandidateId);

            if (oldExp.Any())
                _unitOfWork.Repository<CandidateExperience>().RemoveRange(oldExp);

            // 🔹 REMOVE OLD QUALIFICATIONS
            var oldQual = await _unitOfWork.Repository<CandidateQualification>()
                .FindAsync(x => x.CandidateId == dto.CandidateId);

            if (oldQual.Any())
                _unitOfWork.Repository<CandidateQualification>().RemoveRange(oldQual);

            // 🔹 ADD NEW EXPERIENCES
            if (!string.IsNullOrEmpty(dto.ExperiencesJson))
            {
                var exp = JsonConvert.DeserializeObject<List<CandidateExperienceDto>>(dto.ExperiencesJson);
                await _unitOfWork.Repository<CandidateExperience>()
                    .AddRangeAsync(exp!.Select(e => new CandidateExperience
                    {
                        CandidateId = dto.CandidateId,
                        FromDate = DateOnly.FromDateTime(e.FromDate),
                        ToDate = DateOnly.FromDateTime(e.ToDate),
                        Designation = e.Designation,
                        Organization = e.Organization,
                        CreatedAt = DateTime.Now
                    }));
            }

            // 🔹 ADD NEW QUALIFICATIONS
            if (!string.IsNullOrEmpty(dto.QualificationsJson))
            {
                var qual = JsonConvert.DeserializeObject<List<CandidateQualificationDto>>(dto.QualificationsJson);
                await _unitOfWork.Repository<CandidateQualification>()
                    .AddRangeAsync(qual!.Select(q => new CandidateQualification
                    {
                        CandidateId = dto.CandidateId,
                        FromYear = q.FromYear,
                        ToYear = q.ToYear,
                        Qualification = q.Qualification,
                        BoardUniversity = q.BoardUniversity,
                        CreatedAt = DateTime.Now
                    }));
            }

            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IEnumerable<object>> GetReferenceUsersAsync(int companyId, int regionId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.Status == "Active");

            return users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName
            });
        }

        /// ////////////////screening///////////////////////

        public async Task<IEnumerable<object>> GetRecruitersAsync(int companyId, int regionId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                   x.RoleId == 1009 &&          // 🔥 ONLY RECRUITERS
                x.Status == "Active");

            return users.Select(u => new
            {
                UserId = u.UserId,
                FullName = u.FullName
            });
        }
        public async Task<IEnumerable<object>> GetScreeningCandidatesTopTableInterviewAsync(int userId, string? department, string? designation)
        {
            var query = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.UserId == userId &&
                    c.StageId == 3 &&
                    c.IsActive
                );

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(x => x.Department == department);
            }

            if (!string.IsNullOrEmpty(designation))
            {
                query = query.Where(x => x.Designation == designation);
            }

            return query.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                    ? c.FirstName
                    : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }


        public async Task<IEnumerable<object>> GetScreeningCandidatesTopTableAsync(int userId, string? department, string? designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .GetAllAsync();

            var filtered = candidates
                .Where(c =>
                    c.StageId == 2 &&
                    c.UserId == userId &&
                    c.IsActive);

            // 🔥 Department filter
            if (!string.IsNullOrEmpty(department))
            {
                filtered = filtered.Where(c => c.Department == department);
            }

            // 🔥 Designation filter
            if (!string.IsNullOrEmpty(designation))
            {
                filtered = filtered.Where(c => c.Designation == designation);
            }

            return filtered.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                    ? c.FirstName
                    : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }


        public async Task<bool> SaveCandidateScreeningAsync(CandidateScreeningDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var screening = new CandidateScreening
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    RecruiterId = dto.RecruiterId,
                    ScreeningStatus = dto.ScreeningStatus,
                    Remarks = dto.Remarks,
                    ScreeningDate = DateTime.Now,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<CandidateScreening>()
                    .AddAsync(screening);

                // 🔥 Move Candidate to INTERVIEW stage
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId)
                    ?? throw new Exception("Candidate not found");

                if (dto.ScreeningStatus == "Selected")
                {
                    candidate.StageId = 3;
                }
                else if (dto.ScreeningStatus == "Hold")
                {
                    candidate.StageId = 2;
                }
                else if (dto.ScreeningStatus == "Rejected")
                {
                    candidate.StageId = 12;
                }
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<CandidateScreeningDto>> GetScreeningRecordsAsync(int userId, int companyId, int regionId)
        {
            var screenings = await _unitOfWork.Repository<CandidateScreening>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!screenings.Any())
                return Enumerable.Empty<CandidateScreeningDto>();

            var candidateIds = screenings.Select(x => x.CandidateId).Distinct().ToList();
            var recruiterIds = screenings.Select(x => x.RecruiterId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            var recruiters = await _unitOfWork.Repository<User>()
                .FindAsync(x => recruiterIds.Contains(x.UserId));

            return screenings
                 .OrderByDescending(x => x.CreatedAt)
                 .Select(s =>
                 {
                     var candidate = candidates.FirstOrDefault(c => c.CandidateId == s.CandidateId);
                     var recruiter = recruiters.FirstOrDefault(r => r.UserId == s.RecruiterId);
                     return new CandidateScreeningDto
                     {
                         CompanyId = s.CompanyId,
                         RegionId = s.RegionId,
                         UserId = s.UserId,
                         CandidateId = s.CandidateId,
                         RecruiterId = s.RecruiterId,
                         ScreeningStatus = s.ScreeningStatus,
                         Remarks = s.Remarks,
                         ScreeningDate = s.ScreeningDate,
                         StageId = candidate?.StageId ?? 0,

                         SeqNo = candidate?.SeqNo,
                         CandidateName = candidate == null ? "" :
        string.IsNullOrEmpty(candidate.LastName)
            ? candidate.FirstName
            : $"{candidate.FirstName} {candidate.LastName}",

                         RecruiterName = recruiter?.FullName ?? "Unknown",
                         Mobile = candidate?.Mobile,
                         ExpectedSalary = candidate?.ExpectedSalary
                     };
                 });

        }

        public async Task<bool> UpdateCandidateScreeningAsync(CandidateScreeningDto dto)
        {
            var screening = (await _unitOfWork.Repository<CandidateScreening>()
                .FindAsync(x => x.CandidateId == dto.CandidateId))
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (screening == null) return false;

            screening.RecruiterId = dto.RecruiterId;
            screening.ScreeningStatus = dto.ScreeningStatus;
            screening.Remarks = dto.Remarks;
            screening.ScreeningDate = DateTime.Now;

            _unitOfWork.Repository<CandidateScreening>().Update(screening);
            await _unitOfWork.CompleteAsync();

            return true;
        }



        /////////////////Interview

        public async Task<IEnumerable<object>> GetScreeningCandidatesTopTableInterviewAsync(
int userId)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.UserId == userId &&
                    c.StageId == 3 &&                 // 🔥 ONLY SCREENING

                    c.IsActive
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }
        public async Task<IEnumerable<InterviewLevelDto>> GetInterviewLevelsAsync(int companyId, int regionId)
        {
            var data = await _unitOfWork.Repository<InterviewLevel>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&

                    x.IsActive &&
                    !x.IsDeleted);

            return data.Select(x => new InterviewLevelDto
            {
                InterviewLevelsID = x.InterviewLevelsId,
                CompanyID = x.CompanyId,
                RegionID = x.RegionId,
                InterviewLevels = x.InterviewLevels,
                IsActive = x.IsActive,
                UserId = x.UserId
            });
        }
        public async Task<bool> SaveCandidateInterviewAsync(CandidateInterviewDto dto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var candidateRepo = _unitOfWork.Repository<Candidate>();

                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId)
                    ?? throw new Exception("Candidate not found");

                // GET LEVEL NAME
                var levelRepo = _unitOfWork.Repository<InterviewLevel>();

                var levelData = await levelRepo.GetByIdAsync(dto.LevelNo);

                string levelName = levelData?.InterviewLevels ?? "";

                // STORE INTERVIEWERS FOR EMAIL AFTER COMMIT
                var interviewerList = new List<User>();
                var interviewers = new List<User>();

                foreach (var interviewerId in dto.InterviewerIds)
                {
                    var interviewer = await _unitOfWork
                        .Repository<User>()
                        .GetByIdAsync(interviewerId);

                    if (interviewer != null)
                    {
                        interviewers.Add(interviewer);
                        interviewerList.Add(interviewer); // emails kosam
                    }
                }

                var interview = new CandidateInterview
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    LevelNo = dto.LevelNo,

                    InterviewerId = string.Join(",", dto.InterviewerIds),

                    InterviewerName = string.Join(",",
                        interviewers.Select(x => x.FullName)),

                    InterviewDate = dto.InterviewDate,
                    Location = dto.Location,
                    MeetingLink = dto.MeetingLink,
                    Description = dto.Description,
                    Result = "Pending",
                    HrEmail = dto.HrEmail,
                    CreatedAt = DateTime.Now,
                    CreatedBy = dto.UserId
                };

                await _unitOfWork
                    .Repository<CandidateInterview>()
                    .AddAsync(interview);

                // ================= UPDATE CANDIDATE =================
                candidate.StageId = 4;
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                // ================= SAVE DATABASE FIRST =================
                await _unitOfWork.CompleteAsync();

                // IMPORTANT
                await transaction.CommitAsync();
                var ccList = new List<string>();

                // 1. Interviewer Emails
                var interviewerUsers = await _unitOfWork.Repository<User>()
                    .FindAsync(u => dto.InterviewerIds.Contains(u.UserId));

                ccList.AddRange(interviewerUsers
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .Select(x => x.Email));

                // 2. HR Emails (SINGLE STRING → SPLIT)
                if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                {
                    ccList.AddRange(dto.HrEmail.Split(',', StringSplitOptions.RemoveEmptyEntries));
                }

                // 3. Clean duplicates
                ccList = ccList
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                // ================= EMAIL TO INTERVIEWERS =================
                foreach (var interviewer in interviewerList)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(interviewer.Email))
                        {
                            string subject =
                                $"Interview Scheduled – {candidate.FirstName} {candidate.LastName} ({levelName})";

                            string body = $@"
                            <!DOCTYPE html>
                            <html>
                            <body style='font-family: Arial, Helvetica, sans-serif; background:#f4f6f9; padding:20px;'>

                            <div style='max-width:750px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; border:1px solid #e5e5e5;'>

                                <!-- HEADER -->
                                <div style='background:#198754; color:#ffffff; padding:18px 25px;'>
                                    <h2 style='margin:0;'>Interview Schedule Notification</h2>
                                </div>

                                <!-- BODY -->
                                <div style='padding:25px; color:#333;'>

                                    <p>Dear <strong>{interviewer.FullName}</strong>,</p>

                                    <p>
                                        We are pleased to inform you that an interview has been scheduled.
                                        Please find the details below and make yourself available accordingly.
                                    </p>

                                    <table style='width:100%; border-collapse:collapse; margin-top:15px;' border='1' cellpadding='10'>

                                        <tr style='background:#f8f9fa;'>
                                            <td width='35%'><strong>Candidate Name</strong></td>
                                            <td>{candidate.FirstName} {candidate.LastName}</td>
                                        </tr>

                                        <tr>
                                            <td><strong>Interview Level</strong></td>
                                            <td>{levelName}</td>
                                        </tr>

                                        <tr style='background:#f8f9fa;'>
                                            <td><strong>Date & Time</strong></td>
                                            <td>{dto.InterviewDate:dd-MMM-yyyy hh:mm tt}</td>
                                        </tr>

                                        <tr>
                                            <td><strong>Location</strong></td>
                                            <td>{dto.Location}</td>
                                        </tr>

                                        <tr style='background:#f8f9fa;'>
                                            <td><strong>Meeting Link</strong></td>
                                            <td>
                                                <a href='{dto.MeetingLink}' style='color:#198754; font-weight:bold;' target='_blank'>
                                                    Join Interview
                                                </a>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td><strong>Description</strong></td>
                                            <td>{dto.Description}</td>
                                        </tr>

                                    </table>

                                    <br/>

                                    <p>
                                        Kindly review the candidate profile before the interview and join 5 minutes prior to the scheduled time.
                                    </p>

                                    <p>
                                        For any changes or clarifications, please contact the HR team.
                                    </p>

                                    <br/>

                                    <p>
                                        Best Regards,<br/>
                                        <strong>HR Team</strong>
                                    </p>

                                </div>

                            </div>

                            </body>
                            </html>";

                            await _emailService.SendEmailAsync(
                                interviewer.Email,
                                subject,
                                body,
                                ccList

                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        string subject =
                              $"Interview Scheduled – {candidate.FirstName} {candidate.LastName}";
                        Console.WriteLine("Interviewer email failed: " + ex.Message);

                        string body = $@"
                            <h3>Interview Scheduled</h3>
                            <p>Dear {interviewer.FullName},</p>

                            <p>Interview Details:</p>
                            <table>
                            <tr><td>Candidate</td><td>{candidate.FirstName} {candidate.LastName}</td></tr>
                            <tr><td>Level</td><td>{dto.LevelNo}</td></tr>
                            <tr><td>Date</td><td>{dto.InterviewDate:yyyy-MM-dd HH:mm}</td></tr>
                            <tr><td>Location</td><td>{dto.Location}</td></tr>
                            </table>";

                        await _emailService.SendEmailAsync(interviewer.Email, subject, body);
                    }
                }


                // ================= EMAIL TO CANDIDATE =================
                try
                {
                    if (!string.IsNullOrEmpty(candidate.Email))
                    {
                        string candidateBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <body style='font-family: Arial, Helvetica, sans-serif; color:#333;'>

                            <div style='max-width:700px; margin:auto; border:1px solid #e5e5e5; border-radius:8px; overflow:hidden;'>

                                <div style='background:#0d6efd; color:white; padding:15px 20px;'>
                                    <h2 style='margin:0;'>Interview Invitation</h2>
                                </div>

                                <div style='padding:20px;'>

                                    <p>Dear <strong>{candidate.FirstName} {candidate.LastName}</strong>,</p>

                                    <p>
                                        Thank you for your interest in joining our organization.
                                        We are pleased to inform you that your profile has been shortlisted
                                        and your interview has been scheduled as per the details below.
                                    </p>

                                    <table style='width:100%; border-collapse:collapse;' border='1' cellpadding='8'>
                                        <tr style='background:#f8f9fa;'>
                                            <td width='35%'><strong>Interview Level</strong></td>
                                            <td>{levelName}</td>
                                        </tr>

                                        <tr>
                                            <td><strong>Date & Time</strong></td>
                                            <td>{dto.InterviewDate:dd-MMM-yyyy hh:mm tt}</td>
                                        </tr>

                                        <tr>
                                            <td><strong>Location</strong></td>
                                            <td>{dto.Location}</td>
                                        </tr>

                                        <tr>
                                            <td><strong>Meeting Link</strong></td>
                                            <td>
                                                <a href='{dto.MeetingLink}'
                                                   style='color:#0d6efd; font-weight:bold;'>
                                                    Join Interview
                                                </a>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td><strong>Remarks</strong></td>
                                            <td>{dto.Description}</td>
                                        </tr>
                                    </table>

                                    <br/>

                                    <p>
                                        Please ensure that you join the meeting a few minutes before the scheduled time.
                                        Kindly keep your resume and relevant documents readily available for discussion.
                                    </p>

                                    <p>
                                        If you have any questions or require assistance, please feel free to contact our HR team.
                                    </p>

                                    <br/>

                                    <p>
                                        Best Regards,<br/>
                                        <strong>HR Team</strong>
                                    </p>

                                </div>

                            </div>

                            </body>
                            </html>";

                        await _emailService.SendEmailAsync(
                            candidate.Email,
                            "Interview Scheduled",
                            candidateBody,
                            ccList
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Candidate email failed: " + ex.Message);
                }

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<CandidateInterviewDto>> GetInterviewRecordsAsync(
      int userId,
      int companyId,
      int regionId)
        {
            var interviews = await _unitOfWork.Repository<CandidateInterview>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!interviews.Any())
                return Enumerable.Empty<CandidateInterviewDto>();

            var candidateIds = interviews
                .Select(x => x.CandidateId)
                .Distinct()
                .ToList();

            var interviewerIds = interviews
                .SelectMany(x =>
                    (x.InterviewerId ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                )
                .Distinct()
                .ToList();

            var levelIds = interviews
                .Select(x => x.LevelNo)
                .Distinct()
                .ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            var interviewers = await _unitOfWork.Repository<User>()
                .FindAsync(x => interviewerIds.Contains(x.UserId));

            var levels = await _unitOfWork.Repository<InterviewLevel>()
                .FindAsync(x => levelIds.Contains(x.InterviewLevelsId));

            // GROUPING
            var groupedInterviews = interviews
                .GroupBy(x => new
                {
                    x.CandidateId,
                    x.LevelNo,
                    x.InterviewDate,
                    x.Location,
                    x.Description
                });

            var result = groupedInterviews.Select(group =>
            {
                var first = group.First();

                var candidate = candidates
                    .FirstOrDefault(c => c.CandidateId == first.CandidateId);

                var level = levels
                    .FirstOrDefault(l => l.InterviewLevelsId == first.LevelNo);

                // MULTIPLE INTERVIEWER NAMES
                var interviewerNames = string.Join(", ",
                    group.SelectMany(g =>
                        (g.InterviewerId ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id =>
                        {
                            var intId = int.Parse(id);
                            return interviewers.FirstOrDefault(i => i.UserId == intId)?.FullName;
                        })
                    )
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                );

                return new CandidateInterviewDto
                {
                    InterviewId = first.InterviewId,
                    CompanyId = first.CompanyId,
                    RegionId = first.RegionId,
                    UserId = first.UserId,
                    CandidateId = first.CandidateId,

                    LevelNo = first.LevelNo,

                    InterviewLevels = level?.InterviewLevels ?? "",

                    // COMMA SEPARATED NAMES
                    InterviewerName = interviewerNames,

                    InterviewDate = first.InterviewDate,

                    Location = first.Location,

                    MeetingLink = first.MeetingLink,

                    Description = first.Description,

                    Result = first.Result,

                    StageId = candidate?.StageId ?? 0,

                    SeqNo = candidate?.SeqNo,

                    CandidateName = candidate == null
                        ? ""
                        : $"{candidate.FirstName} {candidate.LastName}",

                    Mobile = candidate?.Mobile,

                    Department = candidate?.Department,

                    Designation = candidate?.Designation,

                    ExpectedSalary = candidate?.ExpectedSalary,
                    HrEmail = first.HrEmail,
                };
            })
            .OrderByDescending(x => x.InterviewDate)
            .ToList();

            return result;
        }


        /// ///appointment


        public async Task<bool> UpdateCandidateInterviewAsync(CandidateInterviewDto dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var interviewRepo = _unitOfWork.Repository<CandidateInterview>();

                var interview = await interviewRepo.GetByIdAsync(dto.InterviewId)
                       ?? throw new Exception("Interview record not found");

                // ================= UPDATE INTERVIEW =================

                interview.InterviewerId = string.Join(",", dto.InterviewerIds);
                interview.InterviewerName = dto.InterviewerName;
                interview.InterviewDate = dto.InterviewDate;
                interview.Location = dto.Location;
                interview.MeetingLink = dto.MeetingLink;
                interview.HrEmail = dto.HrEmail;
                interview.Result = dto.Result;
                interview.Description = dto.Description;
                interview.ModifiedAt = DateTime.Now;
                interview.ModifiedBy = dto.UserId;

                interviewRepo.Update(interview);

                // ================= UPDATE CANDIDATE =================

                var candidateRepo = _unitOfWork.Repository<Candidate>();

                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId)
                    ?? throw new Exception("Candidate not found");

                if (dto.Result == "Selected")
                {
                    candidate.StageId = 5;
                }
                else if (dto.Result == "Rejected")
                {
                    candidate.StageId = 12;
                }
                else if (dto.Result == "Level 1 Complete")
                {
                    candidate.StageId = 4;
                }
                else
                {
                    candidate.StageId = 4;
                }

                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                // ================= SAVE DB FIRST =================

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                // ================= GET LEVEL NAME =================

                var level = await _unitOfWork.Repository<InterviewLevel>()
                    .GetByIdAsync(interview.LevelNo);

                string levelName = level?.InterviewLevels ?? "";

                // ================= HR USERS =================

                var ccEmails = new List<string>();

                // 1. HR emails from DTO
                if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                {
                    ccEmails.AddRange(
                        dto.HrEmail.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(e => e.Trim())
                    );
                }

                // 2. Interviewer emails from DB
                var interviewerUsers = await _unitOfWork.Repository<User>()
                    .FindAsync(u => dto.InterviewerIds.Contains(u.UserId));

                ccEmails.AddRange(interviewerUsers
                    .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                    .Select(x => x.Email));

                // 3. Remove duplicates
                ccEmails = ccEmails
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                // ================= EMAILS =================

                try
                {
                    // ================= HR EMAIL =================

                    string hrSubject =
                                $"Interview Update – {candidate.FirstName} {candidate.LastName} ({levelName})";

                    string hrBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <body style='font-family: Arial, Helvetica, sans-serif; background:#f4f6f9; padding:20px;'>

                            <div style='max-width:750px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; border:1px solid #e5e5e5;'>

                                <div style='background:#0d6efd; color:#fff; padding:18px 25px;'>
                                    <h2 style='margin:0;'>Interview Status Updated</h2>
                                </div>

                                <div style='padding:25px;'>

                                    <p>Dear HR Team,</p>

                                    <p>The interview status has been updated. Please find the latest details below.</p>

                                    <table style='width:100%; border-collapse:collapse;' border='1' cellpadding='10'>

                                        <tr style='background:#f8f9fa;'>
                                            <td width='35%'><b>Candidate</b></td>
                                            <td>{candidate.FirstName} {candidate.LastName}</td>
                                        </tr>

                                        <tr>
                                            <td><b>Interview Level</b></td>
                                            <td>{levelName}</td>
                                        </tr>

                                        <tr style='background:#f8f9fa;'>
                                            <td><b>Status</b></td>
                                            <td><span style='color:#0d6efd; font-weight:bold;'>{dto.Result}</span></td>
                                        </tr>

                                        <tr>
                                            <td><b>Description</b></td>
                                            <td>{dto.Description}</td>
                                        </tr>
                                        <tr>
                                            <td><strong>Meeting Link</strong></td>
                                            <td>
                                                <a href='{dto.MeetingLink}'
                                                   style='color:#0d6efd; font-weight:bold;'>
                                                    Join Interview
                                                </a>
                                            </td>
                                        </tr>

                                    </table>

                                    <br/>

                                    <p>Regards,<br/><b>HR System</b></p>

                                </div>

                            </div>

                            </body>
                            </html>";
                    if (!string.IsNullOrWhiteSpace(candidate.Email))
                    {
                        await _emailService.SendEmailAsync(
                            candidate.Email,
                            hrSubject,
                            hrBody,
                            ccEmails.Any() ? ccEmails : null
                        );
                    }

                    // ================= CANDIDATE EMAIL =================

                    if (!string.IsNullOrEmpty(candidate.Email))
                    {
                        string candidateSubject =
                                $"Interview Update – {candidate.FirstName} {candidate.LastName}";

                        string candidateBody = $@"
                            <!DOCTYPE html>
                            <html>
                            <body style='font-family: Arial, Helvetica, sans-serif; background:#f4f6f9; padding:20px;'>

                            <div style='max-width:750px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; border:1px solid #e5e5e5;'>

                                <div style='background:#198754; color:#fff; padding:18px 25px;'>
                                    <h2 style='margin:0;'>Interview Update Notification</h2>
                                </div>

                                <div style='padding:25px;'>

                                    <p>Dear <b>{candidate.FirstName} {candidate.LastName}</b>,</p>

                                    <p>Your interview status has been updated. Please find the details below.</p>

                                    <table style='width:100%; border-collapse:collapse;' border='1' cellpadding='10'>

                                        <tr style='background:#f8f9fa;'>
                                            <td width='35%'><b>Interview Level</b></td>
                                            <td>{levelName}</td>
                                        </tr>

                                        <tr>
                                            <td><b>Status</b></td>
                                            <td><span style='color:#198754; font-weight:bold;'>{dto.Result}</span></td>
                                        </tr>

                                        <tr style='background:#f8f9fa;'>
                                            <td><b>Remarks</b></td>
                                            <td>{dto.Description}</td>
                                        </tr>
                                         <tr>
                                            <td><strong>Meeting Link</strong></td>
                                            <td>
                                                <a href='{dto.MeetingLink}'
                                                   style='color:#0d6efd; font-weight:bold;'>
                                                    Join Interview
                                                </a>
                                            </td>
                                        </tr>

                                    </table>

                                    <br/>

                                    <p>
                                        Thank you for your participation in the interview process.
                                    </p>

                                    <p>
                                        Regards,<br/>
                                        <b>HR Team</b>
                                    </p>

                                </div>

                            </div>

                            </body>
                            </html>";

                        await _emailService.SendEmailAsync(
                            candidate.Email,
                            candidateSubject,
                            candidateBody,
                            ccEmails.Any() ? ccEmails : null
                        );
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Email Error: " + ex.Message);
                }

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<CandidateAppointmentDto>> GetAppointmentsForInterviewerAsync(int interviewerId)
        {
            // STEP 1: DB query (ONLY simple filter)
            var interviews = await _unitOfWork.Repository<CandidateInterview>()
                .FindAsync(x =>
                    x.InterviewerId != null &&
                    x.Result == "Pending"
                );

            // STEP 2: Memory filter (safe CSV parsing)
            var filtered = interviews
                .Where(x =>
                    x.InterviewerId != null &&
                    x.InterviewerId
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => int.TryParse(id, out var val) ? val : 0)
                        .Contains(interviewerId)
                )
                .ToList();

            if (!filtered.Any())
                return Enumerable.Empty<CandidateAppointmentDto>();

            // STEP 3: Get candidates
            var candidateIds = filtered.Select(x => x.CandidateId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            // STEP 4: Map result
            return filtered
                .OrderByDescending(x => x.InterviewDate)
                .Select(iv =>
                {
                    var candidate = candidates.FirstOrDefault(c => c.CandidateId == iv.CandidateId);
                    if (candidate == null) return null;

                    return new CandidateAppointmentDto
                    {
                        InterviewId = iv.InterviewId,
                        CandidateId = iv.CandidateId,
                        SeqNo = candidate.SeqNo,
                        InterviewDate = iv.InterviewDate,
                        Designation = candidate.Designation,
                        Location = iv.Location,
                        Description = iv.Description
                    };
                })
                .Where(x => x != null)!;
        }



        public async Task<object?> GetAppointmentCandidateDetailsAsync(int candidateId)
        {
            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(candidateId);

            if (candidate == null) return null;

            return new
            {
                candidate.CandidateId,
                candidate.SeqNo,
                Name = string.IsNullOrEmpty(candidate.LastName)
                    ? candidate.FirstName
                    : $"{candidate.FirstName} {candidate.LastName}",
                candidate.Gender,
                candidate.Mobile,
                Expected = candidate.ExpectedSalary,
                Status = candidate.StageId,
                DateToJoin = DateTime.Now.AddDays(15)
            };
        }
        public async Task<IEnumerable<object>> GetDesignationsWithDepartmentAsync(int companyId, int regionId)
        {
            // Get designations
            var designations = await _unitOfWork.Repository<Designation>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive &&
                    !x.IsDeleted);

            // Get departments
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(x => x.IsActive && !x.IsDeleted);

            // Join manually
            var result = from d in designations
                         join dep in departments
                         on d.DepartmentId equals dep.DepartmentId into deptGroup
                         from dep in deptGroup.DefaultIfEmpty()
                         select new
                         {
                             designationId = d.DesignationId,
                             designationName = d.DesignationName,
                             departmentId = d.DepartmentId,
                             departmentName = dep != null ? dep.DepartmentName : ""
                         };

            return result;
        }
        public async Task<IEnumerable<object>> GetOfferCandidatesTopTableAsync(
    string department,
    string designation,
    int userId)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.StageId == 5 &&
                    c.UserId == userId &&
                    c.IsActive &&
                    (string.IsNullOrEmpty(department) || c.Department == department) &&
                    (string.IsNullOrEmpty(designation) || c.Designation == designation)
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                        ? c.FirstName
                        : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }


        public async Task<bool> SaveCandidateOfferAsync(CandidateOfferDto dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var offer = new CandidateOffer
                {
                    RegionId = dto.RegionId,
                    CompanyId = dto.CompanyId,
                    UserId = dto.UserId,
                    CandidateId = dto.CandidateId,
                    OfferedCtc = dto.OfferedCtc,
                    ExpectedDoj = DateOnly.FromDateTime(dto.ExpectedDoj),
                    OfferStatus = dto.OfferStatus,
                    Hrname = dto.HrName,
                    OfferLetterPath = dto.OfferLetterPath,
                    FilePath = dto.FilePath,
                    CreatedBy = dto.UserId,
                    CreatedAt = DateTime.Now,
                    HrEmail = dto.HrEmail,
                };

                await _unitOfWork.Repository<CandidateOffer>().AddAsync(offer);

                // 🔥 Move candidate to Onboarding stage (Stage = 6)
                var candidateRepo = _unitOfWork.Repository<Candidate>();
                var candidate = await candidateRepo.GetByIdAsync(dto.CandidateId);

                if (candidate == null)
                    throw new Exception("Candidate not found");

                candidate.StageId = 6; // ✅ Onboarding
                candidate.ModifiedAt = DateTime.Now;
                candidate.ModifiedBy = dto.UserId;

                candidateRepo.Update(candidate);

                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CandidateOfferDto>> GetOfferRecordsAsync(
    int userId,
    int companyId,
    int regionId)
        {
            var offers = await _unitOfWork.Repository<CandidateOffer>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.UserId == userId
                );

            if (!offers.Any())
                return Enumerable.Empty<CandidateOfferDto>();

            var candidateIds = offers.Select(x => x.CandidateId).Distinct().ToList();

            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(x => candidateIds.Contains(x.CandidateId));

            return offers
                .OrderByDescending(x => x.CreatedAt)
                .Select(o =>
                {
                    var candidate = candidates.First(c => c.CandidateId == o.CandidateId);

                    return new CandidateOfferDto
                    {
                        OfferId = o.OfferId,
                        CandidateId = o.CandidateId,
                        OfferedCtc = o.OfferedCtc,
                        ExpectedDoj = o.ExpectedDoj.ToDateTime(TimeOnly.MinValue),
                        OfferStatus = o.OfferStatus,
                        HrName = o.Hrname,
                        StageId = candidate.StageId,

                        SeqNo = candidate.SeqNo,
                        CandidateName = string.IsNullOrEmpty(candidate.LastName)
                            ? candidate.FirstName
                            : $"{candidate.FirstName} {candidate.LastName}",
                        Designation = candidate.Designation
                    };
                });
        }

        public async Task<IEnumerable<object>> GetHRUsersAsync(int companyId, int regionId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.RoleId == 4 &&              // 🔥 HR ROLE
                    x.Status == "Active"
                );

            return users.Select(u => new
            {
                u.UserId,
                u.FullName
            });
        }

        //        public async Task<bool> SendOfferLetterAsync(int offerId)
        //        {
        //            var offerRepo = _unitOfWork.Repository<CandidateOffer>();
        //            var candidateRepo = _unitOfWork.Repository<Candidate>();

        //            var offer = await offerRepo.GetByIdAsync(offerId);
        //            if (offer == null) throw new Exception("Offer not found");

        //            var candidate = await candidateRepo.GetByIdAsync(offer.CandidateId);
        //            if (candidate == null) throw new Exception("Candidate not found");

        //            // ===== FILE PATH =====
        //            string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "OfferLetters");
        //            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

        //            string fileName = $"Offer_{candidate.FirstName}.pdf";
        //            string fullPath = Path.Combine(root, fileName);

        //            // ===== PDF CREATION =====
        //            using (var writer = new PdfWriter(fullPath))
        //            using (var pdf = new PdfDocument(writer))
        //            using (var document = new iText.Layout.Document(pdf))
        //            {
        //                document.SetMargins(20, 20, 20, 20);

        //                // Date
        //                document.Add(new Paragraph($"Date: {DateTime.Now:dd-MMM-yyyy}")
        //                    .SetTextAlignment(TextAlignment.RIGHT));

        //                // Candidate Address
        //                document.Add(new Paragraph($@"
        //{candidate.FirstName} {candidate.LastName}

        //Asian Suncity,
        //#1101, 11th Floor,
        //B Block, Kondapur,
        //Hyderabad, Telangana 500084"));


        //                document.Add(
        //    new Paragraph("Sub: Employment Offer Letter")
        //    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
        //);

        //                document.Add(new Paragraph($"Dear {candidate.FirstName}{candidate.LastName},"));


        //                document.Add(
        //  new Paragraph("Congratulations!")
        //  .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)));


        //              document.Add(new Paragraph($@"
        //With reference to your application and subsequent interview with us for a career in our
        //organization, we are pleased to offer you the position of {candidate.Designation} with Cortracker IT Solutions Pvt Ltd.
        //"));

        //                document.Add(new Paragraph($"Your total compensation will be Rs.CTC: ₹ {offer.OfferedCtc} per annum which shall be inclusive\r\nof all benefits and taxes."));


        //                document.Add(new Paragraph($"Your base location will be Hyderabad, India, and you are requested to join us on Date of Joining: {offer.ExpectedDoj:dd-MMM-yyyy},on the following terms and conditions:"));



        //                document.Add(new Paragraph($"On the date of joining, you will be required to submit all documents requested for verification\r\nand appointment formalities. Submission of all documents is mandatory for background\r\nverification, validation, and completion of the joining process."));

        //                document.Add(new Paragraph($"You will be entitled to one paid leave per month (sick/casual) after successful completion of\r\nthe probationary period of three months. Any unused leave during the probation period may\r\nbe carried forward."));

        //                document.Add(new Paragraph($"Your employment is at-will, meaning either you or the Company may terminate the\r\nemployment with or without cause by giving 30 days’ notice.\r\n"));
        //                document.Add(new Paragraph($"This offer is subject to verification of your educational and previous employment records.\r\nAny misrepresentation or falsification of information will result in immediate termination"));


        //                document.Add(
        //new Paragraph("Please bring the following documents on the day of joining along with the originals for\r\nverification:")
        //.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
        //);
        //                document.Add(new Paragraph("1. Signed copy of this offer letter."));
        //                document.Add(new Paragraph("2. Recent passport-size photographs (4 copies)."));
        //                document.Add(new Paragraph("3. Copies of educational certificates (SSC / Intermediate / Graduation / PG)"));
        //                document.Add(new Paragraph("4. Copy of offer and relieving letters from previous employers."));
        //                document.Add(new Paragraph("5. Last 3 months’ salary slips and Form 16"));
        //                document.Add(new Paragraph("6. PAN card (mandatory).\r\n"));
        //                document.Add(new Paragraph("7. Proof of address – Passport/Aadhaar Card/Electricity Bill/Telephone Bill/Ration\r\nCard."));

        //                document.Add(new Paragraph(""));

        //                document.Add(new Paragraph(@"
        //We are delighted to welcome you to the team and look forward to a mutually rewarding
        //association. Please sign and return a copy of this letter as confirmation of your acceptance.
        //"));

        //                document.Add(new Paragraph(""));

        //                document.Add(new Paragraph("Best Regards,"));
        //                document.Add(
        // new Paragraph("HR Department")
        // .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
        //);


        //                document.Add(new Paragraph("--------------------------------------------------------------------------------------------------------"));

        //                // Acceptance

        //                document.Add(
        //new Paragraph("Acceptance of Offer")
        //.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
        //);
        //                document.Add(new Paragraph($@"
        //I, {candidate.FirstName}{candidate.LastName} , acknowledge that I have read, understood, and accept this offer letter
        //and agree to abide by the terms and conditions of employment as outlined herein.


        //Sign: _____________      Date: _____________

        //Place: Hyderabad
        //"));
        //            }

        //            // ===== SAVE PATH =====
        //            offer.OfferLetterPath = $"Uploads/OfferLetters/{fileName}";
        //            offerRepo.Update(offer);
        //            await _unitOfWork.CompleteAsync();

        //            string loginUrl = _configuration["AppSettings:LoginUrl"];
        //            string downloadUrl = $"{loginUrl}/{offer.OfferLetterPath}";

        //            // ===== EMAIL =====
        //            string subject = "Offer Letter – Cortracker HRMS";
        //            string body = $@"
        //<p>Dear {candidate.FirstName}{candidate.LastName},</p>
        //<p>Your offer letter is ready.</p>
        //<p><a href='{downloadUrl}'>Download Offer Letter</a></p>
        //<p>Regards,<br/>HR Team</p>";

        //            await _emailService.SendEmailAsync(
        //                candidate.Email,
        //                subject,
        //                body,
        //                string.IsNullOrEmpty(offer.HrEmail)
        //                    ? null
        //                    : new List<string> { offer.HrEmail }
        //            );

        //            return true;
        //        }


        // ================================================================
        // ================= SEND OFFER LETTER ============================
        // ================================================================

        public async Task<bool> SendOfferLetterAsync(int offerId)
        {
            // ============================================================
            // ====================== REPOSITORIES =========================
            // ============================================================

            var offerRepo = _unitOfWork.Repository<CandidateOffer>();
            var candidateRepo = _unitOfWork.Repository<Candidate>();

            // ============================================================
            // ======================== OFFER =============================
            // ============================================================

            var offer = await offerRepo.GetByIdAsync(offerId);

            if (offer == null)
                throw new Exception("Offer not found");

            // ============================================================
            // ====================== CANDIDATE ===========================
            // ============================================================

            var candidate = await candidateRepo.GetByIdAsync(offer.CandidateId);

            if (candidate == null)
                throw new Exception("Candidate not found");

            // ============================================================
            // ======================== COMPANY ===========================
            // ============================================================

            var company = _hRMSContext.Companies
                .Where(c => c.CompanyId == offer.CompanyId)
                .Select(c => new
                {
                    c.CompanyId,
                    c.CompanyName,
                    c.CompanyLogo
                })
                .FirstOrDefault();

            if (company == null)
                throw new Exception("Company not found");

            // ============================================================
            // ================= OFFER LETTER FOLDER ======================
            // ============================================================

            string offerLetterFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Uploads",
                "OfferLetters"
            );

            if (!Directory.Exists(offerLetterFolder))
            {
                Directory.CreateDirectory(offerLetterFolder);
            }

            // ============================================================
            // ======================= FILE NAME ==========================
            // ============================================================

            string safeName =
                $"{candidate.FirstName}_{candidate.LastName}"
                .Replace(" ", "_");

            string fileName =
                $"Offer_{safeName}_{offerId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            string fullPath = Path.Combine(
                offerLetterFolder,
                fileName
            );

            // ============================================================
            // ======================= DELETE OLD =========================
            // ============================================================

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // ============================================================
            // ===================== PDF GENERATION =======================
            // ============================================================

            using (var writer = new PdfWriter(fullPath))
            using (var pdf = new PdfDocument(writer))
            using (var document = new iText.Layout.Document(pdf))
            {
                // ========================================================
                // ===================== WATERMARK =========================
                // ========================================================

                pdf.AddEventHandler(
                    PdfDocumentEvent.END_PAGE,
                    new WatermarkHandler(company.CompanyLogo)
                );

                document.SetMargins(40, 40, 40, 40);

                // ========================================================
                // ======================= FONTS ===========================
                // ========================================================

                PdfFont normalFont =
                    PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                PdfFont boldFont =
                    PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                PdfFont italicFont =
                    PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

                // ========================================================
                // ===================== HEADER TABLE ======================
                // ========================================================

                var headerTable = new Table(
                    UnitValue.CreatePercentArray(new float[] { 1, 2 })
                ).UseAllAvailableWidth();

                // ========================================================
                // ======================== LOGO ===========================
                // ========================================================

                Cell logoCell = new Cell()
                    .SetBorder(Border.NO_BORDER);

                try
                {
                    if (!string.IsNullOrWhiteSpace(company.CompanyLogo))
                    {
                        string base64Data = company.CompanyLogo;

                        if (base64Data.Contains(","))
                        {
                            base64Data = base64Data.Substring(
                                base64Data.IndexOf(",") + 1
                            );
                        }

                        byte[] imageBytes =
                            Convert.FromBase64String(base64Data);

                        var imageData =
                            ImageDataFactory.Create(imageBytes);

                        var logo = new Image(imageData)
                            .ScaleToFit(120, 80)
                            .SetAutoScale(true);

                        logoCell.Add(logo);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Logo Error: " + ex.Message);
                }

                headerTable.AddCell(logoCell);

                // ========================================================
                // ================= COMPANY DETAILS =======================
                // ========================================================

                var companyCell = new Cell()
                    .SetBorder(Border.NO_BORDER)
                    .SetTextAlignment(TextAlignment.RIGHT);

                companyCell.Add(
                    new Paragraph(company.CompanyName)
                        .SetFont(boldFont)
                        .SetFontSize(24)
                        .SetFontColor(new DeviceRgb(25, 45, 80))
                );

                companyCell.Add(
                    new Paragraph("Hyderabad, Telangana, India")
                        .SetFont(normalFont)
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.DARK_GRAY)
                );

                companyCell.Add(
                    new Paragraph("www.companywebsite.com")
                        .SetFont(italicFont)
                        .SetFontSize(9)
                        .SetFontColor(ColorConstants.GRAY)
                );

                headerTable.AddCell(companyCell);

                document.Add(headerTable);

                document.Add(new Paragraph(" "));

                document.Add(
                    new LineSeparator(
                        new SolidLine(1f)
                    )
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================= TITLE ===========================
                // ========================================================

                document.Add(
                    new Paragraph("OFFER LETTER")
                        .SetFont(boldFont)
                        .SetFontSize(28)

                        .SetFontColor(new DeviceRgb(25, 45, 80))
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(5)
                );

                document.Add(
                    new Paragraph("CONFIDENTIAL EMPLOYMENT DOCUMENT")
                        .SetFont(normalFont)
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(20)
                );

                // ========================================================
                // ========================= DATE ==========================
                // ========================================================

                document.Add(
                    new Paragraph($"Date : {DateTime.Now:dd MMMM yyyy}")
                        .SetFont(normalFont)
                        .SetTextAlignment(TextAlignment.RIGHT)
                        .SetFontSize(11)
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ====================== CANDIDATE ========================
                // ========================================================

                document.Add(
                    new Paragraph($@"
To,

{candidate.FirstName} {candidate.LastName}

Hyderabad, Telangana
India")
                    .SetFont(normalFont)
                    .SetFontSize(11)
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================== SUBJECT ========================
                // ========================================================

                document.Add(
                    new Paragraph("Subject : Offer of Employment")
                        .SetFont(boldFont)
                        .SetFontSize(14)
                        .SetFontColor(new DeviceRgb(25, 45, 80))
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================== GREETING =======================
                // ========================================================

                document.Add(
                    new Paragraph(
                        $"Dear {candidate.FirstName} {candidate.LastName},"
                    )
                    .SetFont(normalFont)
                    .SetFontSize(11)
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ========================== BODY =========================
                // ========================================================

                document.Add(
                    new Paragraph(
                        $@"We are pleased to offer you employment with {company.CompanyName} for the position of {candidate.Designation}.

Your experience, professional expertise, and capabilities impressed us during the interview process, and we are confident that you will make a significant contribution to our organization.

The details of your employment offer are as follows:"
                    )
                    .SetFont(normalFont)
                    .SetFontSize(11)
                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                    .SetMinHeight(1.5f)
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ===================== OFFER DETAILS =====================
                // ========================================================

                Table detailsTable = new Table(
                    UnitValue.CreatePercentArray(new float[] { 1, 2 })
                )
                .UseAllAvailableWidth()
                .SetBorder(
                    new SolidBorder(
                        new DeviceRgb(220, 220, 220),
                        1
                    )
                )
                .SetMarginTop(10)
                .SetMarginBottom(20);

                detailsTable.AddCell(GetLabelCell("Designation", boldFont));
                detailsTable.AddCell(GetValueCell(candidate.Designation, normalFont));

                detailsTable.AddCell(GetLabelCell("Department", boldFont));
                detailsTable.AddCell(GetValueCell("Information Technology", normalFont));

                detailsTable.AddCell(GetLabelCell("Joining Date", boldFont));
                detailsTable.AddCell(
                    GetValueCell(
                        offer.ExpectedDoj.ToString("dd MMM yyyy"),
                        normalFont
                    )
                );

                detailsTable.AddCell(GetLabelCell("Annual CTC", boldFont));
                detailsTable.AddCell(
                    GetValueCell(
                        $"₹ {offer.OfferedCtc:N0} Per Annum",
                        normalFont
                    )
                );

                detailsTable.AddCell(GetLabelCell("Work Location", boldFont));
                detailsTable.AddCell(GetValueCell("Hyderabad", normalFont));

                document.Add(detailsTable);

                // ========================================================
                // ================= TERMS & CONDITIONS ====================
                // ========================================================

                document.Add(
                    new Paragraph("Terms & Conditions")
                        .SetFont(boldFont)
                        .SetFontSize(15)
                        .SetFontColor(new DeviceRgb(25, 45, 80))
                );

                document.Add(new Paragraph(" "));

                string[] terms =
                {
            "• You are required to submit all educational and employment documents during onboarding.",
            "• Your employment will be governed by the company’s policies and code of conduct.",
            "• The first six months of employment shall be considered as probation period.",
            "• Either party may terminate employment by providing 30 days written notice.",
            "• This offer is subject to successful background verification."
        };

                foreach (var term in terms)
                {
                    document.Add(
                        new Paragraph(term)
                            .SetFont(normalFont)
                            .SetFontSize(11)
                            .SetMarginLeft(10)
                            .SetMinHeight(1.4f)
                    );
                }

                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================== CLOSING ========================
                // ========================================================

                document.Add(
                    new Paragraph(
                        @"We welcome you to our organization and look forward to a successful and rewarding association with you.

Please sign and return a copy of this letter as confirmation of your acceptance."
                    )
                    .SetFont(normalFont)
                    .SetFontSize(11)
                    .SetTextAlignment(TextAlignment.JUSTIFIED)
                    .SetMinHeight(1.5f)
                );

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================= SIGNATURE =======================
                // ========================================================

                document.Add(
                    new Paragraph($"For {company.CompanyName}")
                        .SetFont(boldFont)
                        .SetFontSize(12)
                );

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));

                document.Add(
                    new Paragraph("Authorized Signatory")
                        .SetFont(normalFont)
                        .SetFontSize(11)
                );

                document.Add(new Paragraph(" "));

                document.Add(
                    new LineSeparator(
                        new SolidLine(1f)
                    )
                );

                document.Add(new Paragraph(" "));

                // ========================================================
                // ======================= ACCEPTANCE ======================
                // ========================================================

                document.Add(
                    new Paragraph("Employee Acceptance")
                        .SetFont(boldFont)
                        .SetFontSize(15)
                        .SetFontColor(new DeviceRgb(25, 45, 80))
                );

                document.Add(new Paragraph(" "));

                document.Add(
                    new Paragraph($@"
I, {candidate.FirstName} {candidate.LastName}, hereby accept the employment offer and agree to the terms and conditions mentioned in this letter.

Employee Signature : ________________________

Date : _____________________________________
")
                    .SetFont(normalFont)
                    .SetFontSize(11)
                    .SetMinHeight(1.5f)
                );

                // ========================================================
                // ========================= FOOTER ========================
                // ========================================================

                document.Add(new Paragraph(" "));

                document.Add(
                    new LineSeparator(
                        new SolidLine(1f)
                    )
                    .SetMarginTop(15)
                );

                document.Add(
                    new Paragraph(
                        $"{company.CompanyName} | Human Resources Department"
                    )
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(9)
                    .SetFontColor(ColorConstants.GRAY)
                );

                document.Add(
                    new Paragraph(
                        "This document is system generated and confidential."
                    )
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(8)
                    .SetFontColor(ColorConstants.LIGHT_GRAY)
                );
            }

            // ============================================================
            // ================= SAVE FILE PATH ===========================
            // ============================================================

            offer.OfferLetterPath =
                $"Uploads/OfferLetters/{fileName}";

            offerRepo.Update(offer);

            await _unitOfWork.CompleteAsync();
            var docRepo = _unitOfWork.Repository<CandidateDocumentChecklist>();
            var existingDoc = await docRepo.FindAsync(x =>
                x.OfferId == offer.OfferId &&
                x.CandidateId == candidate.CandidateId
            );

            if (!existingDoc.Any())
            {
                var doc = new CandidateDocumentChecklist
                {
                    OfferId = offer.OfferId,
                    CandidateId = candidate.CandidateId,
                    CompanyId = offer.CompanyId,
                    RegionId = offer.RegionId,

                    Status = "LinkSent",
                    CreatedDate = DateTime.Now
                };

                await docRepo.AddAsync(doc);
            }
            else
            {
                var doc = existingDoc.First();
                doc.Status = "LinkResent";
                doc.UpdatedDate = DateTime.Now;

                docRepo.Update(doc);
            }

            await _unitOfWork.CompleteAsync();
            var baseUrl = _configuration["AppSettings:FrontendUrl"];

            string uploadLink =
                $"{baseUrl}/#/offer-documents" +
                $"/{offer.OfferId}" +
                $"/{candidate.CandidateId}" +
                $"/{offer.CompanyId}" +
                $"/{offer.RegionId}";

            // ============================================================
            // ======================== EMAIL =============================
            // ============================================================

            string subject = "Offer Letter – HRMS";

            string body = $@"
<html>
<body style='font-family: Arial, sans-serif; color:#333; line-height:1.8;'>

<p>Dear {candidate.FirstName} {candidate.LastName},</p>

<p>Congratulations!</p>

<p>
We are pleased to offer you employment with 
<strong>{company.CompanyName}</strong>.
</p>

<p>
Please find attached your official Offer Letter.
👉 Click below to upload your joining documents:
</p>

<p>
<a href='{uploadLink}' target='_blank'>
Upload Documents
</a>
</p>

<br/>

<p>
Regards,<br/>
<strong>HR Department</strong><br/>
{company.CompanyName}
</p>

</body>
</html>";

            // ============================================================
            // ======================= SEND EMAIL =========================
            // ============================================================

            await _emailService.SendEmailAsync(
                candidate.Email,
                subject,
                body,
                string.IsNullOrWhiteSpace(offer.HrEmail)
                    ? null
                    : new List<string> { offer.HrEmail },
                new List<string> { fullPath }
            );

            return true;
        }

        // ================================================================
        // ===================== HELPER METHODS ============================
        // ================================================================

        private Cell GetLabelCell(string text, PdfFont font)
        {
            return new Cell()
                .Add(
                    new Paragraph(text)
                        .SetFont(font)
                        .SetFontSize(10)
                        .SetFontColor(ColorConstants.WHITE)
                )
                .SetBackgroundColor(
                    new DeviceRgb(25, 45, 80)
                )
                .SetPadding(10)
                .SetBorder(Border.NO_BORDER);
        }

        private Cell GetValueCell(string text, PdfFont font)
        {
            return new Cell()
                .Add(
                    new Paragraph(text)
                        .SetFont(font)
                        .SetFontSize(10)
                )
                .SetPadding(10)
                .SetBorderBottom(
                    new SolidBorder(
                        new DeviceRgb(230, 230, 230),
                        1
                    )
                )
                .SetBorderTop(Border.NO_BORDER)
                .SetBorderLeft(Border.NO_BORDER)
                .SetBorderRight(Border.NO_BORDER);
        }

        // ================================================================
        // ==================== WATERMARK HANDLER =========================
        // ================================================================

        public class WatermarkHandler : AbstractPdfDocumentEventHandler
        {
            private readonly string _base64Logo;

            public WatermarkHandler(string base64Logo)
            {
                _base64Logo = base64Logo;
            }

            protected override void OnAcceptedEvent(
                AbstractPdfDocumentEvent currentEvent
            )
            {
                try
                {
                    PdfDocumentEvent docEvent =
                        (PdfDocumentEvent)currentEvent;

                    PdfDocument pdf =
                        docEvent.GetDocument();

                    PdfPage page =
                        docEvent.GetPage();

                    Rectangle pageSize =
                        page.GetPageSize();

                    string cleanBase64 = _base64Logo;

                    if (cleanBase64.Contains(","))
                    {
                        cleanBase64 =
                            cleanBase64.Substring(
                                cleanBase64.IndexOf(",") + 1
                            );
                    }

                    byte[] imageBytes =
                        Convert.FromBase64String(cleanBase64);

                    ImageData imageData =
                        ImageDataFactory.Create(imageBytes);

                    Image watermark =
                        new Image(imageData);

                    watermark
                        .ScaleToFit(300, 300)
                        .SetOpacity(0.08f);

                    float x =
                        (pageSize.GetWidth() - 300) / 2;

                    float y =
                        (pageSize.GetHeight() - 300) / 2;

                    PdfCanvas pdfCanvas =
                        new PdfCanvas(
                            page.NewContentStreamBefore(),
                            page.GetResources(),
                            pdf
                        );

                    iText.Layout.Canvas canvas =
                        new iText.Layout.Canvas(
                            pdfCanvas,
                            pageSize
                        );

                    watermark.SetFixedPosition(x, y);

                    canvas.Add(watermark);

                    canvas.Close();
                }
                catch
                {
                    // Ignore watermark errors
                }
            }
        }

        //        public async Task<bool> SendOfferLetterAsync(int offerId)
        //        {
        //            // =====================================================
        //            // ===== REPOSITORIES =====
        //            // =====================================================

        //            var offerRepo = _unitOfWork.Repository<CandidateOffer>();
        //            var candidateRepo = _unitOfWork.Repository<Candidate>();

        //            // =====================================================
        //            // ===== GET OFFER =====
        //            // =====================================================

        //            var offer = await offerRepo.GetByIdAsync(offerId);

        //            if (offer == null)
        //                throw new Exception("Offer not found");

        //            // =====================================================
        //            // ===== GET CANDIDATE =====
        //            // =====================================================

        //            var candidate = await candidateRepo.GetByIdAsync(offer.CandidateId);

        //            if (candidate == null)
        //                throw new Exception("Candidate not found");

        //            // =====================================================
        //            // ===== GET COMPANY =====
        //            // =====================================================

        //            var company = _hRMSContext.Companies
        //                .Where(c => c.CompanyId == offer.CompanyId)
        //                .Select(c => new
        //                {
        //                    c.CompanyId,
        //                    c.CompanyName,
        //                    c.CompanyLogo
        //                })
        //                .FirstOrDefault();

        //            if (company == null)
        //                throw new Exception("Company not found");

        //            // =====================================================
        //            // ===== OFFER LETTER DIRECTORY =====
        //            // =====================================================

        //            string offerLetterFolder = Path.Combine(
        //                Directory.GetCurrentDirectory(),
        //                "wwwroot",
        //                "Uploads",
        //                "OfferLetters"
        //            );

        //            if (!Directory.Exists(offerLetterFolder))
        //            {
        //                Directory.CreateDirectory(offerLetterFolder);
        //            }

        //            // =====================================================
        //            // ===== SAFE FILE NAME =====
        //            // =====================================================

        //            string safeName =
        //                $"{candidate.FirstName}_{candidate.LastName}"
        //                .Replace(" ", "_");

        //            string fileName =
        //                $"Offer_{safeName}_{offerId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

        //            string fullPath = Path.Combine(
        //                offerLetterFolder,
        //                fileName
        //            );

        //            // =====================================================
        //            // ===== DELETE OLD FILE IF EXISTS =====
        //            // =====================================================

        //            if (File.Exists(fullPath))
        //            {
        //                File.Delete(fullPath);
        //            }

        //            // =====================================================
        //            // ===== PDF GENERATION =====
        //            // =====================================================

        //            using (var writer = new PdfWriter(fullPath))
        //            using (var pdf = new PdfDocument(writer))
        //            using (var document = new iText.Layout.Document(pdf))
        //            {
        //                document.SetMargins(30, 30, 30, 30);

        //                // =================================================
        //                // ===== FONTS =====
        //                // =================================================

        //                PdfFont normalFont =
        //                    PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

        //                PdfFont boldFont =
        //                    PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        //                // =================================================
        //                // ===== HEADER TABLE =====
        //                // =================================================

        //                var headerTable = new Table(
        //                    UnitValue.CreatePercentArray(new float[] { 1, 2 })
        //                ).UseAllAvailableWidth();

        //                // =================================================
        //                // ===== LOGO =====
        //                // =================================================

        //                Cell logoCell = new Cell()
        //                    .SetBorder(Border.NO_BORDER);

        //                try
        //                {
        //                    if (!string.IsNullOrWhiteSpace(company.CompanyLogo))
        //                    {
        //                        // Example:
        //                        // data:image/png;base64,iVBORw0KGgoAAAANS...
        //                        // OR
        //                        // /9j/4AAQSkZJRgABAQAAAQABAAD...

        //                        string base64Data = company.CompanyLogo;

        //                        // ===== REMOVE PREFIX =====
        //                        if (base64Data.Contains(","))
        //                        {
        //                            base64Data = base64Data.Substring(
        //                                base64Data.IndexOf(",") + 1
        //                            );
        //                        }

        //                        // ===== CONVERT BASE64 TO BYTE[] =====
        //                        byte[] imageBytes =
        //                            Convert.FromBase64String(base64Data);

        //                        // ===== CREATE IMAGE =====
        //                        var imageData =
        //                            ImageDataFactory.Create(imageBytes);

        //                        var logo = new Image(imageData)
        //                            .ScaleToFit(120, 80)
        //                            .SetAutoScale(true);

        //                        logoCell.Add(logo);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine("Logo Error: " + ex.Message);
        //                }

        //                headerTable.AddCell(logoCell);

        //                // =================================================
        //                // ===== COMPANY DETAILS =====
        //                // =================================================

        //                var companyCell = new Cell()
        //                    .SetBorder(Border.NO_BORDER)
        //                    .SetTextAlignment(TextAlignment.RIGHT);

        //                companyCell.Add(
        //                    new Paragraph(company.CompanyName)
        //                        .SetFont(boldFont)
        //                        .SetFontSize(18)
        //                );

        //                companyCell.Add(
        //                    new Paragraph("Hyderabad, Telangana, India")
        //                        .SetFont(normalFont)
        //                        .SetFontSize(10)
        //                        .SetFontColor(ColorConstants.DARK_GRAY)
        //                );

        //                headerTable.AddCell(companyCell);

        //                document.Add(headerTable);

        //                // =================================================
        //                // ===== LINE =====
        //                // =================================================

        //                document.Add(new Paragraph(" "));
        //                document.Add(new LineSeparator(new SolidLine()));

        //                // =================================================
        //                // ===== DATE =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph($"Date: {DateTime.Now:dd-MMM-yyyy}")
        //                        .SetFont(normalFont)
        //                        .SetTextAlignment(TextAlignment.RIGHT)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== CANDIDATE ADDRESS =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph($@"
        //{candidate.FirstName} {candidate.LastName}

        //Hyderabad, Telangana
        //India
        //")
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== SUBJECT =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph("Subject: Employment Offer Letter")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(14)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== GREETING =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph(
        //                        $"Dear {candidate.FirstName} {candidate.LastName},"
        //                    )
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== CONGRATULATIONS =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph("Congratulations!")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(12)
        //                );

        //                document.Add(
        //                    new Paragraph(
        //                        $@"With reference to your application and subsequent interview process with us,
        //we are pleased to offer you the position of {candidate.Designation} at
        //{company.CompanyName}."
        //                    )
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== CTC =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph(
        //                        $"Your total compensation will be ₹ {offer.OfferedCtc:N0} per annum inclusive of all applicable benefits and taxes."
        //                    )
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== DOJ =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph(
        //                        $"Your base location will be Hyderabad, India, and you are requested to join on {offer.ExpectedDoj:dd-MMM-yyyy}."
        //                    )
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== TERMS & CONDITIONS =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph("Terms & Conditions")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(13)
        //                );

        //                string[] terms =
        //                {
        //            "1. You are required to submit all necessary documents during joining.",
        //            "2. Your employment will be governed by company policies and procedures.",
        //            "3. Leave and other benefits will be applicable after successful completion of probation.",
        //            "4. Either party may terminate employment by providing 30 days notice.",
        //            "5. This offer is subject to successful background verification."
        //        };

        //                foreach (var term in terms)
        //                {
        //                    document.Add(
        //                        new Paragraph(term)
        //                            .SetFont(normalFont)
        //                            .SetFontSize(11)
        //                    );
        //                }

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== DOCUMENTS REQUIRED =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph("Documents Required During Joining")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(13)
        //                );

        //                string[] documents =
        //                {
        //            "1. Signed copy of this Offer Letter",
        //            "2. Passport-size photographs",
        //            "3. Educational certificates",
        //            "4. Previous employment documents",
        //            "5. Last 3 months salary slips",
        //            "6. PAN Card and Aadhaar Card"
        //        };

        //                foreach (var doc in documents)
        //                {
        //                    document.Add(
        //                        new Paragraph(doc)
        //                            .SetFont(normalFont)
        //                            .SetFontSize(11)
        //                    );
        //                }

        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== CLOSING =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph(
        //                        @"We are delighted to welcome you to the organization and look forward to a successful professional association."
        //                    )
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));

        //                document.Add(
        //                    new Paragraph("Best Regards,")
        //                        .SetFont(normalFont)
        //                        .SetFontSize(11)
        //                );

        //                document.Add(
        //                    new Paragraph("HR Department")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(11)
        //                );

        //                document.Add(new Paragraph(" "));
        //                document.Add(new LineSeparator(new SolidLine()));
        //                document.Add(new Paragraph(" "));

        //                // =================================================
        //                // ===== ACCEPTANCE =====
        //                // =================================================

        //                document.Add(
        //                    new Paragraph("Acceptance of Offer")
        //                        .SetFont(boldFont)
        //                        .SetFontSize(13)
        //                );

        //                document.Add(
        //                    new Paragraph($@"
        //I, {candidate.FirstName} {candidate.LastName}, acknowledge that I have read and understood the terms and conditions mentioned in this offer letter and hereby accept the offer.

        //Signature: ______________________

        //Date: __________________________

        //Place: Hyderabad
        //")
        //                    .SetFont(normalFont)
        //                    .SetFontSize(11)
        //                );
        //            }

        //            // =====================================================
        //            // ===== SAVE FILE PATH =====
        //            // =====================================================

        //            offer.OfferLetterPath =
        //                $"Uploads/OfferLetters/{fileName}";

        //            offerRepo.Update(offer);

        //            await _unitOfWork.CompleteAsync();

        //            // =====================================================
        //            // ===== EMAIL =====
        //            // =====================================================

        //            string subject = "Offer Letter – HRMS";

        //            string body = $@"
        //<html>
        //<body style='font-family: Arial, sans-serif; color:#333; line-height:1.6;'>

        //    <p>Dear {candidate.FirstName} {candidate.LastName},</p>

        //    <p>
        //        Congratulations!
        //    </p>

        //    <p>
        //        We are delighted to extend an offer of employment with 
        //        <strong>{company.CompanyName}</strong>.
        //    </p>

        //    <p>
        //        Please find your official Offer Letter attached with this email.
        //        Kindly review the document carefully and confirm your acceptance.
        //    </p>

        //    <p>
        //        We are excited about the opportunity to have you join our team and
        //        look forward to a successful professional journey together.
        //    </p>

        //    <br/>

        //    <p>
        //        Regards,<br/>
        //        <strong>HR Department</strong><br/>
        //        {company.CompanyName}
        //    </p>

        //</body>
        //</html>";

        //            // =====================================================
        //            // ===== SEND EMAIL =====
        //            // =====================================================

        //            await _emailService.SendEmailAsync(
        //                candidate.Email,
        //                subject,
        //                body,
        //                string.IsNullOrWhiteSpace(offer.HrEmail)
        //                    ? null
        //                    : new List<string> { offer.HrEmail },
        //                new List<string> { fullPath }
        //            );

        //            return true;
        //        }

        public async Task<(byte[] fileBytes, string fileName)> DownloadOfferLetterAsync(int offerId)
        {
            var offer = await _unitOfWork.Repository<CandidateOffer>().GetByIdAsync(offerId);
            if (offer == null || string.IsNullOrEmpty(offer.OfferLetterPath))
                throw new Exception("Offer letter not found");

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", offer.OfferLetterPath);
            var bytes = await File.ReadAllBytesAsync(fullPath);

            return (bytes, Path.GetFileName(fullPath));
        }




        //OnBoarding 


        public async Task<IEnumerable<object>> GetOnboardingCandidatesTopTableAsync(int companyId, int regionId, string department, string designation)
        {
            var candidates = await _unitOfWork.Repository<Candidate>()
                .FindAsync(c =>
                    c.CompanyId == companyId &&
                    c.RegionId == regionId &&
                    c.StageId == 6 &&
                    c.IsActive &&
                    c.Department == department &&
                    c.Designation == designation
                );

            return candidates.Select(c => new
            {
                c.CandidateId,
                c.SeqNo,
                Name = string.IsNullOrEmpty(c.LastName)
                    ? c.FirstName
                    : $"{c.FirstName} {c.LastName}",
                c.Mobile,
                Expected = c.ExpectedSalary
            });
        }

        public async Task<int> SaveCandidateOnboardingAsync(CandidateOnboardingDTO dto)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var repo = _unitOfWork.Repository<CandidateOnboarding>();

                var existing = (await repo.FindAsync(x =>
                    x.CandidateId == dto.CandidateId &&
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId))
                    .FirstOrDefault();

                if (existing == null)
                {
                    var entity = new CandidateOnboarding
                    {
                        RegionId = dto.RegionId,
                        CompanyId = dto.CompanyId,
                        UserId = dto.UserId,
                        CandidateId = dto.CandidateId,
                        JoiningDate = dto.JoiningDate.HasValue ? DateOnly.FromDateTime(dto.JoiningDate.Value) : null,


                        DocumentsCollected = dto.DocumentsCollected,
                        BackgroundCheckStatus = dto.BackgroundCheckStatus,
                        LaptopIssued = dto.LaptopIssued,
                        BuddyAssigned = dto.BuddyAssigned,
                        OnboardingStatus = (dto.DocumentsCollected && dto.BackgroundCheckStatus == "Clear")
                            ? "Completed"
                            : "InProgress",
                        CreatedAt = DateTime.Now,
                        CreatedBy = dto.UserId
                    };

                    await repo.AddAsync(entity);
                    await _unitOfWork.CompleteAsync();

                    // ✅ Move candidate to Stage 7 (Onboarding)
                    var candidate = await _unitOfWork.Repository<Candidate>()
                        .GetByIdAsync(dto.CandidateId);

                    if (candidate != null)
                    {
                        candidate.StageId = 7;
                        candidate.ModifiedAt = DateTime.Now;
                        candidate.ModifiedBy = dto.UserId;
                        await _unitOfWork.CompleteAsync();
                    }

                    await tx.CommitAsync();
                    return entity.OnboardingId;
                }
                else
                {
                    existing.JoiningDate = dto.JoiningDate.HasValue
                        ? DateOnly.FromDateTime(dto.JoiningDate.Value)
                        : null;
                    existing.DocumentsCollected = dto.DocumentsCollected;
                    existing.BackgroundCheckStatus = dto.BackgroundCheckStatus;
                    existing.LaptopIssued = dto.LaptopIssued;
                    existing.BuddyAssigned = dto.BuddyAssigned;
                    existing.OnboardingStatus = (dto.DocumentsCollected && dto.BackgroundCheckStatus == "Clear")
                        ? "Completed"
                        : "InProgress";
                    existing.ModifiedAt = DateTime.Now;
                    existing.ModifiedBy = dto.UserId;

                    await _unitOfWork.CompleteAsync();

                    // ✅ Ensure stage is still 7 on update
                    var candidate = await _unitOfWork.Repository<Candidate>()
                        .GetByIdAsync(dto.CandidateId);

                    if (candidate != null && candidate.StageId != 7)
                    {
                        candidate.StageId = 7;
                        candidate.ModifiedAt = DateTime.Now;
                        candidate.ModifiedBy = dto.UserId;
                        await _unitOfWork.CompleteAsync();
                    }

                    await tx.CommitAsync();
                    return existing.OnboardingId;
                }
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }


        public async Task<IEnumerable<object>> GetOnboardedCandidatesAsync(int companyId, int regionId)
        {
            var result = await _unitOfWork.Repository<CandidateOnboarding>()
            .FindAsync(x =>
                x.CompanyId == companyId &&
                x.RegionId == regionId &&
                (x.OnboardingStatus == null || x.OnboardingStatus != "Completed"));

            var candidateRepo = _unitOfWork.Repository<Candidate>();

            var candidates = await candidateRepo.FindAsync(c =>
                c.CompanyId == companyId &&
                c.RegionId == regionId &&
                c.IsActive);

            return result.Select(o =>
            {
                var cand = candidates.FirstOrDefault(c => c.CandidateId == o.CandidateId);
                return new
                {
                    o.CandidateId,
                    Name = cand != null
                        ? string.IsNullOrEmpty(cand.LastName)
                            ? cand.FirstName
                            : $"{cand.FirstName} {cand.LastName}"
                        : "",
                    o.JoiningDate,
                    DocsCollected = o.DocumentsCollected,
                    BgCheck = o.BackgroundCheckStatus,
                    Laptop = o.LaptopIssued,
                    Buddy = o.BuddyAssigned,
                    Stage = cand?.StageId ?? 0,  // ✅ REAL stage (7)
                    Status = o.OnboardingStatus
                };
            });
        }
        public async Task<int> SubmitJobApplicationAsync(JobApplicationDto dto, IFormFile? resume)
        {
            using var tx = await _unitOfWork.BeginTransactionAsync();

            try
            {
                string resumePath = null;

                // =========================
                // 1. SAVE FILE
                // =========================
                if (resume != null && resume.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                    var ext = Path.GetExtension(resume.FileName).ToLower();

                    if (!allowedExtensions.Contains(ext))
                        throw new Exception("Invalid file type");

                    string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "Resumes");

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var fileName = $"{Guid.NewGuid()}_{resume.FileName}";
                    string fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await resume.CopyToAsync(stream);
                    }

                    resumePath = $"Uploads/Resumes/{fileName}";
                }

                // =========================
                // 2. SAVE JOB APPLICATION
                // =========================
                var entity = new JobApplication
                {
                    CandidateName = dto.CandidateName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    JobTitle = dto.JobTitle,
                    ExperienceYears = dto.ExperienceYears,
                    Technology = dto.Technology,
                    ResumeUrl = resumePath,
                    Status = "Applied",
                    AppliedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Repository<JobApplication>().AddAsync(entity);
                await _unitOfWork.CompleteAsync(); // gets ApplicationId

                // =========================
                // 3. PARSE RESUME
                // =========================
                string text = "";

                if (!string.IsNullOrEmpty(resumePath))
                {
                    var fileFullPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        resumePath.TrimStart('/'));

                    text = ExtractTextFromResume(fileFullPath);
                }

                var experienceBlock = GetSection(text, "Experience");
                var educationBlock = GetSection(text, "Education");
                var skills = GetSection(text, "Skills");

                // =========================
                // 4. CREATE CANDIDATE
                // =========================
                var candidate = new Candidate
                {
                    FirstName = dto.CandidateName,
                    Email = dto.Email,
                    Mobile = dto.Phone,
                    Designation = dto.JobTitle,
                    Skills = skills,
                    FilePath = resumePath,
                    // Sequence
                    SeqNo = $"AppRes_{DateTime.Now:yyyy}_{entity.ApplicationId}",
                    // Auto Applied Date
                    AppliedDate = DateOnly.FromDateTime(DateTime.Now),
                    // Auto Stage = Resume Received
                    StageId = 1,
                    // Audit
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Repository<Candidate>().AddAsync(candidate);
                await _unitOfWork.CompleteAsync();

                // =========================
                // EXPERIENCE TABLE
                // =========================

                var experienceLines = experienceBlock
                     .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => x.Trim())
                     .ToList();

                string company = "";
                string role = "";
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;

                foreach (var line in experienceLines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (line.Contains("Experience", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Safer pattern
                    var match = Regex.Match(line,
                        @"^(?<role>.+?)\s*[-–]\s*(?<company>.+?)\s*\((?<range>.+)\)$",
                        RegexOptions.IgnoreCase);

                    if (!match.Success)
                        continue;

                    role = match.Groups["role"].Value.Trim();
                    company = match.Groups["company"].Value.Trim();

                    var range = match.Groups["range"].Value;

                    var dates = range.Split(new[] { '-', '–' }, StringSplitOptions.RemoveEmptyEntries);

                    if (dates.Length > 0)
                        fromDate = ParseExperienceDate(dates[0].Trim()) ?? DateTime.Now;

                    if (dates.Length > 1)
                        toDate = ParseExperienceDate(dates[1].Trim()) ?? DateTime.Now;

                    break;
                }

                CandidateExperience exp = new CandidateExperience
                {
                    CandidateId = candidate.CandidateId,
                    Designation = role,
                    Organization = company,
                    FromDate = DateOnly.FromDateTime(fromDate),
                    ToDate = DateOnly.FromDateTime(toDate),
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<CandidateExperience>().AddAsync(exp);

                // =========================
                // 6. QUALIFICATION TABLE (FIXED)
                // =========================


                // DEGREE / QUALIFICATION
                var qualMatch = Regex.Match(
                    educationBlock,
                    @"(B\.?Tech|Bachelor of Technology|Engineering|M\.?Tech|MBA|MCA|B\.?Sc|B\.?Com|Diploma|PhD)",
                    RegexOptions.IgnoreCase
                );

                var qualification = qualMatch.Success ? qualMatch.Value : "Not Found";

                // UNIVERSITY / COLLEGE
                var universityMatch = Regex.Match(
                    educationBlock,
                    @"([A-Z][A-Za-z&.\s]+(University|College|Institute|School|Academy))",
                    RegexOptions.IgnoreCase
                );

                var university = universityMatch.Success ? universityMatch.Value : "Not Found";

                // YEARS
                var yearMatches = Regex.Matches(educationBlock, @"(19\d{2}|20\d{2})")
                    .Select(m => int.Parse(m.Value))
                    .ToList();

                int? fromYear = null;
                int? toYear = null;

                if (yearMatches.Count == 1)
                {
                    toYear = yearMatches[0];   // ONLY ONE YEAR FOUND
                }
                else if (yearMatches.Count >= 2)
                {
                    fromYear = yearMatches[0];
                    toYear = yearMatches[1];
                }

                var qual = new CandidateQualification
                {
                    CandidateId = candidate.CandidateId,
                    CreatedAt = DateTime.Now,
                    Qualification = qualification,
                    BoardUniversity = university,
                    FromYear = fromYear ?? 0,
                    ToYear = toYear ?? 0
                };

                await _unitOfWork.Repository<CandidateQualification>().AddAsync(qual);

                // =========================
                // 7. FINAL SAVE
                // =========================
                await _unitOfWork.CompleteAsync();
                await tx.CommitAsync();

                return entity.ApplicationId;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                var inner = ex.InnerException?.Message;
                var full = ex.ToString();

                throw new Exception($"DB ERROR: {inner ?? full}");
            }
        }
        private string ExtractTextFromResume(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            var extension = Path.GetExtension(path).ToLower();

            switch (extension)
            {
                case ".pdf":
                    return ExtractTextFromPdf(path);

                case ".docx":
                    return ExtractTextFromDocx(path);

                case ".doc":
                    return ExtractTextFromDoc(path);

                default:
                    return "";
            }
        }
        private string ExtractTextFromPdf(string path)
        {
            var text = new StringBuilder();

            using var pdfReader = new iText.Kernel.Pdf.PdfReader(path);
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);

                text.Append(
                    iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page)
                );
            }

            return text.ToString();
        }
        private string ExtractTextFromDocx(string path)
        {
            StringBuilder text = new StringBuilder();

            using (WordprocessingDocument wordDoc =
                   WordprocessingDocument.Open(path, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;

                if (body != null)
                {
                    text.Append(body.InnerText);
                }
            }

            return text.ToString();
        }
        private string ExtractTextFromDoc(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            HWPFDocument doc = new HWPFDocument(fs);

            WordExtractor extractor = new WordExtractor(doc);

            return extractor.Text;
        }
        private string GetSection(string text, string section)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = Regex.Replace(text, @"\r", "\n");

            var startKeywords = new Dictionary<string, string[]>
    {
        { "Experience", new[] { "Professional Experience", "Work Experience", "Experience" } },
        { "Education", new[] { "Education", "Academic Details", "Qualifications" } },
        { "Skills", new[] { "Skills", "Technical Skills", "Key Skills" } }
    };

            var stopKeywords = new[]
            {
        "Experience",
        "Education",
        "Projects",
        "Certifications",
        "Achievements",
        "Declaration",
        "Contact"
    };

            if (!startKeywords.ContainsKey(section))
                return "";

            foreach (var keyword in startKeywords[section])
            {
                int startIndex = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);

                if (startIndex >= 0)
                {
                    int endIndex = text.Length;

                    foreach (var stop in stopKeywords)
                    {
                        int temp = text.IndexOf(stop, startIndex + keyword.Length, StringComparison.OrdinalIgnoreCase);

                        if (temp > startIndex && temp < endIndex)
                            endIndex = temp;
                    }

                    return text.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }

            return "";
        }
        private DateTime? ParseResumeDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim().ToLower();

            if (input.Contains("present") || input.Contains("current"))
                return DateTime.Now;

            if (DateTime.TryParse(input, out var parsedDate))
                return parsedDate;

            return null;
        }
        private DateTime? ParseExperienceDate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.ToLower();

            if (input.Contains("present") || input.Contains("current") || input.Contains("till now"))
                return DateTime.Now;

            // Try extracting year
            var yearMatch = Regex.Match(input, @"(19\d{2}|20\d{2})");

            if (yearMatch.Success)
                return new DateTime(int.Parse(yearMatch.Value), 1, 1);

            return null;
        }

        public async Task<bool> UpdateCompanyRegionAsync(string email, string mobile, int companyId, int regionId, int userId)
        {
            // 1. FIND CANDIDATE (BETTER: DB FILTER NOT GetAll)
            var candidate = (await _unitOfWork.Repository<Candidate>()
                .GetAllAsync())
                .Where(x =>
                    x.Email == email &&
                    x.Mobile == mobile &&
                    x.SeqNo.StartsWith("AppRes_"))
                .OrderByDescending(x => x.CreatedAt) // IMPORTANT: take latest
                .FirstOrDefault();

            if (candidate == null)
                return false;

            var candidateId = candidate.CandidateId;

            // 2. UPDATE CANDIDATE
            candidate.CompanyId = companyId;
            candidate.RegionId = regionId;
            candidate.UserId = userId;

            _unitOfWork.Repository<Candidate>().Update(candidate);

            // 3. UPDATE EXPERIENCE
            var experiences = (await _unitOfWork.Repository<CandidateExperience>()
                .GetAllAsync())
                .Where(x => x.CandidateId == candidateId)
                .ToList();

            foreach (var exp in experiences)
            {
                exp.CompanyId = companyId;
                exp.RegionId = regionId;
                exp.UserId = userId;

                _unitOfWork.Repository<CandidateExperience>().Update(exp);
            }

            // 4. UPDATE QUALIFICATION
            var qualifications = (await _unitOfWork.Repository<CandidateQualification>()
                .GetAllAsync())
                .Where(x => x.CandidateId == candidateId)
                .ToList();

            foreach (var qual in qualifications)
            {
                qual.CompanyId = companyId;
                qual.RegionId = regionId;
                qual.UserId = userId;

                _unitOfWork.Repository<CandidateQualification>().Update(qual);
            }

            // 5. SAVE ALL CHANGES
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<List<JobApplicationDto>> GetJobApplicationsAsync()
        {
            var data = await _unitOfWork.Repository<JobApplication>()
                .GetAllAsync();

            return data
            .OrderByDescending(x => x.AppliedDate)
            .Select(x => new JobApplicationDto
            {
                ApplicationId = x.ApplicationId,
                CandidateName = x.CandidateName,
                Email = x.Email,
                Phone = x.Phone,
                JobTitle = x.JobTitle,
                ExperienceYears = x.ExperienceYears,
                Technology = x.Technology,
                ResumeUrl = x.ResumeUrl,

                Status = x.Status,
                AppliedDate = x.AppliedDate
            })
            .ToList();
        }
        public async Task<List<CandidateDocumentWithCandidateDto>> GetAllCandidateDocuments(int companyId, int regionId)
        {
            var result = await (
                from doc in _hRMSContext.CandidateDocumentChecklists
                join c in _hRMSContext.Candidates
                    on doc.CandidateId equals c.CandidateId   // 🔥 FIX HERE

                where doc.CompanyId == companyId
                      && doc.RegionId == regionId

                select new CandidateDocumentWithCandidateDto
                {
                    Id = doc.Id,
                    CompanyId = doc.CompanyId,
                    RegionId = doc.RegionId,
                    CandidateId = doc.CandidateId,
                    OfferId = doc.OfferId,

                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Designation = c.Designation,
                    Department = c.Department,

                    Status = doc.Status,

                    AadharCard = doc.AadharCard,
                    PanCard = doc.PanCard,
                    Passport = doc.Passport,
                    IdProof = doc.IdProof,
                    OfferLetter = doc.OfferLetter,
                    ExperienceLetter = doc.ExperienceLetter,
                    RelievingLetter = doc.RelievingLetter,
                    HikeLetter = doc.HikeLetter
                }
            ).ToListAsync();

            return result;
        }
        public async Task<object> GetOfferByIdAsync(int offerId)
        {
            var offer = await _unitOfWork.Repository<CandidateOffer>()
                .GetByIdAsync(offerId);

            if (offer == null)
                return null;

            var candidate = await _unitOfWork.Repository<Candidate>()
                .GetByIdAsync(offer.CandidateId);

            return new
            {
                offer.OfferId,
                offer.CandidateId,
                offer.CompanyId,
                offer.RegionId,

                CandidateName = candidate.FirstName + " " + candidate.LastName,
                Designation = candidate.Designation
            };
        }

        public async Task<List<string>> GetRecruitmentDepartmentsAsync(
     int companyId,
     int regionId)
        {
            var departments = await (
                from d in _hRMSContext.Departments

                where d.CompanyId == companyId
                      && d.RegionId == regionId
                      && d.IsDeleted == false

                select d.DepartmentName
            )
            .Union(

                from c in _hRMSContext.Candidates

                where c.CompanyId == companyId
                      && c.RegionId == regionId
                      && !string.IsNullOrEmpty(c.Department)

                select c.Department
            )
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

            return departments;
        }

        public async Task<List<string>> GetRecruitmentDesignationsAsync(
            int companyId,
            int regionId)
        {
            var designations = await (
                from d in _hRMSContext.Designations

                where d.CompanyId == companyId
                      && d.RegionId == regionId
                      && d.IsDeleted == false

                select d.DesignationName
            )
            .Union(

                from c in _hRMSContext.Candidates

                where c.CompanyId == companyId
                      && c.RegionId == regionId
                      && !string.IsNullOrEmpty(c.Designation)

                select c.Designation
            )
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

            return designations;
        }
        public async Task<bool> UpdateChecklistStatusAsync(int offerId, int companyId, int regionId, string status)
        {
            var record = await _hRMSContext.CandidateDocumentChecklists
                .FirstOrDefaultAsync(x =>
                    x.OfferId == offerId &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId);

            if (record == null)
                return false;

            record.Status = status;

            await _hRMSContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UploadCandidateDocumentsAsync(UploadCandidateDocumentsDto dto)
        {
            string folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Uploads",
                "CandidateDocuments"
            );

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            async Task<string?> SaveFile(IFormFile? file)
            {
                if (file == null || file.Length == 0)
                    return null;

                string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }

            var repo = _unitOfWork.Repository<CandidateDocumentChecklist>();

            // ================= FIND EXISTING ROW =================
            var existing = (await repo.FindAsync(x =>
                x.OfferId == dto.OfferId &&
                x.CandidateId == dto.CandidateId &&
                x.CompanyId == dto.CompanyId &&
                x.RegionId == dto.RegionId
            )).FirstOrDefault();

            // ================= IF NOT FOUND → STOP =================
            if (existing == null)
            {
                throw new Exception("Invalid request: Offer record not found for this candidate.");
            }

            // ================= UPDATE ONLY =================

            if (dto.AadharCard != null)
                existing.AadharCard = await SaveFile(dto.AadharCard);

            if (dto.PanCard != null)
                existing.PanCard = await SaveFile(dto.PanCard);

            if (dto.Passport != null)
                existing.Passport = await SaveFile(dto.Passport);

            if (dto.IdProof != null)
                existing.IdProof = await SaveFile(dto.IdProof);

            if (dto.OfferLetter != null)
                existing.OfferLetter = await SaveFile(dto.OfferLetter);

            if (dto.ExperienceLetter != null)
                existing.ExperienceLetter = await SaveFile(dto.ExperienceLetter);

            if (dto.RelievingLetter != null)
                existing.RelievingLetter = await SaveFile(dto.RelievingLetter);

            if (dto.HikeLetter != null)
                existing.HikeLetter = await SaveFile(dto.HikeLetter);

            existing.Status = "Submitted";
            existing.UpdatedDate = DateTime.Now;

            repo.Update(existing);

            await _unitOfWork.CompleteAsync();

            return true;
        }

    }
}