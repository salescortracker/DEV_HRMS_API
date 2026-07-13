using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class PerformanceService: IPerformanceService
    {
        private readonly IEmailService _emailService;
        private readonly HRMSContext _context;

        public PerformanceService(HRMSContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ===============================
        // GET BY USER ID (Employee View)
        // ===============================
        public async Task<ApiResponse<IEnumerable<PerformanceReviewDto>>> GetByUserIdAsync(int userId)
        {
            var list = await _context.PerformanceReviews
                .Where(x => x.UserId == userId)
                .Include(x => x.KPIs)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var result = list.Select(review => new PerformanceReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                RoleId = review.RoleId,
                DepartmentProject = review.DepartmentProject,
                ReportingManagerId = review.ReportingManagerId,
                Designation = review.Designation,
                Department = review.Department,
                DateOfJoining = review.DateOfJoining,
                ProbationStatus = review.ProbationStatus,
                PerformanceCycle = review.PerformanceCycle,
                ApplicableStartDate = review.ApplicableStartDate,
                ApplicableEndDate = review.ApplicableEndDate,
                ProgressType = review.ProgressType,
                AppraisalYear = review.AppraisalYear,
                DocumentEvidence = review.DocumentEvidence,
                SelfReviewSummary = review.SelfReviewSummary,
                Status = review.Status,

                KPIs = review.KPIs?.Select(k => new PerformanceKPIDto
                {
                    Id = k.Id,
                    KPIName = k.Kpiname,
                    Weightage = k.Weightage,
                    Target = k.Target,
                    Achieved = k.Achieved,
                    SelfRating = k.SelfRating,
                    ManagerRating = k.ManagerRating,
                    Remarks = k.Remarks
                }).ToList()
            }).ToList();

            return new ApiResponse<IEnumerable<PerformanceReviewDto>>(result);
        }

        // ===============================
        // SAVE DRAFT / SUBMIT
        // ===============================
        public async Task<ApiResponse<bool>> SaveAsync(PerformanceReviewDto dto)
        {
            PerformanceReview review;

            if (dto.Id == null || dto.Id == 0)
            {
                review = new PerformanceReview
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId,
                    DepartmentProject = dto.DepartmentProject,
                    ReportingManagerId = dto.ReportingManagerId,
                    Designation = dto.Designation,
                    Department = dto.Department,
                    DateOfJoining = dto.DateOfJoining,
                    ProbationStatus = dto.ProbationStatus,
                    PerformanceCycle = dto.PerformanceCycle,
                    ApplicableStartDate = dto.ApplicableStartDate,
                    ApplicableEndDate = dto.ApplicableEndDate,
                    ProgressType = dto.ProgressType,
                    AppraisalYear = dto.AppraisalYear,
                    DocumentEvidence = dto.DocumentEvidence,
                    SelfReviewSummary = dto.SelfReviewSummary,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = dto.UserId,
                    HrEmail = dto.HrEmail, // ✅ ADD
                };

                _context.PerformanceReviews.Add(review);
                await _context.SaveChangesAsync();
            }
            else
            {
                review = await _context.PerformanceReviews
                    .Include(x => x.KPIs)
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                if (review == null)
                    return new ApiResponse<bool>(false, "Record not found");

                review.DepartmentProject = dto.DepartmentProject;
                review.ProbationStatus = dto.ProbationStatus;
                review.PerformanceCycle = dto.PerformanceCycle;
                review.ApplicableStartDate = dto.ApplicableStartDate;
                review.ApplicableEndDate = dto.ApplicableEndDate;
                review.ProgressType = dto.ProgressType;
                review.AppraisalYear = dto.AppraisalYear;
                review.DocumentEvidence = dto.DocumentEvidence;
                review.SelfReviewSummary = dto.SelfReviewSummary;
                review.Status = dto.Status;
                review.ModifiedAt = DateTime.UtcNow;
                review.ModifiedBy = dto.UserId;
                review.HrEmail = dto.HrEmail; // ✅ ADD

                await _context.SaveChangesAsync();

                // Remove old KPIs
                _context.PerformanceKpis.RemoveRange(review.KPIs);
                await _context.SaveChangesAsync();
            }

            // Save KPI List
            if (dto.KPIs != null && dto.KPIs.Any())
            {
                foreach (var kpi in dto.KPIs)
                {
                    var entity = new PerformanceKpi
                    {
                        PerformanceReviewId = review.Id,
                        Kpiname = kpi.KPIName,
                        Weightage = kpi.Weightage,
                        Target = kpi.Target,
                        Achieved = kpi.Achieved,
                        SelfRating = kpi.SelfRating,
                        ManagerRating = kpi.ManagerRating,
                        Remarks = kpi.Remarks
                    };

                    _context.PerformanceKpis.Add(entity);
                }

                await _context.SaveChangesAsync();
            }
            // ================= EMAIL LOGIC =================

            //        // 🔹 Get Employee
            //        var employee = await _context.Users
            //            .Where(x => x.UserId == dto.UserId)
            //            .Select(x => new { x.Email, x.FullName })
            //            .FirstOrDefaultAsync();

            //        // 🔹 Get Manager
            //        var manager = await _context.Users
            //            .Where(x => x.UserId == dto.ReportingManagerId)
            //            .Select(x => new { x.Email, x.FullName })
            //            .FirstOrDefaultAsync();

            //        // 🔹 EMAIL TO MANAGER (Submission)
            //        if (manager != null && !string.IsNullOrEmpty(manager.Email))
            //        {
            //            var body = $@"
            //<div style='font-family:Arial'>
            //    <h3>KPI Submission Notification</h3>

            //    <p>Dear {manager.FullName},</p>

            //    <p>An employee has submitted KPI review.</p>

            //    <table border='1' cellpadding='6' cellspacing='0'>
            //        <tr><td><b>Employee</b></td><td>{employee?.FullName}</td></tr>
            //        <tr><td><b>Project</b></td><td>{dto.DepartmentProject}</td></tr>
            //        <tr><td><b>Cycle</b></td><td>{dto.PerformanceCycle}</td></tr>
            //        <tr><td><b>Appraisal Year</b></td><td>{dto.AppraisalYear}</td></tr>
            //    </table>

            //    <p>Please review and take action.</p>

            //    <br/>
            //    <p>Regards,<br/><b>HRMS Team</b></p>
            //</div>";

            //            await _emailService.SendEmailAsync(
            //                manager.Email,
            //                "KPI Submitted for Review",
            //                body,
            //                string.IsNullOrEmpty(dto.HrEmail)
            //                    ? null
            //                    : new List<string> { dto.HrEmail } // ✅ CC EMAIL
            //            );
            //        }

            //        // 🔹 EMAIL TO EMPLOYEE (Confirmation)
            //        if (employee != null && !string.IsNullOrEmpty(employee.Email))
            //        {
            //            var body = $@"
            //<div style='font-family:Arial'>
            //    <h3>KPI Submitted Successfully</h3>

            //    <p>Dear {employee.FullName},</p>

            //    <p>Your KPI has been submitted successfully.</p>

            //    <table border='1' cellpadding='6' cellspacing='0'>
            //        <tr><td><b>Project</b></td><td>{dto.DepartmentProject}</td></tr>
            //        <tr><td><b>Cycle</b></td><td>{dto.PerformanceCycle}</td></tr>
            //        <tr><td><b>Appraisal Year</b></td><td>{dto.AppraisalYear}</td></tr>
            //    </table>

            //    <br/>
            //    <p>Regards,<br/><b>HRMS Team</b></p>
            //</div>";

            //            await _emailService.SendEmailAsync(
            //                employee.Email,
            //                "KPI Submission Confirmation",
            //                body,
            //                string.IsNullOrEmpty(dto.HrEmail)
            //                    ? null
            //                    : new List<string> { dto.HrEmail } // ✅ CC EMAIL
            //            );
            //        }

            // ================= EMAIL LOGIC =================

            if (dto.Status == "Submitted")
            {
                // 🔹 Get Employee
                //var employee = await _context.Users
                //    .Where(x => x.UserId == dto.UserId)
                //    .Select(x => new { x.Email, x.FullName })
                //    .FirstOrDefaultAsync();

                var employee = await _context.Users
    .Where(x => x.UserId == dto.UserId)
    .Select(x => new
    {
        x.Email,
        x.FullName,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();

                string? reportingHrEmail = null;

                if (employee?.ReportingHr != null)
                {
                    var reportingHrUser = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                    reportingHrEmail = reportingHrUser?.Email;
                }

                // 🔹 Get Manager
                var manager = await _context.Users
                    .Where(x => x.UserId == dto.ReportingManagerId)
                    .Select(x => new { x.Email, x.FullName })
                    .FirstOrDefaultAsync();

                // 🔹 EMAIL TO MANAGER
                if (manager != null && !string.IsNullOrEmpty(manager.Email))
                {
                    var body = $@"
        <div style='font-family:Arial'>
            <h3>KPI Submission Notification</h3>

            <p>Dear {manager.FullName},</p>

            <p>An employee has submitted KPI review.</p>

            <table border='1' cellpadding='6' cellspacing='0'>
                <tr><td><b>Employee</b></td><td>{employee?.FullName}</td></tr>
                <tr><td><b>Project</b></td><td>{dto.DepartmentProject}</td></tr>
                <tr><td><b>Cycle</b></td><td>{dto.PerformanceCycle}</td></tr>
                <tr><td><b>Appraisal Year</b></td><td>{dto.AppraisalYear}</td></tr>
            </table>

            <p>Please review and take action.</p>

            <br/>
            <p>Regards,<br/><b>HRMS Team</b></p>
        </div>";

                    //await _emailService.SendEmailAsync(
                    //    manager.Email,
                    //    "KPI Submitted for Review",
                    //    body,
                    //    string.IsNullOrEmpty(dto.HrEmail)
                    //        ? null
                    //        : new List<string> { dto.HrEmail }
                    //);

                    var ccList = new List<string>();

                    // Reporting HR
                    if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                    {
                        ccList.Add(reportingHrEmail);
                    }

                    // UI CC Emails
                    if (!string.IsNullOrWhiteSpace(dto.HrEmail))
                    {
                        ccList.AddRange(
                            dto.HrEmail
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                        );
                    }

                    ccList = ccList.Distinct().ToList();

                    await _emailService.SendEmailAsync(
                        manager.Email,
                        "KPI Submitted for Review",
                        body,
                        ccList
                    );
                }

                // 🔹 EMAIL TO EMPLOYEE
        //        if (employee != null && !string.IsNullOrEmpty(employee.Email))
        //        {
        //            var body = $@"
        //<div style='font-family:Arial'>
        //    <h3>KPI Submitted Successfully</h3>

        //    <p>Dear {employee.FullName},</p>

        //    <p>Your KPI has been submitted successfully.</p>

        //    <table border='1' cellpadding='6' cellspacing='0'>
        //        <tr><td><b>Project</b></td><td>{dto.DepartmentProject}</td></tr>
        //        <tr><td><b>Cycle</b></td><td>{dto.PerformanceCycle}</td></tr>
        //        <tr><td><b>Appraisal Year</b></td><td>{dto.AppraisalYear}</td></tr>
        //    </table>

        //    <br/>
        //    <p>Regards,<br/><b>HRMS Team</b></p>
        //</div>";

        //            await _emailService.SendEmailAsync(
        //                employee.Email,
        //                "KPI Submission Confirmation",
        //                body,
        //                string.IsNullOrEmpty(dto.HrEmail)
        //                    ? null
        //                    : new List<string> { dto.HrEmail }
        //            );
        //        }
            }

            return new ApiResponse<bool>(true);
        }

        // ===============================
        // GET MANAGER REVIEWS
        // ===============================
        public async Task<ApiResponse<IEnumerable<PerformanceReviewDto>>> GetManagerReviewsAsync(int loggedInUserId)
        {
            var reviews = await (
                from review in _context.PerformanceReviews
                join user in _context.Users
                    on review.UserId equals user.UserId
                where review.ReportingManagerId == loggedInUserId
                      && review.Status != "Draft"
                select new { review, user }
            ).ToListAsync();

            if (!reviews.Any())
            {
                return new ApiResponse<IEnumerable<PerformanceReviewDto>>(
                    new List<PerformanceReviewDto>(),
                    "No records found"
                );
            }

            var result = reviews.Select(x => new PerformanceReviewDto
            {
                Id = x.review.Id,
                UserId = x.review.UserId,
                RoleId = x.review.RoleId,

                DepartmentProject = x.review.DepartmentProject,
                ReportingManagerId = x.review.ReportingManagerId,
                Designation = x.review.Designation,
                Department = x.review.Department,
                DateOfJoining = x.review.DateOfJoining,

                ProbationStatus = x.review.ProbationStatus,
                PerformanceCycle = x.review.PerformanceCycle,
                ApplicableStartDate = x.review.ApplicableStartDate,
                ApplicableEndDate = x.review.ApplicableEndDate,
                ProgressType = x.review.ProgressType,
                AppraisalYear = x.review.AppraisalYear,
                DocumentEvidence = x.review.DocumentEvidence,

                SelfReviewSummary = x.review.SelfReviewSummary,
                Status = x.review.Status,

                // 🔥 CORRECT WAY TO GET EMPLOYEE NAME
                EmployeeName = x.user.FullName,

                KPIs = _context.PerformanceKpis
                    .Where(k => k.PerformanceReviewId == x.review.Id)
                    .Select(k => new PerformanceKPIDto
                    {
                        Id = k.Id,
                        KPIName = k.Kpiname,
                        Weightage = k.Weightage,
                        Target = k.Target,
                        Achieved = k.Achieved,
                        SelfRating = k.SelfRating,
                        ManagerRating = k.ManagerRating,
                        Remarks = k.Remarks
                    }).ToList()
            }).ToList();

            return new ApiResponse<IEnumerable<PerformanceReviewDto>>(result);
        }

        // ===============================
        // APPROVE
        // ===============================
        public async Task<ApiResponse<bool>> ApproveAsync(int reviewId, int managerId, string remarks)
        {
            var review = await _context.PerformanceReviews
        .FirstOrDefaultAsync(x => x.Id == reviewId && x.ReportingManagerId == managerId);

            if (review == null)
                return new ApiResponse<bool>(false, "Record not found");

            review.Status = "Approved";
            review.ManagerRemarks = remarks;
            review.ModifiedAt = DateTime.UtcNow;
            review.ModifiedBy = managerId;

            await _context.SaveChangesAsync();



            // GET EMPLOYEE

            //var employee = await _context.Users
            //       .Where(x => x.UserId == review.UserId)
            //       .Select(x => new
            //       {
            //           x.Email,
            //           x.FullName
            //       })
            //       .FirstOrDefaultAsync();

            var employee = await _context.Users
    .Where(x => x.UserId == review.UserId)
    .Select(x => new
    {
        x.Email,
        x.FullName,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();

            string? reportingHrEmail = null;

            if (employee?.ReportingHr != null)
            {
                var reportingHrUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                reportingHrEmail = reportingHrUser?.Email;
            }

            // GET MANAGER

            var manager = await _context.Users
                .Where(x => x.UserId == managerId)
                .Select(x => new
                {
                    x.Email,
                    x.FullName
                })
                .FirstOrDefaultAsync();

            // EMAIL TO EMPLOYEE

            if (employee != null && !string.IsNullOrEmpty(employee.Email))
            {
                var body = $@"
                  <div style='font-family:Arial'>
 
                      <h3>KPI Review Approved</h3>
 
                      <p>Dear {employee.FullName},</p>
 
                      <p>Your KPI review has been approved by manager.</p>
 
                      <table border='1' cellpadding='6' cellspacing='0'>
                          <tr>
                              <td><b>Status</b></td>
                              <td>Approved</td>
                          </tr>
 
                          <tr>
                              <td><b>Manager Remarks</b></td>
                              <td>{remarks}</td>
                          </tr>
                      </table>
 
                      <br/>
 
                      <p>Regards,<br/><b>HRMS Team</b></p>
 
                  </div>";

                var ccList = new List<string>();

                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccList.Add(reportingHrEmail);
                }

                if (!string.IsNullOrWhiteSpace(review.HrEmail))
                {
                    ccList.AddRange(
                        review.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                    );
                }

                ccList = ccList.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    employee.Email,
                    "KPI Review Approved",
                    body,
                    ccList
                );
            }


            return new ApiResponse<bool>(true);
        }

        // ===============================
        // REJECT
        // ===============================
        public async Task<ApiResponse<bool>> RejectAsync(int reviewId, int managerId, string remarks)
        {
            var review = await _context.PerformanceReviews
          .FirstOrDefaultAsync(x => x.Id == reviewId && x.ReportingManagerId == managerId);

            if (review == null)
                return new ApiResponse<bool>(false, "Record not found");

            review.Status = "Rejected";
            review.ManagerRemarks = remarks;
            review.ModifiedAt = DateTime.UtcNow;
            review.ModifiedBy = managerId;

            await _context.SaveChangesAsync();


            // GET EMPLOYEE

            //var employee = await _context.Users
            //    .Where(x => x.UserId == review.UserId)
            //    .Select(x => new
            //    {
            //        x.Email,
            //        x.FullName
            //    })
            //    .FirstOrDefaultAsync();

            var employee = await _context.Users
    .Where(x => x.UserId == review.UserId)
    .Select(x => new
    {
        x.Email,
        x.FullName,
        x.ReportingHr
    })
    .FirstOrDefaultAsync();

            string? reportingHrEmail = null;

            if (employee?.ReportingHr != null)
            {
                var reportingHrUser = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserId == employee.ReportingHr);

                reportingHrEmail = reportingHrUser?.Email;
            }

            // EMAIL TO EMPLOYEE

            if (employee != null && !string.IsNullOrEmpty(employee.Email))
            {
                var body = $@"
             <div style='font-family:Arial'>
 
                 <h3>KPI Review Rejected</h3>
 
                 <p>Dear {employee.FullName},</p>
 
                 <p>Your KPI review has been rejected by manager.</p>
 
                 <table border='1' cellpadding='6' cellspacing='0'>
 
                     <tr>
                         <td><b>Status</b></td>
                         <td>Rejected</td>
                     </tr>
 
                     <tr>
                         <td><b>Manager Remarks</b></td>
                         <td>{remarks}</td>
                     </tr>
 
                 </table>
 
                 <br/>
 
                 <p>Please update and resubmit your KPI review.</p>
 
                 <br/>
 
                 <p>Regards,<br/><b>HRMS Team</b></p>
 
             </div>";

                //await _emailService.SendEmailAsync(

                //    employee.Email,
                //    "KPI Review Rejected",
                //    body,
                //    string.IsNullOrEmpty(review.HrEmail)
                //        ? null
                //        : new List<string> { review.HrEmail }
                //);

                var ccList = new List<string>();

                if (!string.IsNullOrWhiteSpace(reportingHrEmail))
                {
                    ccList.Add(reportingHrEmail);
                }

                if (!string.IsNullOrWhiteSpace(review.HrEmail))
                {
                    ccList.AddRange(
                        review.HrEmail
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Where(x => !string.IsNullOrEmpty(x))
                    );
                }

                ccList = ccList.Distinct().ToList();

                await _emailService.SendEmailAsync(
                    employee.Email,
                    "KPI Review Rejected",
                    body,
                    ccList
                );
            }

            return new ApiResponse<bool>(true);
        }

        public async Task<ApiResponse<bool>> RequestAsync(int reviewId)
        {
            var review = await _context.PerformanceReviews
                .FirstOrDefaultAsync(x => x.Id == reviewId);

            if (review == null)
                return new ApiResponse<bool>(false, "Record not found");

            review.Status = "Submitted"; // reset to submitted
            review.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true);
        }
        public async Task<ApiResponse<List<PerformanceReviewDto>>> GetEmployeeSubmissions(int userId)
        {
            var data = await _context.PerformanceReviews
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .Select(x => new PerformanceReviewDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    DepartmentProject = x.DepartmentProject,
                    PerformanceCycle = x.PerformanceCycle,
                    AppraisalYear = x.AppraisalYear,
                    Status = x.Status,
                    SelfReviewSummary = x.SelfReviewSummary,
                    ApplicableStartDate = x.ApplicableStartDate,
                    ApplicableEndDate = x.ApplicableEndDate,
                    Designation = x.Designation,
                    Department = x.Department,
                })
                .ToListAsync();

            return new ApiResponse<List<PerformanceReviewDto>>(data);
        }
        public async Task<ApiResponse<List<object>>> GetPerformanceReports(
 int userId,
 string roleName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                return new ApiResponse<List<object>>
                {
                    Success = false,
                    Message = "User not found",
                    Data = new List<object>()
                };
            }

            var query =
                from pr in _context.PerformanceReviews
                join u in _context.Users
                on pr.UserId equals u.UserId
                select new
                {
                    pr,
                    u
                };

            // HR
            if (roleName.ToLower() == "hr")
            {
                query = query.Where(x =>
                     //x.u.CompanyId == user.CompanyId &&
                     //x.u.RegionId == user.RegionId);
                     x.u.ReportingHr == userId);
            }

            // MANAGER
            else
            {
                query = query.Where(x =>
                    x.pr.ReportingManagerId == userId);
            }
            // Hide Draft records
            query = query.Where(x => x.pr.Status != "Draft");

            var result = await query
                .Select(x => new
                {
                    employeeName = x.u.FullName,
                    department = x.pr.Department,
                    departmentProject = x.pr.DepartmentProject,
                    appraisalYear = x.pr.AppraisalYear,
                    status = x.pr.Status
                })
                .ToListAsync<object>();

            return new ApiResponse<List<object>>
            {
                Success = true,
                Message = "Performance reports fetched successfully",
                Data = result
            };
        }



    }
}
