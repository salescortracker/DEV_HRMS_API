using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;



namespace BusinessLayer.Implementations
{
    public class UserService : IUserService
    {
        private readonly DataAccessLayer.DBContext.HRMSContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;

        public UserService(DataAccessLayer.DBContext.HRMSContext context, IConfiguration configuration, INotificationService notificationService, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;
            _emailService = emailService;
        }
        public async Task<AdminDashboardCountDto> GetAdminDashboardCountAsync(int userId)
        {

            // Get Companies created by Admin
            var companyIds = await _context.Companies
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive == true)
                .Select(x => x.CompanyId)
                .ToListAsync();



            // Get Regions created by Admin
            var regionIds = await _context.Regions
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive == true)
                .Select(x => x.RegionId)
                .ToListAsync();



            // Get Employees under those companies and regions
            var employeeCount = await _context.Users
                .Where(x =>
                    x.Status == "Active" &&
                    companyIds.Contains(x.CompanyId) &&
                    regionIds.Contains(x.RegionId))
                .CountAsync();



            return new AdminDashboardCountDto
            {
                TotalCompanies = companyIds.Count,

                TotalRegions = regionIds.Count,

                TotalEmployees = employeeCount
            };
        }
        public async Task<IEnumerable<DataAccessLayer.DBContext.User>> GetAllUsersAsync(int userCompanyId)
        {
            return await _context.Users.Where(x=>x.UserCompanyId==userCompanyId).ToListAsync();
        }
        public async Task<IEnumerable<DataAccessLayer.DBContext.User>> GetcmpregAllUsersAsync(int CompanyId,int regionId)
        {
            return await _context.Users.Where(x => x.CompanyId == CompanyId && x.RegionId==regionId).ToListAsync();
        }
        public async Task<IEnumerable<MaritalStatus>> GetAllMaritalStatusByCmp(int CompanyId, int regionId)
        {
            return await _context.MaritalStatuses.Where(x => x.CompanyId == CompanyId && x.RegionId == regionId).ToListAsync();
        }

        public async Task<DataAccessLayer.DBContext.User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<DataAccessLayer.DBContext.User> CreateUserAsync(UserCreateDto userDto)
        {
            try
            {
               
                
                if (userDto == null)
                    throw new ArgumentNullException(nameof(userDto));

                var existingUseradmin = _context.Users
                    .Where(u =>
                        u.Email == userDto.Email 
                    );
                if (existingUseradmin.Count() > 0)
                {
                    return null;
                }             

                var existingUser =  _context.Users
                    .Where(u =>
                        u.Email == userDto.Email &&
                        u.CompanyId == userDto.CompanyID &&
                        u.RegionId == userDto.RegionID
                    );

                if (existingUser.Count()>0)
                {
                    throw new Exception("This email already exists in the selected Company and Region.");
                }

              
                if (string.IsNullOrWhiteSpace(userDto.EmployeeCode))
                {
                    var employeeCodes = await _context.Users
                        .Where(x =>
                            x.CompanyId == userDto.CompanyID &&
                            x.RegionId == userDto.RegionID)
                        .Select(x => x.EmployeeCode)
                        .ToListAsync();

                    int maxNumber = employeeCodes
                        .Select(c =>
                        {
                            var num = new string((c ?? "").Where(char.IsDigit).ToArray());
                            return int.TryParse(num, out int n) ? n : 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();

                    userDto.EmployeeCode = $"EMP{(maxNumber + 1):D4}";
                }
                string newEmployeeCode = userDto.EmployeeCode;


                // ✅ Hash Password
                string hashedPassword = HashPassword(userDto.Password);
             var userdemo=   await _context.Users.FirstOrDefaultAsync(u => u.UserId == userDto.UserCompanyId);
                DataAccessLayer.DBContext.User user=null;
                if (userDto.loginType == "Admin")
                {
                    user = new DataAccessLayer.DBContext.User
                    {
                        FullName = userDto.FullName,
                        Email = userDto.Email,
                        PhoneNumber = null,// userDto.Phone,
                        CompanyName = null,// userDto.Company,
                        //Module = dto.Module,
                        Type = "Demo",
                        CompanyId = 1,
                        RegionId = 2,
                        EmployeeCode = newEmployeeCode,
                        RoleId = 1,
                        PasswordHash = "Demo@123", // In real scenarios, hash the password properly
                        DemoStartDate = DateTime.UtcNow,
                        DemoExpiryDate = userdemo != null ? userdemo.DemoExpiryDate : null,
                        CreatedDate = DateTime.Now,
                        UserCompanyId = userDto.UserCompanyId,
                        LoginType = userDto.loginType,
                        DesignationId = userDto.DesignationId,
                        ReportingHr = userDto.ReportingHR,
                        JoiningDate = userDto.JoiningDate

                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // ✅ Send Welcome Email
                    await SendWelcomeEmailAsync(
                       user, userDto.Password
                    );
                }
                else
                {

                    // ✅ Create User Entity
                    user = new DataAccessLayer.DBContext.User
                    {
                        CompanyId = userDto.CompanyID,
                        RegionId = userDto.RegionID,
                        EmployeeCode = newEmployeeCode,
                        FullName = userDto.FullName,
                        Email = userDto.Email,
                        PasswordHash = userDto.Password,
                        ReportingTo = userDto.reportingTo,
                        DepartmentId = userDto.departmentId,
                        RoleId = userDto.RoleId,
                        Status = "Active",
                        CreatedDate = DateTime.UtcNow,
                        UserCompanyId = userDto.UserCompanyId
                        ,
                        DemoStartDate = DateTime.UtcNow,
                        DemoExpiryDate = userdemo != null ? userdemo.DemoExpiryDate : null,
                        LoginType = userDto.loginType,
                        DesignationId = userDto.DesignationId,
                        ReportingHr = userDto.ReportingHR,
                        JoiningDate = userDto.JoiningDate
                    };



                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    var ccEmails = await _context.Users
                        .Where(u =>
                            u.UserId == userDto.ReportingHR ||
                            u.UserId == userDto.reportingTo
                        )
                        .Select(u => u.Email)
                        .Where(e => !string.IsNullOrWhiteSpace(e))
                        .Distinct()
                        .ToListAsync();

                    // ✅ Send Welcome Email
                    await SendWelcomeEmailAsync(
                       user, 
                       userDto.Password,
                       ccEmails
                    );

                }
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DataAccessLayer.DBContext.User>> GetDemoUsers()
        {
            return await _context.Users
                .Where(u => u.Type == "demo")
                .ToListAsync();
        }

        public async Task<List<UserSubscriptionDto>> GetALLSubcriptionUsers()
{
    var subscriptions = await _context.UserSubscriptions
        .Select(x => new UserSubscriptionDto
        {
            SubscriptionId = x.SubscriptionId,
            UserId = x.UserId,
            PlanId = x.PlanId,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Status = x.Status
        })
        .ToListAsync();

    return subscriptions;
}
        public async Task<LoginResponseDto?> VerifyLoginAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == username && u.PasswordHash == password);
                if (user == null)
                {
                    return new LoginResponseDto
                    {
                        Error = "Invalid username or password"
                    };
                }
                var sessionId = Guid.NewGuid();

                user.LoginSessionId = sessionId;
                // Browser Session
                var browserSessionId = Guid.NewGuid();

                // Remove previous browser session
                var existingBrowserSessions = await _context.ActiveBrowserSessions.ToListAsync();

                if (existingBrowserSessions.Any())
                {
                    _context.ActiveBrowserSessions.Add(new ActiveBrowserSession
                    {
                        BrowserSessionId = browserSessionId,
                        UserId = user.UserId,
                        CreatedDate = DateTime.Now
                    });
                }

                // Insert new browser session
                _context.ActiveBrowserSessions.Add(new ActiveBrowserSession
                {
                    BrowserSessionId = browserSessionId,
                    UserId = user.UserId,
                    CreatedDate = DateTime.Now
                });

                // Prevent inactive users from logging in
                if (user.Status != "Active")
                {
                    return new LoginResponseDto
                    {
                        Error = "ACCOUNT_INACTIVE",
                        Message = "Your account is inactive. Please contact the administrator."
                    };
                }
                if (user.RoleId == 0)
                {
                    var superAdminData = await _context.Users
                        .Where(u => u.UserId == user.UserId)
                        .Select(u => new
                        {
                            u.UserId,
                            u.Email,
                            u.FullName,
                            u.RoleId,
                            u.CompanyId,
                            u.RegionId,
                            u.EmployeeCode,
                            u.Userloginstatus,
                            u.Passwordchanged
                        })
                        .FirstOrDefaultAsync();

                    user.Userloginstatus = true;
                    await _context.SaveChangesAsync();

                    return new LoginResponseDto
                    {
                        User = superAdminData,
                        AllowedModules = new List<object>(),
                        SessionId = sessionId,
                        BrowserSessionId = browserSessionId
                    };
                }
                int subscriptionUserId = user.UserId;

                if (user.UserCompanyId.HasValue && user.UserCompanyId.Value > 0)
                {
                    subscriptionUserId = user.UserCompanyId.Value;
                }

                var subscription = await _context.UserSubscriptions
                .Include(x => x.Plan)
                .Where(x => x.UserId == subscriptionUserId)
                .OrderByDescending(x => x.SubscriptionId)
                .FirstOrDefaultAsync();

                if (subscription == null)
                    return new LoginResponseDto { UserId = user.UserId, Error = "NO_SUBSCRIPTION", Message = "No active subscription found" };

                if (subscription.EndDate < DateTime.UtcNow.Date)
                    return new LoginResponseDto { UserId = user.UserId, Error = "SUBSCRIPTION_EXPIRED", Message = "Please renew your plan" };

                if (subscription.Plan != null && subscription.Plan.Status != true)
                    return new LoginResponseDto { UserId = user.UserId, Error = "PLAN_DISABLED", Message = "Plan disabled" };

                var allowedModules = await _context.SubscriptionPlanModules
               .Where(x => x.PlanId == subscription.PlanId
                        && x.IsAllowed == true)
               .Select(x => new
               {
                   x.ModuleId,
                   ModuleName = x.Module.ModuleName
               })
               .ToListAsync();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Username or password cannot be empty.");



                if (user.RoleId == 0)
                {
                    var roledata = await (
                        from u in _context.Users
                            // join r in _context.RoleMasters on u.RoleId equals r.RoleId
                        join reg in _context.Regions on u.RegionId equals reg.RegionId
                        join c in _context.Companies on u.CompanyId equals c.CompanyId
                        join d in _context.Departments on u.DepartmentId equals d.DepartmentId into deptJoin
                        from d in deptJoin.DefaultIfEmpty()
                        join des in _context.Designations
                            on u.DesignationId equals des.DesignationId into desJoin
                        from des in desJoin.DefaultIfEmpty()


                        join rm in _context.Users on u.ReportingTo equals rm.UserId into managerJoin
                        from rm in managerJoin.DefaultIfEmpty()

                        where u.Email == username && u.PasswordHash == password

                        select new
                        {
                            u.UserId,
                            u.Email,
                            u.FullName,

                            // RoleName = r.RoleName,
                            RegionName = reg.RegionName,
                            CompanyName = c.CompanyName,

                            roleId = u.RoleId,
                            companyId = u.CompanyId,
                            regionId = u.RegionId,
                            employeeCode = u.EmployeeCode,

                            DepartmentId = u.DepartmentId,
                            DepartmentName = d.DepartmentName, // 🔥 STRING

                            ReportingManagerId = u.ReportingTo,
                            ReportingManagerName = rm.FullName, // 🔥 STRING

                            DesignationId = u.DesignationId,
                            DesignationName = des.DesignationName,

                            personalEmail = u.Email,
                            userLoginStatus = u.Userloginstatus,
                            paswordChanged = u.Passwordchanged,
                            userCompanyId = u.UserCompanyId
                        })
                     .FirstOrDefaultAsync();
                    user.Userloginstatus = true;
                    user.LoginSessionId = sessionId;
                    await _context.SaveChangesAsync();

                    return new LoginResponseDto
                    {
                        User = roledata,
                        AllowedModules = allowedModules.Cast<object>().ToList(),
                        SessionId = sessionId,
                        BrowserSessionId = browserSessionId
                    };

                }
                else
                {
                    var userData = await (
                    from u in _context.Users

                    join r in _context.RoleMasters
                        on u.RoleId equals r.RoleId

                    join d in _context.Departments
                        on u.DepartmentId equals d.DepartmentId into deptJoin
                    from d in deptJoin.DefaultIfEmpty()

                    join des in _context.Designations
                        on u.DesignationId equals des.DesignationId into desJoin
                    from des in desJoin.DefaultIfEmpty()

                    join rm in _context.Users
                        on u.ReportingTo equals rm.UserId into managerJoin
                    from rm in managerJoin.DefaultIfEmpty()

                    where u.Email == username && u.PasswordHash == password

                    select new
                    {
                        u.UserId,
                        u.Email,
                        u.FullName,

                        RoleName = r.RoleName,

                        roleId = u.RoleId,
                        companyId = u.CompanyId,
                        regionId = u.RegionId,
                        employeeCode = u.EmployeeCode,

                        // Department
                        DepartmentId = u.DepartmentId,
                        DepartmentName = d != null ? d.DepartmentName : "",

                        // Designation
                        DesignationId = u.DesignationId,
                        DesignationName = des != null ? des.DesignationName : "",

                        // Reporting Manager
                        ReportingManagerId = u.ReportingTo,
                        ReportingManagerName = rm != null ? rm.FullName : "",


                        personalEmail = u.Email,
                        userLoginStatus = u.Userloginstatus,
                        paswordChanged = u.Passwordchanged,
                        userCompanyId = u.UserCompanyId
                    })
                    .FirstOrDefaultAsync();
                    user.Userloginstatus = true;
                        user.LoginSessionId = sessionId;
                        await _context.SaveChangesAsync();

                        return new LoginResponseDto
                        {
                            User = userData,
                            AllowedModules = allowedModules.Cast<object>().ToList(),
                            SessionId = sessionId,
                            BrowserSessionId = browserSessionId
                        };

                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Input error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying login: {ex.Message}");
                return null;
            }
        }
        //public async Task<List<UserSubscriptionDto>> GetALLSubcriptionUsers()
        //{
        //    var users = await _context.UserSubscriptions
        //        .Include(x => x.User)
        //        .Include(x => x.SubscriptionPlan)
        //        .Select(x => new UserSubscriptionDto
        //        {
        //            SubscriptionId = x.SubscriptionId,
        //            UserId = x.UserId,
        //            UserName = x.User.FirstName + " " + x.User.LastName,
        //            PlanName = x.SubscriptionPlan.PlanName,
        //            StartDate = x.StartDate,
        //            EndDate = x.EndDate,
        //            Status = x.Status,
        //            IsActive = x.IsActive
        //        })
        //        .ToListAsync();

        //    return users;
        //}
        public async Task<ApiResponse<bool>> ChangePasswordAsync(PasswordChangeDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

                if (user == null)
                    return new ApiResponse<bool>(false, "User not found", false);

                // Old password check only if NOT first login
                if (!user.Userloginstatus!=null? !user.Userloginstatus.Value:false)
                {
                    string oldHash = HashPassword(dto.OldPassword);
                    if (user.PasswordHash != oldHash)
                        return new ApiResponse<bool>(false, "Old password is incorrect", false);
                }
                string pattern =@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,13}$";

                if (!Regex.IsMatch(dto.NewPassword, pattern))
                {
                    return new ApiResponse<bool>(
                        false,
                        "Password must be 8-13 characters long and contain at least one uppercase letter, one lowercase letter, one number, one special character (@$!%*?&) and no spaces.",
                        false);
                }

                user.PasswordHash = dto.NewPassword;
                user.Userloginstatus = true;
                user.Passwordchanged = true;
                await _context.SaveChangesAsync();

                await SendPasswordChangedEmailAsync(user, user.PasswordHash);
                return new ApiResponse<bool>(true, "Password updated successfully", true);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            }



        // 🔹 Generate Employee Code (Emp0001, Emp0002, etc.)
        private async Task<string> GenerateNextEmployeeCodeAsync()
        {
            var lastUser = await _context.Users
                .OrderByDescending(u => u.UserId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastUser != null && !string.IsNullOrEmpty(lastUser.EmployeeCode))
            {
                string numberPart = new string(lastUser.EmployeeCode.SkipWhile(c => !char.IsDigit(c)).ToArray());
                if (int.TryParse(numberPart, out int lastNumber))
                    nextNumber = lastNumber + 1;
            }

            return $"EMP{nextNumber:D4}";
        }

        // 🔹 Simple SHA256 password hashing
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<DataAccessLayer.DBContext.User?> UpdateUserAsync(UserCreateDto updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(updatedUser.userId);
            if (existingUser == null) return null;
            var emailExists = await _context.Users
            .AnyAsync(u =>
                u.Email == updatedUser.Email &&
                u.CompanyId == updatedUser.CompanyID &&
                u.RegionId == updatedUser.RegionID &&
                u.UserId != updatedUser.userId
            );

            if (emailExists)
            {
                throw new Exception("This email is already assigned to another user.");
            }

            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            existingUser.RoleId = updatedUser.RoleId;
            existingUser.ReportingTo = updatedUser.reportingTo;
            existingUser.DepartmentId = updatedUser.departmentId;
            existingUser.PasswordHash = updatedUser.Password;
            existingUser.Status = updatedUser.Status;
            existingUser.LoginType = updatedUser.loginType;
            existingUser.DesignationId = updatedUser.DesignationId;
            existingUser.ReportingHr = updatedUser.ReportingHR;
            existingUser.JoiningDate = updatedUser.JoiningDate;

            await _context.SaveChangesAsync();
            return existingUser;

        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SendWelcomeEmailAsync(DataAccessLayer.DBContext.User user,string password, List<string>? ccEmails = null)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _configuration["Smtp:Host"];
                    smtpClient.Port = int.Parse(_configuration["Smtp:Port"]);
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["Smtp:User"],
                        _configuration["Smtp:Password"]
                    );

                    string logoUrl = "https://corhr.cortracker360.com/assets/images/cor-logo.png"; // Replace with your actual logo

                    string subject = "Welcome to HRMS – Your Login Details";

                    string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Welcome to HRMS</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#333;'>
    <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background-color:#f4f6f8;padding:40px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' width='600' style='background-color:#ffffff;border-radius:10px;overflow:hidden;box-shadow:0 2px 10px rgba(0,0,0,0.08);'>
                    <tr>
                        <td style='background-color:#004aad;padding:20px;text-align:center;'>
                            <img src='{logoUrl}' alt='Cortracker HRMS' style='height:60px;'/>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:40px 30px;'>
                            <h2 style='color:#004aad;margin-bottom:10px;'>Welcome to Cortracker HRMS, {user.FullName}!</h2>
                            <p style='font-size:16px;line-height:1.6;margin:20px 0;'>
                                We’re excited to have you onboard! Your HRMS account has been successfully created. Please find your login details below.
                            </p>

                            <table cellpadding='6' cellspacing='0' style='width:100%;margin:20px 0;border-collapse:collapse;'>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;width:120px;'>Login URL:</td>
                                    <td><a href='https://corhr.cortracker360.com' style='color:#004aad;text-decoration:none;'>https://corhr.cortracker360.com</a></td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Username:</td>
                                    <td>{user.Email}</td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Password:</td>
                                    <td>{password}</td>
                                </tr>
                            </table>

                            <p style='font-size:15px;line-height:1.6;'>
                                For your security, please update your password after your first login.
                            </p>

                            <div style='margin-top:30px;text-align:center;'>
                                <a href='https://corhr.cortracker360.com' 
                                   style='background-color:#004aad;color:#fff;padding:12px 24px;border-radius:6px;text-decoration:none;font-weight:600;'>
                                   Go to HRMS Portal
                                </a>
                            </div>

                            <p style='font-size:14px;color:#888;margin-top:30px;'>
                                Regards,<br/>
                                <strong>HR Team</strong><br/>
                                Cortracker HRMS
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='background-color:#f0f2f5;text-align:center;padding:15px;font-size:12px;color:#888;'>
                            © {DateTime.UtcNow.Year} Cortracker HRMS. All rights reserved.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";


                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_configuration["Smtp:FromEmail"], "Cortracker HRMS"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(user.Email);
                    if (ccEmails != null && ccEmails.Any())
                    {
                        foreach (var cc in ccEmails)
                        {
                            if (!string.IsNullOrWhiteSpace(cc))
                            {
                                mailMessage.CC.Add(cc);
                            }
                        }
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
        public async Task SendPasswordChangedEmailAsync(
    DataAccessLayer.DBContext.User user,
    string newPassword)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = _configuration["Smtp:Host"];
                    smtpClient.Port = int.Parse(_configuration["Smtp:Port"]);
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(
                        _configuration["Smtp:User"],
                        _configuration["Smtp:Password"]
                    );

                    string logoUrl = "http://mock-hr.cortracker360.com/assets/images/cor-logo.png";

                    string subject = "Your HRMS Password Has Been Updated";

                    string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Password Changed</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f8;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;color:#333;'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' width='600' cellpadding='0' cellspacing='0'
                       style='background:#fff;border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,0.08);overflow:hidden;'>

                    <!-- Header -->
                    <tr>
                        <td style='background:#004aad;padding:20px;text-align:center;'>
                            <img src='{logoUrl}' alt='Cortracker HRMS' style='height:60px;' />
                        </td>
                    </tr>

                    <!-- Content -->
                    <tr>
                        <td style='padding:40px 30px;'>
                            <h2 style='color:#004aad;margin-bottom:10px;'>
                                Password Changed Successfully
                            </h2>

                            <p style='font-size:16px;line-height:1.6;margin:20px 0;'>
                                Hi <strong>{user.FullName}</strong>,<br/><br/>
                                Your HRMS account password has been updated successfully.
                                Below are your updated login credentials.
                            </p>

                            <table cellpadding='6' cellspacing='0'
                                   style='width:100%;margin:20px 0;border-collapse:collapse;'>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;width:130px;'>
                                        Login URL:
                                    </td>
                                    <td>
                                        <a href='https://corhr.cortracker360.com'
                                           style='color:#004aad;text-decoration:none;'>
                                           https://corhr.cortracker360.com
                                        </a>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Username:</td>
                                    <td>{user.Email}</td>
                                </tr>
                                <tr>
                                    <td style='font-weight:bold;color:#004aad;'>Password:</td>
                                    <td>{newPassword}</td>
                                </tr>
                            </table>

                            <p style='font-size:15px;line-height:1.6;'>
                                If you did not initiate this change, please contact the HR
                                team immediately.
                            </p>

                            <div style='margin-top:30px;text-align:center;'>
                                <a href='https://corhr.cortracker360.com'
                                   style='background:#004aad;color:#fff;padding:12px 26px;
                                          border-radius:6px;text-decoration:none;font-weight:600;'>
                                   Login to HRMS
                                </a>
                            </div>

                            <p style='font-size:14px;color:#888;margin-top:30px;'>
                                Regards,<br/>
                                <strong>HR Team</strong><br/>
                                Cortracker HRMS
                            </p>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background:#f0f2f5;text-align:center;padding:15px;
                                   font-size:12px;color:#888;'>
                            © {DateTime.UtcNow.Year} Cortracker HRMS. All rights reserved.
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_configuration["Smtp:FromEmail"], "Cortracker HRMS"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(user.Email);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> SendOtpAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return new ApiResponse<bool>(false, "Invalid Email", false);

            var otp = new Random().Next(100000, 999999).ToString();

            user.RefreshToken = otp;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(5);

            await _context.SaveChangesAsync();

            await SendOtpEmailAsync(user, otp);

            return new ApiResponse<bool>(true, "OTP sent successfully", true);
        }

        public async Task<ApiResponse<bool>> VerifyOtpAsync(string email, string otp)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return new ApiResponse<bool>(false, "Invalid Email", false);

            if (user.RefreshToken != otp ||
                user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return new ApiResponse<bool>(false, "Invalid or Expired OTP", false);
            }

            return new ApiResponse<bool>(true, "OTP Verified", true);
        }
        public async Task<ApiResponse<bool>> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return new ApiResponse<bool>(false, "Invalid Email", false);
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,13}$";

            if (!Regex.IsMatch(newPassword, pattern))
            {
                return new ApiResponse<bool>(
                    false,
                    "Password must be 8-13 characters and contain at least one uppercase letter, one lowercase letter, one number, one special character (@$!%*?&) and no spaces.",
                    false);
            }


            user.PasswordHash = newPassword;
            user.Passwordchanged = true;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _context.SaveChangesAsync();

            await SendPasswordChangedEmailAsync(user, newPassword);

            return new ApiResponse<bool>(true, "Password reset successful", true);
        }

        private async Task SendOtpEmailAsync(DataAccessLayer.DBContext.User user, string otp)
        {
            string subject = "Your OTP for Password Reset";

            string body = $@"
        <h3>Hello {user.FullName},</h3>
        <p>Your OTP for password reset is:</p>
        <h2>{otp}</h2>
        <p>This OTP is valid for 5 minutes.</p>
        <br/>
        <p>Regards,<br/>HR Team</p>
    ";

            using var smtpClient = new SmtpClient
            {
                Host = _configuration["Smtp:Host"],
                Port = int.Parse(_configuration["Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["Smtp:User"],
                    _configuration["Smtp:Password"]
                )
            };

            var mail = new MailMessage(
                _configuration["Smtp:FromEmail"],
                user.Email,
                subject,
                body
            )
            { IsBodyHtml = true };

            await smtpClient.SendMailAsync(mail);
        }
        public async Task<bool> UpdateDemoExpiry(int userId, DateTime demoExpiryDate)
        {
            var user = await _context.Users
                        .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                return false;

            user.DemoExpiryDate = demoExpiryDate;
            user.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task SendEmployeeCelebrationEmailsAsync()
        {
            var today = DateTime.Today;


            var anniversaryEmployees = await _context.Users
                .Where(x =>
                    x.Status == "Active" &&
                    x.JoiningDate.HasValue &&
                    x.JoiningDate.Value.Month == today.Month &&
                    x.JoiningDate.Value.Day == today.Day &&
                    !string.IsNullOrEmpty(x.Email))
                .Select(x => new
                {
                    x.UserId,
                    x.FullName,
                    x.Email,
                    x.JoiningDate
                })
                .ToListAsync();


            foreach (var employee in anniversaryEmployees)
            {
                int years = today.Year - employee.JoiningDate.Value.Year;


                string subject =
                    $"Happy Work Anniversary {employee.FullName}";


                string body = $@"
                        <html>
                        <body style='font-family:Segoe UI'>

                        <h2>🎉 Happy Work Anniversary!</h2>

                        <p>Dear <b>{employee.FullName}</b>,</p>

                        <p>
                        Congratulations on completing 
                        <b>{years} year(s)</b> with our organization.
                        </p>

                        <p>
                        Thank you for your valuable contribution.
                        </p>

                        <br/>

                        Regards,<br/>
                        <b>HR Team</b>

                        </body>
                        </html>";


                await _emailService.SendEmailAsync(
                    employee.Email,
                    subject,
                    body);
                await _notificationService.CreateNotificationAsync(
                        new List<int> { employee.UserId },
                        "Work Anniversary 🎉",
                        $"Congratulations on completing {years} years!",
                        "Work Anniversary",
                        employee.UserId
                );
            }


            var birthdayEmployees = await
                (
                    from emp in _context.EmployeePersonalDetails
                    join user in _context.Users
                    on emp.UserId equals user.UserId

                    where user.Status == "Active"
                    && emp.DateOfBirth.Month == today.Month
                    && emp.DateOfBirth.Day == today.Day
                    && !string.IsNullOrEmpty(user.Email)

                    select new
                    {
                        UserId = user.UserId,
                        Name = emp.FirstName + " " + emp.LastName,
                        Email = user.Email
                    }

                ).ToListAsync();



            foreach (var employee in birthdayEmployees)
            {

                string subject =
                    $"Happy Birthday {employee.Name}";


                string body = $@"
                        <html>
                        <body style='font-family:Segoe UI'>

                        <h2>🎂 Happy Birthday!</h2>

                        <p>
                        Dear <b>{employee.Name}</b>,
                        </p>


                        <p>
                        Wishing you a very Happy Birthday.
                        May your day be filled with happiness,
                        success and good health.
                        </p>


                        <br/>

                        Regards,<br/>
                        <b>HR Team</b>

                        </body>
                        </html>";


                await _emailService.SendEmailAsync(
                    employee.Email,
                    subject,
                    body);
                await _notificationService.CreateNotificationAsync(
                        new List<int> { employee.UserId },
                        "Happy Birthday 🎂",
                        "Wishing you a very Happy Birthday!",
                        "Birthday",
                        employee.UserId
                    );
            }
        }

    }
}

