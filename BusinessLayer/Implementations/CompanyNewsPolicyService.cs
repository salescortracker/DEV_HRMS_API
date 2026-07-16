using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class CompanyNewsPolicyService: ICompanyNewsPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly HRMSContext _context;
        private readonly INotificationService _notificationService;

        public CompanyNewsPolicyService(IUnitOfWork unitOfWork, IEmailService emailService, HRMSContext context, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _context = context;
            _notificationService = notificationService;
        }

        // =========================================================
        // ====================== COMPANY NEWS =====================
        // =========================================================

        /// <summary>
        /// Get all news based on UserId
        /// </summary>
        public async Task<IEnumerable<CompanyNewsMasterDto>> GetAllNewsAsync(int userId)
        {
            var news = await _unitOfWork
                .Repository<CompanyNewsMaster>()
                .GetAllAsync();

            var mappings = await _unitOfWork
                .Repository<CompanyNewsDepartment>()
                .GetAllAsync();

            return news
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.NewsId)
                .Select(x => new CompanyNewsMasterDto
                {
                    NewsId = x.NewsId,
                    Title = x.Title,
                    Description = x.Description,
                    PostedDate = x.PostedDate,
                    ExpiryDate = x.ExpiryDate,
                    IsActive = x.IsActive,
                    UserId = x.UserId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    Category = x.Category,
                    AttachmentName = x.AttachmentName,
                    AttachmentPath = x.AttachmentPath,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,

                    DepartmentIds = mappings
                        .Where(m => m.NewsId == x.NewsId)
                        .Select(m => m.DepartmentId)
                        .ToList()
                })
                .ToList();
        }

        /// <summary>
        /// Get only today's news based on PostedDate and UserId
        /// </summary>
        //public async Task<IEnumerable<CompanyNewsMasterDto>> GetTodayNewsAsync(int userId)
        //{
        //    var users = await _unitOfWork.Repository<User>().GetAllAsync();

        //    var user = users.FirstOrDefault(x => x.UserId == userId);

        //    if (user == null)
        //        return new List<CompanyNewsMasterDto>();

        //    var departmentId = user.DepartmentId;

        //    var today = DateOnly.FromDateTime(DateTime.Now);

        //    var news = await _unitOfWork.Repository<CompanyNewsMaster>().GetAllAsync();

        //    return news
        //        .Where(x =>
        //            x.DepartmentId == departmentId &&
        //            x.IsActive == true &&
        //            x.PostedDate.HasValue &&
        //            x.PostedDate.Value == today)
        //        .Select(MapNewsToDto)
        //        .ToList();
        //}

        public async Task<IEnumerable<CompanyNewsMasterDto>> GetTodayNewsAsync(
      int companyId,
      int regionId)
        {
            var news = await _unitOfWork.Repository<CompanyNewsMaster>().GetAllAsync();

            return news
                .Where(x =>
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive)
                .Select(MapNewsToDto)
                .ToList();
        }

        /// <summary>
        /// Get news by Id & UserId
        /// </summary>
        public async Task<CompanyNewsMasterDto?> GetNewsByIdAsync(int id, int userId)
        {
            var entity = await _unitOfWork.Repository<CompanyNewsMaster>().GetByIdAsync(id);

            if (entity == null || entity.UserId != userId)
                return null;

            return MapNewsToDto(entity);
        }

        /// <summary>
        /// Add new news
        /// </summary>
        //public async Task<CompanyNewsMasterDto> AddNewsAsync(CompanyNewsMasterDto dto)
        //{
        //    string? fileName = null;
        //    string? filePath = null;

        //    if (dto.Attachment != null)
        //    {
        //        var uploadsFolder = Path.Combine(
        //            Directory.GetCurrentDirectory(),
        //            "wwwroot",
        //            "news"
        //        );

        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }

        //        fileName = Guid.NewGuid().ToString()
        //                   + Path.GetExtension(dto.Attachment.FileName);

        //        var fullPath = Path.Combine(uploadsFolder, fileName);

        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            await dto.Attachment.CopyToAsync(stream);
        //        }

        //        filePath = "/news/" + fileName;
        //    }

        //    var entity = new CompanyNewsMaster
        //    {
        //        Title = dto.Title,
        //        Description = dto.Description,
        //        PostedDate = dto.PostedDate,
        //        ExpiryDate = dto.ExpiryDate,
        //        Category = dto.Category,
        //        IsActive = true,
        //        UserId = dto.UserId,
        //        CompanyId = dto.CompanyId,
        //        RegionId = dto.RegionId,
        //        AttachmentName = fileName,
        //        AttachmentPath = filePath,
        //        CreatedBy = dto.CreatedBy,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _unitOfWork
        //        .Repository<CompanyNewsMaster>()
        //        .AddAsync(entity);

        //    await _unitOfWork.CompleteAsync();

        //    if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
        //    {
        //        foreach (var deptId in dto.DepartmentIds)
        //        {
        //            await _unitOfWork
        //                .Repository<CompanyNewsDepartment>()
        //                .AddAsync(new CompanyNewsDepartment
        //                {
        //                    NewsId = entity.NewsId,
        //                    DepartmentId = deptId,
        //                    CreatedAt = DateTime.UtcNow
        //                });
        //        }

        //        await _unitOfWork.CompleteAsync();
        //    }

        //    return MapNewsToDto(entity);
        //}


        public async Task<CompanyNewsMasterDto> AddNewsAsync(CompanyNewsMasterDto dto)
        {
            string? fileName = null;
            string? filePath = null;

            //=============================
            // Upload Attachment
            //=============================

            if (dto.Attachment != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "news"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                fileName = Guid.NewGuid().ToString() +
                           Path.GetExtension(dto.Attachment.FileName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                filePath = "/news/" + fileName;
            }

            //=============================
            // Save News
            //=============================

            var entity = new CompanyNewsMaster
            {
                Title = dto.Title,
                Description = dto.Description,
                PostedDate = dto.PostedDate,
                ExpiryDate = dto.ExpiryDate,
                Category = dto.Category,
                IsActive = true,
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                AttachmentName = fileName,
                AttachmentPath = filePath,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork
                .Repository<CompanyNewsMaster>()
                .AddAsync(entity);

            await _unitOfWork.CompleteAsync();
            //=========================================
            // CREATE SYSTEM NOTIFICATION
            //=========================================

            var notificationUsers = await _context.Users
                .Where(x =>
                    x.CompanyId == dto.CompanyId &&
                    x.RegionId == dto.RegionId &&
                    x.Status == "Active" &&
                    (
                        dto.DepartmentIds == null ||
                        !dto.DepartmentIds.Any() ||
                        dto.DepartmentIds.Contains((int)x.DepartmentId)
                    ))
                .Select(x => x.UserId)
                .ToListAsync();


            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "New Company Announcement",
                    dto.Title,
                    "CompanyNews",
                    entity.NewsId
                );
            }

            //=============================
            // Save Departments
            //=============================

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    await _unitOfWork
                        .Repository<CompanyNewsDepartment>()
                        .AddAsync(new CompanyNewsDepartment
                        {
                            NewsId = entity.NewsId,
                            DepartmentId = deptId,
                            CreatedAt = DateTime.UtcNow
                        });
                }

                await _unitOfWork.CompleteAsync();
            }

            //=============================
            // Get Employee Emails
            //=============================

            List<string> emails = new();

            if (dto.DepartmentIds == null || !dto.DepartmentIds.Any())
            {
                // Send to all employees in Company & Region

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                // Send only selected departments

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=============================
            // Send Mail
            //=============================

            if (emails.Any())
            {
                string subject = $"Company News - {dto.Title}";

                string body = $@"
<html>
<body style='font-family:Calibri'>

<h2 style='color:#0d6efd;'>Company News</h2>

<p>Dear Employee,</p>

<p>A new company announcement has been published.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
<td><b>Title</b></td>
<td>{dto.Title}</td>
</tr>

<tr>
<td><b>Category</b></td>
<td>{dto.Category}</td>
</tr>

<tr>
<td><b>Posted Date</b></td>
<td>{dto.PostedDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Expiry Date</b></td>
<td>{dto.ExpiryDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Description</b></td>
<td>{dto.Description}</td>
</tr>

</table>

<br/>

<p>Please login to the HRMS Portal for more details.</p>

<br/>

Regards,<br/>
<b>HR Team</b>

</body>
</html>";

                List<string>? attachments = null;

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    string physicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(physicalPath))
                    {
                        attachments = new List<string>
                {
                    physicalPath
                };
                    }
                }

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body,
                            null,
                            attachments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Mail failed to {email}: {ex.Message}");
                    }
                }
            }

            return MapNewsToDto(entity);
        }

        /// <summary>
        /// Update existing news
        /// </summary>
        public async Task<CompanyNewsMasterDto> UpdateNewsAsync(
     int id,
     CompanyNewsMasterDto dto)
        {
            var entity = await _unitOfWork
                .Repository<CompanyNewsMaster>()
                .GetByIdAsync(id);

            if (entity == null)
                throw new Exception("News not found");

            // ✅ Update Basic Fields
            entity.Title = dto.Title;

            entity.Description = dto.Description;

            entity.PostedDate = dto.PostedDate;

            entity.Category = dto.Category;


            entity.CompanyId = dto.CompanyId;

            entity.RegionId = dto.RegionId;

            entity.ExpiryDate = dto.ExpiryDate;

            entity.IsActive = dto.IsActive;

            entity.UpdatedBy = dto.UpdatedBy;

            entity.UpdatedAt = DateTime.UtcNow;

            // ✅ Upload New File
            if (dto.Attachment != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "news"
                );

                // Create folder if not exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique file name
                var fileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(dto.Attachment.FileName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                // Optional: Delete old file
                if (!string.IsNullOrEmpty(entity.AttachmentPath))
                {
                    var oldFile = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.AttachmentPath.TrimStart('/')
                    );

                    if (File.Exists(oldFile))
                    {
                        File.Delete(oldFile);
                    }
                }

                // ✅ Save new attachment details
                entity.AttachmentName = fileName;

                entity.AttachmentPath = "/news/" + fileName;
            }

            _unitOfWork
    .Repository<CompanyNewsMaster>()
    .Update(entity);

            await _unitOfWork.CompleteAsync();
            var notificationUsers = await _context.Users
    .Where(x =>
        x.CompanyId == dto.CompanyId &&
        x.RegionId == dto.RegionId &&
        x.Status == "Active" &&
        (
            dto.DepartmentIds == null ||
            !dto.DepartmentIds.Any() ||
            dto.DepartmentIds.Contains((int)x.DepartmentId)
        ))
    .Select(x => x.UserId)
    .ToListAsync();


            if (notificationUsers.Any())
            {
                await _notificationService.CreateNotificationAsync(
                    notificationUsers,
                    "Company News Updated",
                    entity.Title,
                    "CompanyNews",
                    entity.NewsId
                );
            }


            // Existing mappings delete
            var existingMappings =
                (await _unitOfWork
                    .Repository<CompanyNewsDepartment>()
                    .GetAllAsync())
                .Where(x => x.NewsId == entity.NewsId)
                .ToList();

            foreach (var item in existingMappings)
            {
                _unitOfWork
                    .Repository<CompanyNewsDepartment>()
                    .Remove(item);
            }

            await _unitOfWork.CompleteAsync();


            // Add new mappings
            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    await _unitOfWork
                        .Repository<CompanyNewsDepartment>()
                        .AddAsync(new CompanyNewsDepartment
                        {
                            NewsId = entity.NewsId,
                            DepartmentId = deptId,
                            CreatedAt = DateTime.UtcNow
                        });
                }

                await _unitOfWork.CompleteAsync();
            }
            //=========================================
            // Get User Emails
            //=========================================

            List<string> emails = new();

            if (dto.DepartmentIds == null || !dto.DepartmentIds.Any())
            {
                // All employees in Company & Region

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                // Selected Departments

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=========================================
            // Send Mail
            //=========================================

            if (emails.Any())
            {
                string subject = $"Updated Company News - {entity.Title}";

                string body = $@"
<html>

<body style='font-family:Calibri'>

<h2 style='color:#0d6efd;'>Company News Updated</h2>

<p>Dear Employee,</p>

<p>An existing company announcement has been updated.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
<td><b>Title</b></td>
<td>{entity.Title}</td>
</tr>

<tr>
<td><b>Category</b></td>
<td>{entity.Category}</td>
</tr>

<tr>
<td><b>Posted Date</b></td>
<td>{entity.PostedDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Expiry Date</b></td>
<td>{entity.ExpiryDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Description</b></td>
<td>{entity.Description}</td>
</tr>

</table>

<br/>

<p>Please login to the HRMS Portal to view the updated announcement.</p>

<br/>

Regards,<br/>
<b>HR Team</b>

</body>

</html>";

                List<string>? attachments = null;

                if (!string.IsNullOrWhiteSpace(entity.AttachmentPath))
                {
                    string physicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.AttachmentPath.TrimStart('/')
                            .Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(physicalPath))
                    {
                        attachments = new List<string>
            {
                physicalPath
            };
                    }
                }

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body,
                            null,
                            attachments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send mail to {email}: {ex.Message}");
                    }
                }
            }
            return MapNewsToDto(entity);
        }

        /// <summary>
        /// Delete news based on UserId
        /// </summary>
        public async Task<bool> DeleteNewsAsync(int id, int userId)
        {
            var entity = await _unitOfWork
                .Repository<CompanyNewsMaster>()
                .GetByIdAsync(id);

            if (entity == null || entity.UserId != userId)
                return false;

            // 🔥 STEP 1: delete child table records first
            var deptRepo = _unitOfWork.Repository<CompanyNewsDepartment>();

            var childRecords = await deptRepo
                .FindAsync(x => x.NewsId == id);

            if (childRecords != null && childRecords.Any())
            {
                deptRepo.RemoveRange(childRecords);
            }

            // 🔥 STEP 2: delete parent
            _unitOfWork.Repository<CompanyNewsMaster>().Remove(entity);

            await _unitOfWork.CompleteAsync();

            return true;
        }


        // =========================================================
        // ====================== COMPANY POLICY ===================
        // =========================================================

        public async Task<IEnumerable<CompanyPolicyMasterDto>> GetAllPoliciesAsync(int userId)
        {
            var policies = await _unitOfWork
                .Repository<CompanyPoliciesMaster>()
                .GetAllAsync();

            var mappings = await _unitOfWork
                .Repository<CompanyPolicyDepartment>()
                .GetAllAsync();

            return policies
                .Where(x => x.UserId == userId)   // ✅ User filter
                .OrderByDescending(x => x.PolicyId)
                .Select(x => new CompanyPolicyMasterDto
                {
                    PolicyId = x.PolicyId,
                    CompanyId = x.CompanyId,
                    RegionId = x.RegionId,
                    UserId = x.UserId,

                    PolicyTitle = x.PolicyTitle,

                    DepartmentIds = mappings
                        .Where(m => m.PolicyId == x.PolicyId)
                        .Select(m => m.DepartmentId)
                        .ToList(),

                    Category = x.Category,
                    EffectiveDate = x.EffectiveDate,
                    PolicyDescription = x.PolicyDescription,
                    DepartmentId = x.DepartmentId,

                    AttachmentName = x.AttachmentName,
                    AttachmentPath = x.AttachmentPath
                })
                .ToList();
        }

        //    public async Task<IEnumerable<CompanyPolicyMasterDto>> GetTodayPoliciesAsync(int userId)
        //    {
        //        // Get all users
        //        var users = await _unitOfWork.Repository<User>().GetAllAsync();

        //        var user = users.FirstOrDefault(x => x.UserId == userId);

        //        if (user == null)
        //            return new List<CompanyPolicyMasterDto>();

        //        // Get DepartmentId from user
        //        var departmentId = user.DepartmentId;

        //        // Get today's date
        //        var today = DateOnly.FromDateTime(DateTime.Now);

        //        // Get all policies
        //        var policies = await _unitOfWork.Repository<CompanyPoliciesMaster>().GetAllAsync();

        //        // Filter policies
        //        //return policies
        //        //    .Where(x =>
        //        //        x.DepartmentId == departmentId &&
        //        //        x.IsActive == true &&
        //        //        x.PostedDate.HasValue &&
        //        //        x.PostedDate.Value == today)
        //        //    .Select(MapPolicyToDto)
        //        //    .ToList();


        //        var mappings = await _unitOfWork
        //.Repository<CompanyPolicyDepartment>()
        //.GetAllAsync();

        //        var policyIds = mappings
        //            .Where(x => x.DepartmentId == departmentId)
        //            .Select(x => x.PolicyId)
        //            .Distinct()
        //            .ToList();

        //        return policies
        //            .Where(x =>
        //                policyIds.Contains(x.PolicyId) &&
        //                x.IsActive == true &&
        //                x.PostedDate.HasValue &&
        //                x.PostedDate.Value == today)
        //            .Select(MapPolicyToDto)
        //            .ToList();
        //    }


        public async Task<IEnumerable<CompanyPolicyMasterDto>> GetTodayPoliciesAsync(int companyId, int regionId, int UserId)
        {
            var users = await _unitOfWork.Repository<User>().GetAllAsync();

            var currentUser = users.FirstOrDefault(x =>
                x.UserId == UserId &&
                x.CompanyId == companyId &&
                x.RegionId == regionId);

            if (currentUser == null)
                return new List<CompanyPolicyMasterDto>();


            // Logged-in User Department
            var departmentId = currentUser.DepartmentId;

            if (departmentId == null)
                return new List<CompanyPolicyMasterDto>();


            // Get Policy Master Data
            var policies = await _unitOfWork
                .Repository<CompanyPoliciesMaster>()
                .GetAllAsync();


            // Get Policy Department Mapping
            var policyDepartments = await _unitOfWork
                .Repository<CompanyPolicyDepartment>()
                .GetAllAsync();



            // Get PolicyIds based on User Department
            var policyIds = policyDepartments
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.PolicyId)
                .Distinct()
                .ToList();



            // Get Active Policies
            var filteredPolicies = policies
                .Where(x =>
                    policyIds.Contains(x.PolicyId) &&
                    x.CompanyId == companyId &&
                    x.RegionId == regionId &&
                    x.IsActive == true
                )
                .ToList();



            // Map Response
            var result = filteredPolicies.Select(policy => new CompanyPolicyMasterDto
            {
                PolicyId = policy.PolicyId,

                PolicyTitle = policy.PolicyTitle,

                PolicyDescription = policy.PolicyDescription,

                PostedDate = policy.PostedDate,

                EffectiveDate = policy.EffectiveDate,

                ExpiryDate = policy.ExpiryDate,

                IsActive = policy.IsActive,

                UserId = policy.UserId,

                CompanyId = policy.CompanyId,

                RegionId = policy.RegionId,

                CreatedBy = policy.CreatedBy,

                UpdatedBy = policy.UpdatedBy,

                CreatedAt = policy.CreatedAt,

                UpdatedAt = policy.UpdatedAt,

                Category = policy.Category,

                FromDate = policy.FromDate,

                ToDate = policy.ToDate,

                AttachmentName = policy.AttachmentName,

                AttachmentPath = policy.AttachmentPath,


                // Get Departments from Mapping Table Only
                DepartmentIds = policyDepartments
                    .Where(d => d.PolicyId == policy.PolicyId)
                    .Select(d => d.DepartmentId)
                    .ToList()

            }).ToList();


            return result;
        }

        public async Task<CompanyPolicyMasterDto?> GetPolicyByIdAsync(int id, int userId)
        {
            var entity = await _unitOfWork.Repository<CompanyPoliciesMaster>().GetByIdAsync(id);

            if (entity == null || entity.UserId != userId)
                return null;

            return MapPolicyToDto(entity);
        }

        //public async Task<CompanyPolicyMasterDto> AddPolicyAsync(CompanyPolicyMasterDto dto)
        //{

        //    var deptIds = dto.DepartmentIds;


        //    var entity = new CompanyPoliciesMaster
        //    {
        //        PolicyTitle = dto.PolicyTitle,
        //        PolicyDescription = dto.PolicyDescription,
        //        PostedDate = dto.PostedDate,
        //        EffectiveDate = dto.EffectiveDate,
        //        ExpiryDate = dto.ExpiryDate,
        //        DepartmentId = null,
        //        AttachmentName = dto.AttachmentName,
        //        AttachmentPath = dto.AttachmentPath,
        //        Category = dto.Category,
        //        IsActive = dto.IsActive,
        //        UserId = dto.UserId,
        //        CompanyId = dto.CompanyId,
        //        RegionId = dto.RegionId,
        //        CreatedBy = dto.CreatedBy,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _unitOfWork.Repository<CompanyPoliciesMaster>().AddAsync(entity);
        //    await _unitOfWork.CompleteAsync();
        //    var policyId = entity.PolicyId;
        //    if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
        //    {
        //        foreach (var deptId in dto.DepartmentIds)
        //        {
        //            await _unitOfWork.Repository<CompanyPolicyDepartment>()
        //                .AddAsync(new CompanyPolicyDepartment
        //                {
        //                    PolicyId = entity.PolicyId,
        //                    DepartmentId = deptId,
        //                    CreatedDate = DateTime.Now
        //                });
        //        }

        //        await _unitOfWork.CompleteAsync();
        //    }

        //    await _unitOfWork.CompleteAsync();

        //    return MapPolicyToDto(entity);
        //}

        public async Task<CompanyPolicyMasterDto> AddPolicyAsync(CompanyPolicyMasterDto dto)
        {
            string? fileName = null;
            string? filePath = null;

            //=============================
            // Upload Attachment
            //=============================

            if (dto.Attachment != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "CompanyPolicies"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                fileName = Guid.NewGuid().ToString() +
                           Path.GetExtension(dto.AttachmentName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                filePath = "/CompanyPolicies/" + fileName;
            }

            var entity = new CompanyPoliciesMaster
            {
                PolicyTitle = dto.PolicyTitle,
                PolicyDescription = dto.PolicyDescription,
                PostedDate = dto.PostedDate,
                EffectiveDate = dto.EffectiveDate,
                ExpiryDate = dto.ExpiryDate,
                DepartmentId = null,
                AttachmentName = dto.AttachmentName,
                AttachmentPath = dto.AttachmentPath,
                Category = dto.Category,
                IsActive = dto.IsActive,
                UserId = dto.UserId,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,

            };

            //=========================================
            // Save Policy
            //=========================================

            await _unitOfWork.Repository<CompanyPoliciesMaster>()
                .AddAsync(entity);

            await _unitOfWork.CompleteAsync();

            //=========================================
            // Save Departments
            //=========================================

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    await _unitOfWork.Repository<CompanyPolicyDepartment>()
                        .AddAsync(new CompanyPolicyDepartment
                        {
                            PolicyId = entity.PolicyId,
                            DepartmentId = deptId,
                            CreatedDate = DateTime.Now
                        });
                }

                await _unitOfWork.CompleteAsync();
            }

            //=========================================
            // Get Employee Emails
            //=========================================

            List<string> emails = new();

            if (dto.DepartmentIds == null || !dto.DepartmentIds.Any())
            {
                // Send to all active employees of Company & Region
                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                // Send only to selected departments
                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=========================================
            // Send Email
            //=========================================

            if (emails.Any())
            {
                string subject = $"New Company Policy - {dto.PolicyTitle}";

                string body = $@"
<html>

<body style='font-family:Calibri'>

<h2 style='color:#0d6efd;'>Company Policy Notification</h2>

<p>Dear Employee,</p>

<p>A new company policy has been published.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
<td><b>Policy Title</b></td>
<td>{dto.PolicyTitle}</td>
</tr>

<tr>
<td><b>Category</b></td>
<td>{dto.Category}</td>
</tr>

<tr>
<td><b>Posted Date</b></td>
<td>{dto.PostedDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Effective Date</b></td>
<td>{dto.EffectiveDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Expiry Date</b></td>
<td>{dto.ExpiryDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Description</b></td>
<td>{dto.PolicyDescription}</td>
</tr>

</table>

<br/>

<p>Please login to the HRMS Portal to view the complete policy.</p>

<br/>

Regards,<br/>

<b>HR Team</b>

</body>

</html>";

                List<string>? attachments = null;

                if (!string.IsNullOrWhiteSpace(dto.AttachmentPath))
                {
                    string physicalPath = dto.AttachmentPath;
                    //string physicalPath = Path.Combine(
                    //    Directory.GetCurrentDirectory(),
                    //    "wwwroot",
                    //    filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(physicalPath))
                    {
                        attachments = new List<string>
                {
                    physicalPath
                };
                    }
                }

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body,
                            null,
                            attachments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send mail to {email}: {ex.Message}");
                    }
                }
            }

            return MapPolicyToDto(entity);
        }

        // public async Task<CompanyPolicyMasterDto> UpdatePolicyAsync(int id, CompanyPolicyMasterDto dto)
        // {
        //     var entity = await _unitOfWork.Repository<CompanyPoliciesMaster>().GetByIdAsync(id);

        //     if (entity == null)
        //         throw new Exception("Policy not found");

        //     entity.CompanyId = dto.CompanyId;
        //     entity.RegionId = dto.RegionId;
        //     entity.PolicyTitle = dto.PolicyTitle;
        //     entity.PolicyDescription = dto.PolicyDescription;
        //     entity.PostedDate = dto.PostedDate;
        //     entity.EffectiveDate = dto.EffectiveDate;
        //     entity.AttachmentPath = dto.AttachmentPath;
        //     entity.Category = dto.Category;
        //     entity.AttachmentName = dto.AttachmentName;
        //     entity.DepartmentId = null;
        //     entity.ExpiryDate = dto.ExpiryDate;
        //     entity.IsActive = dto.IsActive;
        //     entity.UpdatedBy = dto.UpdatedBy;
        //     entity.UpdatedAt = DateTime.UtcNow;

        //     _unitOfWork.Repository<CompanyPoliciesMaster>().Update(entity);
        //     await _unitOfWork.CompleteAsync();

        //     var existingMappings =
        //(await _unitOfWork.Repository<CompanyPolicyDepartment>().GetAllAsync())
        //.Where(x => x.PolicyId == id)
        //.ToList();

        //     foreach (var item in existingMappings)
        //     {
        //         _unitOfWork.Repository<CompanyPolicyDepartment>().Remove(item);
        //     }

        //     await _unitOfWork.CompleteAsync();

        //     // Add new mappings
        //     if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
        //     {
        //         foreach (var deptId in dto.DepartmentIds)
        //         {
        //             await _unitOfWork.Repository<CompanyPolicyDepartment>()
        //                 .AddAsync(new CompanyPolicyDepartment
        //                 {
        //                     PolicyId = id,
        //                     DepartmentId = deptId,
        //                     CreatedDate = DateTime.Now
        //                 });
        //         }

        //         await _unitOfWork.CompleteAsync();
        //     }


        //     return MapPolicyToDto(entity);
        // }

        public async Task<CompanyPolicyMasterDto> UpdatePolicyAsync(
     int id,
     CompanyPolicyMasterDto dto)
        {
            var entity = await _unitOfWork
                .Repository<CompanyPoliciesMaster>()
                .GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Policy not found");

            //=========================================
            // Update Basic Fields
            //=========================================

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.PolicyTitle = dto.PolicyTitle;
            entity.PolicyDescription = dto.PolicyDescription;
            entity.PostedDate = dto.PostedDate;
            entity.EffectiveDate = dto.EffectiveDate;
            entity.ExpiryDate = dto.ExpiryDate;
            entity.Category = dto.Category;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = dto.UpdatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            //=========================================
            // Upload New File
            //=========================================

            if (dto.Attachment != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "policy");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString()
                               + Path.GetExtension(dto.Attachment.FileName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                // Delete old file
                if (!string.IsNullOrEmpty(entity.AttachmentPath))
                {
                    var oldFile = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.AttachmentPath.TrimStart('/'));

                    if (File.Exists(oldFile))
                    {
                        File.Delete(oldFile);
                    }
                }

                entity.AttachmentName = dto.Attachment.FileName;
                entity.AttachmentPath = "/policy/" + fileName;
            }

            _unitOfWork
                .Repository<CompanyPoliciesMaster>()
                .Update(entity);

            await _unitOfWork.CompleteAsync();

            //=========================================
            // Delete Existing Department Mapping
            //=========================================

            var existingMappings =
                (await _unitOfWork
                    .Repository<CompanyPolicyDepartment>()
                    .GetAllAsync())
                .Where(x => x.PolicyId == entity.PolicyId)
                .ToList();

            foreach (var item in existingMappings)
            {
                _unitOfWork
                    .Repository<CompanyPolicyDepartment>()
                    .Remove(item);
            }

            await _unitOfWork.CompleteAsync();

            //=========================================
            // Add New Department Mapping
            //=========================================

            if (dto.DepartmentIds != null && dto.DepartmentIds.Any())
            {
                foreach (var deptId in dto.DepartmentIds)
                {
                    await _unitOfWork
                        .Repository<CompanyPolicyDepartment>()
                        .AddAsync(new CompanyPolicyDepartment
                        {
                            PolicyId = entity.PolicyId,
                            DepartmentId = deptId,
                            CreatedDate = DateTime.UtcNow
                        });
                }

                await _unitOfWork.CompleteAsync();
            }

            //=========================================
            // Get User Emails
            //=========================================

            List<string> emails = new();

            if (dto.DepartmentIds == null || !dto.DepartmentIds.Any())
            {
                // All Employees

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }
            else
            {
                // Selected Departments

                emails = await _context.Users
                    .Where(x =>
                        x.CompanyId == dto.CompanyId &&
                        x.RegionId == dto.RegionId &&
                        dto.DepartmentIds.Contains((int)x.DepartmentId) &&
                        x.Status == "Active" &&
                        !string.IsNullOrEmpty(x.Email))
                    .Select(x => x.Email)
                    .Distinct()
                    .ToListAsync();
            }

            //=========================================
            // Send Email
            //=========================================

            if (emails.Any())
            {
                string subject = $"Updated Company Policy - {entity.PolicyTitle}";

                string body = $@"
<html>

<body style='font-family:Calibri'>

<h2 style='color:#0d6efd;'>Company Policy Updated</h2>

<p>Dear Employee,</p>

<p>An existing company policy has been updated.</p>

<table border='1'
       cellpadding='8'
       cellspacing='0'
       style='border-collapse:collapse;'>

<tr>
<td><b>Policy Title</b></td>
<td>{entity.PolicyTitle}</td>
</tr>

<tr>
<td><b>Category</b></td>
<td>{entity.Category}</td>
</tr>

<tr>
<td><b>Posted Date</b></td>
<td>{entity.PostedDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Effective Date</b></td>
<td>{entity.EffectiveDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Expiry Date</b></td>
<td>{entity.ExpiryDate:dd-MMM-yyyy}</td>
</tr>

<tr>
<td><b>Description</b></td>
<td>{entity.PolicyDescription}</td>
</tr>

</table>

<br/>

<p>Please find the updated policy document attached for your reference.</p>

<br/>

Regards,<br/>
<b>HR Team</b>

</body>

</html>";

                List<string>? attachments = null;

                if (!string.IsNullOrWhiteSpace(entity.AttachmentPath))
                {
                    string physicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        entity.AttachmentPath.TrimStart('/')
                            .Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(physicalPath))
                    {
                        attachments = new List<string>
                {
                    physicalPath
                };
                    }
                }

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            subject,
                            body,
                            null,
                            attachments);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send mail to {email}: {ex.Message}");
                    }
                }
            }

            return MapPolicyToDto(entity);
        }
        
        
        //public async Task<bool> DeletePolicyAsync(int id, int userId)
        //{
        //    var entity = await _unitOfWork.Repository<CompanyPoliciesMaster>()
        //        .GetByIdAsync(id);

        //    if (entity == null || entity.UserId != userId)
        //        return false;

        //    // Delete Mapping Records
        //    var mappings = (await _unitOfWork.Repository<CompanyPolicyDepartment>()
        //        .GetAllAsync())
        //        .Where(x => x.PolicyId == id)
        //        .ToList();

        //    foreach (var item in mappings)
        //    {
        //        _unitOfWork.Repository<CompanyPolicyDepartment>().Remove(item);
        //    }

        //    // Delete Main Policy Record
        //    _unitOfWork.Repository<CompanyPoliciesMaster>().Remove(entity);

        //    await _unitOfWork.CompleteAsync();

        //    return true;
        //}


        public async Task<bool> DeletePolicyAsync(int id, int userId)
        {
            var entity = await _unitOfWork.Repository<CompanyPoliciesMaster>()
                .GetByIdAsync(id);

            if (entity == null)
                return false;

            // Delete Mapping Records
            var mappings = (await _unitOfWork.Repository<CompanyPolicyDepartment>()
                .GetAllAsync())
                .Where(x => x.PolicyId == id)
                .ToList();

            foreach (var item in mappings)
            {
                _unitOfWork.Repository<CompanyPolicyDepartment>().Remove(item);
            }

            // Delete Main Policy Record
            _unitOfWork.Repository<CompanyPoliciesMaster>().Remove(entity);

            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ================= MAPPERS =================

        private CompanyNewsMasterDto MapNewsToDto(CompanyNewsMaster entity)
        {
            return new CompanyNewsMasterDto
            {
                NewsId = entity.NewsId,

                Title = entity.Title,

                Description = entity.Description,

                PostedDate = entity.PostedDate,

                ExpiryDate = entity.ExpiryDate,

                IsActive = entity.IsActive,

                UserId = entity.UserId,

                CompanyId = entity.CompanyId,

                RegionId = entity.RegionId,

                CreatedBy = entity.CreatedBy,

                UpdatedBy = entity.UpdatedBy,

                CreatedAt = entity.CreatedAt,

                UpdatedAt = entity.UpdatedAt,

                Category = entity.Category,


                // ✅ IMPORTANT
                AttachmentName = entity.AttachmentName,

                // ✅ IMPORTANT
                AttachmentPath = entity.AttachmentPath
            };
        }

        private CompanyPolicyMasterDto MapPolicyToDto(CompanyPoliciesMaster x)
        {
            return new CompanyPolicyMasterDto
            {
                PolicyId = x.PolicyId,
                PolicyTitle = x.PolicyTitle,
                PolicyDescription = x.PolicyDescription,
                PostedDate = x.PostedDate,
                EffectiveDate = x.EffectiveDate,
                ExpiryDate = x.ExpiryDate,
                IsActive = x.IsActive,
                DepartmentId = x.DepartmentId,
                Category = x.Category,
                UserId = x.UserId,
                CompanyId = x.CompanyId,
                RegionId = x.RegionId,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                AttachmentName = x.AttachmentName,
                AttachmentPath = x.AttachmentPath,
                DepartmentIds = x.CompanyPolicyDepartments
                .Select(d => d.DepartmentId)
                .ToList()
            };
        }
    }
}
