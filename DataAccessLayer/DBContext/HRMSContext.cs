using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.DBContext;

public partial class HRMSContext : DbContext
{
    public HRMSContext()
    {
    }

    public HRMSContext(DbContextOptions<HRMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountType> AccountTypes { get; set; }

    public virtual DbSet<AdminMenuMaster> AdminMenuMasters { get; set; }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AssetAssignment> AssetAssignments { get; set; }

    public virtual DbSet<AssetCategory> AssetCategories { get; set; }

    public virtual DbSet<AssetFilterMaster> AssetFilterMasters { get; set; }

    public virtual DbSet<AssetMaster> AssetMasters { get; set; }

    public virtual DbSet<AssetRequest> AssetRequests { get; set; }

    public virtual DbSet<AssetStatus> AssetStatuses { get; set; }

    public virtual DbSet<AssetType> AssetTypes { get; set; }

    public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }

    public virtual DbSet<AttendanceConfiguration> AttendanceConfigurations { get; set; }

    public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }

    public virtual DbSet<AttendanceStatus> AttendanceStatuses { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<AuditLog1> AuditLogs1 { get; set; }

    public virtual DbSet<AuditLogDetail> AuditLogDetails { get; set; }

    public virtual DbSet<BirthdayEmployee> BirthdayEmployees { get; set; }

    public virtual DbSet<BloodGroup> BloodGroups { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<CandidateDocumentChecklist> CandidateDocumentChecklists { get; set; }

    public virtual DbSet<CandidateExperience> CandidateExperiences { get; set; }

    public virtual DbSet<CandidateInterview> CandidateInterviews { get; set; }

    public virtual DbSet<CandidateOffer> CandidateOffers { get; set; }

    public virtual DbSet<CandidateOnboarding> CandidateOnboardings { get; set; }

    public virtual DbSet<CandidateQualification> CandidateQualifications { get; set; }

    public virtual DbSet<CandidateScreening> CandidateScreenings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CertificationType> CertificationTypes { get; set; }

    public virtual DbSet<ChatbotKnowledge> ChatbotKnowledges { get; set; }

    public virtual DbSet<CityMaster> CityMasters { get; set; }

    public virtual DbSet<ClockInOut> ClockInOuts { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyEvent> CompanyEvents { get; set; }

    public virtual DbSet<CompanyModule> CompanyModules { get; set; }

    public virtual DbSet<CompanyNews> CompanyNews { get; set; }

    public virtual DbSet<CompanyNews1> CompanyNews1 { get; set; }

    public virtual DbSet<CompanyNewsMaster> CompanyNewsMasters { get; set; }

    public virtual DbSet<CompanyPoliciesMaster> CompanyPoliciesMasters { get; set; }

    public virtual DbSet<CompanyPolicy> CompanyPolicies { get; set; }

    public virtual DbSet<CompanyRegion> CompanyRegions { get; set; }

    public virtual DbSet<CompanySubscription> CompanySubscriptions { get; set; }

    public virtual DbSet<CompanyUsageLog> CompanyUsageLogs { get; set; }

    public virtual DbSet<CountryMaster> CountryMasters { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<CurrencyMaster> CurrencyMasters { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<EmailLog> EmailLogs { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<EmailTemplate1> EmailTemplates1 { get; set; }

    public virtual DbSet<EmailTemplateVariable> EmailTemplateVariables { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAssetFilterMaster> EmployeeAssetFilterMasters { get; set; }

    public virtual DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }

    public virtual DbSet<EmployeeBankDetail> EmployeeBankDetails { get; set; }

    public virtual DbSet<EmployeeCertification> EmployeeCertifications { get; set; }

    public virtual DbSet<EmployeeDailyWorkingHourDetail> EmployeeDailyWorkingHourDetails { get; set; }

    public virtual DbSet<EmployeeDailyWorkingHourHeader> EmployeeDailyWorkingHourHeaders { get; set; }

    public virtual DbSet<EmployeeDdlist> EmployeeDdlists { get; set; }

    public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

    public virtual DbSet<EmployeeEducation> EmployeeEducations { get; set; }

    public virtual DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts { get; set; }

    public virtual DbSet<EmployeeFamilyDetail> EmployeeFamilyDetails { get; set; }

    public virtual DbSet<EmployeeForm> EmployeeForms { get; set; }

    public virtual DbSet<EmployeeFormEmployee> EmployeeFormEmployees { get; set; }

    public virtual DbSet<EmployeeFormEmployeeFile> EmployeeFormEmployeeFiles { get; set; }

    public virtual DbSet<EmployeeFormFile> EmployeeFormFiles { get; set; }

    public virtual DbSet<EmployeeImage> EmployeeImages { get; set; }

    public virtual DbSet<EmployeeImmigration> EmployeeImmigrations { get; set; }

    public virtual DbSet<EmployeeJobHistory> EmployeeJobHistories { get; set; }

    public virtual DbSet<EmployeeKpi> EmployeeKpis { get; set; }

    public virtual DbSet<EmployeeKpiitem> EmployeeKpiitems { get; set; }

    public virtual DbSet<EmployeeLetter> EmployeeLetters { get; set; }

    public virtual DbSet<EmployeeLetterEmployee> EmployeeLetterEmployees { get; set; }

    public virtual DbSet<EmployeeLetterFile> EmployeeLetterFiles { get; set; }

    public virtual DbSet<EmployeeMaster> EmployeeMasters { get; set; }

    public virtual DbSet<EmployeeNotification> EmployeeNotifications { get; set; }

    public virtual DbSet<EmployeePersonalDetail> EmployeePersonalDetails { get; set; }

    public virtual DbSet<EmployeeReference> EmployeeReferences { get; set; }

    public virtual DbSet<EmployeeResignation> EmployeeResignations { get; set; }

    public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }

    public virtual DbSet<EmployeeW4> EmployeeW4s { get; set; }

    public virtual DbSet<Employmenttype> Employmenttypes { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<EventType1> EventTypes1 { get; set; }

    public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<ExpenseCategoryType> ExpenseCategoryTypes { get; set; }

    public virtual DbSet<ExpenseLimitConfig> ExpenseLimitConfigs { get; set; }

    public virtual DbSet<ExpenseStatus> ExpenseStatuses { get; set; }

    public virtual DbSet<FilingStatus> FilingStatuses { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<GeoLocation> GeoLocations { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<HelpDeskCategory> HelpDeskCategories { get; set; }

    public virtual DbSet<HolidayList> HolidayLists { get; set; }

    public virtual DbSet<InterviewLevel> InterviewLevels { get; set; }

    public virtual DbSet<JobApplication> JobApplications { get; set; }

    public virtual DbSet<KpiCategory> KpiCategories { get; set; }

    public virtual DbSet<LateLogin> LateLogins { get; set; }

    public virtual DbSet<LateLoginPolicy> LateLoginPolicies { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveStatus> LeaveStatuses { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<LeaveTypeDesignation> LeaveTypeDesignations { get; set; }

    public virtual DbSet<LeaveTypeGrade> LeaveTypeGrades { get; set; }

    public virtual DbSet<ManagerKpireview> ManagerKpireviews { get; set; }

    public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; }

    public virtual DbSet<MenuMaster> MenuMasters { get; set; }

    public virtual DbSet<MenuRoleMaster> MenuRoleMasters { get; set; }

    public virtual DbSet<MissedPunchRequest> MissedPunchRequests { get; set; }

    public virtual DbSet<MissedType> MissedTypes { get; set; }

    public virtual DbSet<ModeOfStudy> ModeOfStudies { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<NewsCategory> NewsCategories { get; set; }

    public virtual DbSet<OnboardingLink> OnboardingLinks { get; set; }

    public virtual DbSet<PayrollDetail> PayrollDetails { get; set; }

    public virtual DbSet<PayrollTransaction> PayrollTransactions { get; set; }

    public virtual DbSet<PerformanceKpi> PerformanceKpis { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<PlanModule> PlanModules { get; set; }

    public virtual DbSet<PlanRoleMenuMapping> PlanRoleMenuMappings { get; set; }

    public virtual DbSet<PolicyCategory> PolicyCategories { get; set; }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<ProjectMaster> ProjectMasters { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<RecruitmentNoticePeriod> RecruitmentNoticePeriods { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<Resignation> Resignations { get; set; }

    public virtual DbSet<ResignationTypeMaster> ResignationTypeMasters { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<SalaryComponent> SalaryComponents { get; set; }

    public virtual DbSet<SalaryStructure> SalaryStructures { get; set; }

    public virtual DbSet<SalaryStructureComponent> SalaryStructureComponents { get; set; }

    public virtual DbSet<ScreeningResult> ScreeningResults { get; set; }

    public virtual DbSet<ShiftAllocation> ShiftAllocations { get; set; }

    public virtual DbSet<ShiftMaster> ShiftMasters { get; set; }

    public virtual DbSet<StageMaster> StageMasters { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<StateMaster> StateMasters { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<SubscriptionPlan1> SubscriptionPlans1 { get; set; }

    public virtual DbSet<SuperadminCompany> SuperadminCompanies { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TaskFile> TaskFiles { get; set; }

    public virtual DbSet<TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<TaxSetting> TaxSettings { get; set; }

    public virtual DbSet<TaxType> TaxTypes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Timesheet> Timesheets { get; set; }

    public virtual DbSet<TimesheetApproval> TimesheetApprovals { get; set; }

    public virtual DbSet<TimesheetProject> TimesheetProjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

    public virtual DbSet<VisaType> VisaTypes { get; set; }

    public virtual DbSet<VisaTypeMaster> VisaTypeMasters { get; set; }

    public virtual DbSet<Weekoff> Weekoffs { get; set; }

    public virtual DbSet<WfhremoteRequest> WfhremoteRequests { get; set; }

    public virtual DbSet<WorkAuthStatusMaster> WorkAuthStatusMasters { get; set; }

    // VIEW TABLE 
    public DbSet<DemoUserSubscriptionDto> DemoUserSubscriptionDtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.29.53,50491;Database=HRMS_Prod_New;user id= sa; password=CtDev@2026@01; TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>(entity =>
        {
            entity.HasKey(e => e.AccountTypeId).HasName("PK__AccountT__8F95854F114C2509");

            entity.ToTable("AccountType", "adminmaster");

            entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");
            entity.Property(e => e.AccountType1)
                .HasMaxLength(100)
                .HasColumnName("AccountType");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<AdminMenuMaster>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__AdminMen__C99ED250E7A2567D");

            entity.ToTable("AdminMenuMaster", "UM");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.MenuName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ParentMenuId).HasColumnName("ParentMenuID");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.AssetId).HasName("PK__Assets__434923727A17F508");

            entity.ToTable("Assets", "Asset");

            entity.Property(e => e.AssetId).HasColumnName("AssetID");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.AssetCode).HasMaxLength(50);
            entity.Property(e => e.AssetCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AssetLocation).HasMaxLength(100);
            entity.Property(e => e.AssetModel).HasMaxLength(50);
            entity.Property(e => e.AssetName).HasMaxLength(100);
            entity.Property(e => e.AssetStatusId).HasColumnName("AssetStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.CurrencyCode).HasMaxLength(10);
            entity.Property(e => e.EmployeeName).HasMaxLength(150);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.PurchaseOrder).HasMaxLength(50);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AssetCategory).WithMany(p => p.Assets)
                .HasForeignKey(d => d.AssetCategoryId)
                .HasConstraintName("FK_Asset_AssetCategory");

            entity.HasOne(d => d.AssetStatus).WithMany(p => p.Assets)
                .HasForeignKey(d => d.AssetStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assets_AssetStatus");

            entity.HasOne(d => d.AssetType).WithMany(p => p.Assets)
                .HasForeignKey(d => d.AssetTypeId)
                .HasConstraintName("FK_Asset_AssetType");
        });

        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__AssetAss__32499E771A3EF2C4");

            entity.ToTable("AssetAssignments", "Asset");

            entity.Property(e => e.AssetCode).HasMaxLength(50);
            entity.Property(e => e.AssetName).HasMaxLength(100);
            entity.Property(e => e.AssetType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Assigned");
        });

        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.HasKey(e => e.AssetCategoryId).HasName("PK__AssetCat__C381F49D503086F2");

            entity.ToTable("AssetCategory", "adminmaster");

            entity.Property(e => e.AssetCategoryId).HasColumnName("AssetCategoryID");
            entity.Property(e => e.AssetCategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Company).WithMany(p => p.AssetCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AssetCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetCategory_Region");
        });

        modelBuilder.Entity<AssetFilterMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetFil__3214EC073DEAB740");

            entity.ToTable("AssetFilterMaster", "employee");

            entity.Property(e => e.AssetName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(150);
            entity.Property(e => e.Currency).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(150);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<AssetMaster>(entity =>
        {
            entity.HasKey(e => e.AssetId).HasName("PK__AssetMas__43492352955B5D25");

            entity.ToTable("AssetMaster", "employee");

            entity.Property(e => e.AssetCode).HasMaxLength(100);
            entity.Property(e => e.AssetCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AssetLocation).HasMaxLength(200);
            entity.Property(e => e.AssetModelOrVersion).HasMaxLength(200);
            entity.Property(e => e.AssetName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.PurchaseOrder).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<AssetRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__AssetReq__33A8519AAC4435DE");

            entity.ToTable("AssetRequests", "Asset");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.AssetCategoryId).HasColumnName("AssetCategoryID");
            entity.Property(e => e.AssetTypeId).HasColumnName("AssetTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(150);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PriorityId).HasColumnName("PriorityID");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<AssetStatus>(entity =>
        {
            entity.HasKey(e => e.AssetStatusId).HasName("PK__AssetSta__E63EE4F6768B5A7D");

            entity.ToTable("AssetStatus", "adminmaster");

            entity.Property(e => e.AssetStatusId).HasColumnName("AssetStatusID");
            entity.Property(e => e.AssetStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AssetStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AssetStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetStatus_Region");
        });

        modelBuilder.Entity<AssetType>(entity =>
        {
            entity.HasKey(e => e.AssetTypeId).HasName("PK__AssetTyp__FD33C2226B348F9B");

            entity.ToTable("AssetType", "adminmaster");

            entity.Property(e => e.AssetTypeId).HasColumnName("AssetTypeID");
            entity.Property(e => e.AssetCategoryId).HasColumnName("AssetCategoryID");
            entity.Property(e => e.AssetTypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AssetCategory).WithMany(p => p.AssetTypes)
                .HasForeignKey(d => d.AssetCategoryId)
                .HasConstraintName("FK_AssetType_AssetCategory");

            entity.HasOne(d => d.Company).WithMany(p => p.AssetTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AssetTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssetType_Region");
        });

        modelBuilder.Entity<AttachmentType>(entity =>
        {
            entity.HasKey(e => e.AttachmentTypeId).HasName("PK__Attachme__5C63AB44C849321C");

            entity.ToTable("AttachmentType", "adminmaster");

            entity.Property(e => e.AttachmentTypeId).HasColumnName("AttachmentTypeID");
            entity.Property(e => e.AttachmentCategory)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.AttachmentTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AttachmentTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttachmentType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AttachmentTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttachmentType_Region");
        });

        modelBuilder.Entity<AttendanceConfiguration>(entity =>
        {
            entity.HasKey(e => e.AttendanceConfigurationId).HasName("PK__Attendan__6E2C348877363B31");

            entity.ToTable("AttendanceConfiguration", "adminmaster");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Mode).HasMaxLength(50);
            entity.Property(e => e.OvertimeCalculation).HasMaxLength(50);
            entity.Property(e => e.ShiftEndTime).HasPrecision(0);
            entity.Property(e => e.ShiftStartTime).HasPrecision(0);
        });

        modelBuilder.Entity<AttendanceLog>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__8B69261C2B7B9877");

            entity.ToTable("AttendanceLogs", "attendance");

            entity.Property(e => e.EntryTime).HasColumnType("datetime");
            entity.Property(e => e.ExitTime).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Building).WithMany(p => p.AttendanceLogs)
                .HasForeignKey(d => d.BuildingId)
                .HasConstraintName("FK__Attendanc__Build__2077C861");
        });

        modelBuilder.Entity<AttendanceStatus>(entity =>
        {
            entity.HasKey(e => e.AttendanceStatusId).HasName("PK__Attendan__7696A71512B038C0");

            entity.ToTable("AttendanceStatus", "adminmaster");

            entity.Property(e => e.AttendanceStatusId).HasColumnName("AttendanceStatusID");
            entity.Property(e => e.AttendanceStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.AttendanceStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.AttendanceStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendanceStatus_Region");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F2398764B110C");

            entity.ToTable("AuditLogs", "superadmin");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TableName).HasMaxLength(100);
        });

        modelBuilder.Entity<AuditLog1>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CDD7A6FBEE4");

            entity.ToTable("AuditLog", "Users");

            entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Device).HasMaxLength(100);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(50)
                .HasColumnName("IPAddress");
            entity.Property(e => e.RecordId)
                .HasMaxLength(150)
                .HasColumnName("RecordID");
            entity.Property(e => e.Remarks).HasMaxLength(255);
            entity.Property(e => e.TableName).HasMaxLength(150);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserName).HasMaxLength(150);
        });

        modelBuilder.Entity<AuditLogDetail>(entity =>
        {
            entity.HasKey(e => e.AuditLogDetailId).HasName("PK__AuditLog__A5C56C5802B63039");

            entity.ToTable("AuditLogDetail", "Users");

            entity.Property(e => e.AuditLogDetailId).HasColumnName("AuditLogDetailID");
            entity.Property(e => e.AuditLogId).HasColumnName("AuditLogID");
            entity.Property(e => e.ColumnName).HasMaxLength(150);

            entity.HasOne(d => d.AuditLog).WithMany(p => p.AuditLogDetails)
                .HasForeignKey(d => d.AuditLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuditLogD__Audit__520F23F5");
        });

        modelBuilder.Entity<BirthdayEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Birthday__3214EC07EA53CF75");

            entity.ToTable("BirthdayEmployees", "adminmaster");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<BloodGroup>(entity =>
        {
            entity.HasKey(e => e.BloodGroupId).HasName("PK__BloodGro__4398C6AFEC82E221");

            entity.ToTable("BloodGroup", "adminmaster");

            entity.Property(e => e.BloodGroupId).HasColumnName("BloodGroupID");
            entity.Property(e => e.BloodGroupName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.BloodGroups)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodGroup_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.BloodGroups)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BloodGroup_Region");
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.BuildingId).HasName("PK__Building__5463CDC43030ED2B");

            entity.ToTable("Buildings", "attendance");

            entity.Property(e => e.BuildingName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
        });

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.HasKey(e => e.CandidateId).HasName("PK__Candidat__DF539B9C74E9B01C");

            entity.ToTable("Candidates", "Recruitment");

            entity.Property(e => e.AnyOffers)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CandidateName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CurrentCtc)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CurrentSalary).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ExpectedSalary).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Location)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.NoticePeriod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Reason)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ReferenceSource)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SeqNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Skills)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Technology)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CandidateDocumentChecklist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Candidat__3214EC07D732E95E");

            entity.ToTable("CandidateDocumentChecklist", "Recruitment");

            entity.Property(e => e.AadharCard).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExperienceLetter).HasMaxLength(255);
            entity.Property(e => e.HikeLetter).HasMaxLength(255);
            entity.Property(e => e.IdProof).HasMaxLength(255);
            entity.Property(e => e.OfferLetter).HasMaxLength(255);
            entity.Property(e => e.PanCard).HasMaxLength(255);
            entity.Property(e => e.Passport).HasMaxLength(255);
            entity.Property(e => e.RelievingLetter).HasMaxLength(255);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<CandidateExperience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PK__Candidat__2F4E34499B471D77");

            entity.ToTable("CandidateExperience", "Recruitment");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Designation)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Organization)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateExperiences)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_CandidateExperience_Candidate");
        });

        modelBuilder.Entity<CandidateInterview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Candidat__C97C58529DC1FA33");

            entity.ToTable("CandidateInterviews", "Recruitment");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.InterviewDate).HasColumnType("datetime");
            entity.Property(e => e.InterviewerName).HasMaxLength(150);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.MeetingLink).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Result)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateInterviews)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateInterviews_Candidate");

            entity.HasOne(d => d.LevelNoNavigation).WithMany(p => p.CandidateInterviews)
                .HasForeignKey(d => d.LevelNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateInterviews_InterviewLevels");
        });

        modelBuilder.Entity<CandidateOffer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("PK__Candidat__8EBCF0918D764A4D");

            entity.ToTable("CandidateOffers", "Recruitment");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpectedDoj).HasColumnName("ExpectedDOJ");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.Hrname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("HRName");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.OfferLetterPath).HasMaxLength(255);
            entity.Property(e => e.OfferStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OfferedCtc)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("OfferedCTC");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateOffers)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateOffers_Candidate");
        });

        modelBuilder.Entity<CandidateOnboarding>(entity =>
        {
            entity.HasKey(e => e.OnboardingId).HasName("PK__Candidat__43F2373E8AECB1BE");

            entity.ToTable("CandidateOnboarding", "Recruitment");

            entity.Property(e => e.BackgroundCheckStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.BuddyAssigned)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.OnboardingStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("InProgress");

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateOnboardings)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CandidateOnboarding_Candidate");
        });

        modelBuilder.Entity<CandidateQualification>(entity =>
        {
            entity.HasKey(e => e.QualificationId).HasName("PK__Candidat__C95C12AA423B4BBD");

            entity.ToTable("CandidateQualification", "Recruitment");

            entity.Property(e => e.BoardUniversity)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Qualification)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateQualifications)
                .HasForeignKey(d => d.CandidateId)
                .HasConstraintName("FK_CandidateQualification_Candidate");
        });

        modelBuilder.Entity<CandidateScreening>(entity =>
        {
            entity.HasKey(e => e.ScreeningId).HasName("PK__Candidat__7734E40C51AE1BC5");

            entity.ToTable("CandidateScreening", "Recruitment");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ScreeningDate).HasColumnType("datetime");
            entity.Property(e => e.ScreeningStatus)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Candidate).WithMany(p => p.CandidateScreenings)
                .HasForeignKey(d => d.CandidateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Screening_Candidate");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2BA578B045");

            entity.ToTable("Category", "adminmaster");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<CertificationType>(entity =>
        {
            entity.HasKey(e => e.CertificationTypeId).HasName("PK__Certific__D1A09641FE2EC857");

            entity.ToTable("CertificationTypes", "adminmaster");

            entity.HasIndex(e => new { e.CertificationTypeName, e.CompanyId, e.RegionId }, "UQ_CertificationType_Company_Region_Name").IsUnique();

            entity.Property(e => e.CertificationTypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.CertificationTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificationTypes_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.CertificationTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificationTypes_Region");
        });

        modelBuilder.Entity<ChatbotKnowledge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChatbotK__3214EC07C1B2F8C9");

            entity.ToTable("ChatbotKnowledge", "chatbot");

            entity.Property(e => e.CardType).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Keywords).HasMaxLength(500);
            entity.Property(e => e.Question).HasMaxLength(500);
        });

        modelBuilder.Entity<CityMaster>(entity =>
        {
            entity.HasKey(e => e.CityId);

            entity.ToTable("CityMaster", "adminmaster");

            entity.Property(e => e.CityName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ClockInOut>(entity =>
        {
            entity.ToTable("ClockInOut", "attendance");

            entity.Property(e => e.ActionType).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Company__2D971C4C55EB801F");

            entity.ToTable("Company", "UM");

            entity.HasIndex(e => e.CompanyCode, "UQ__Company__11A0134B4A5DF064").IsUnique();

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyContact)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyEmail)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Headquarters)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IndustryType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDefault).HasColumnName("isDefault");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PlanStartDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<CompanyEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CompanyE__3214EC074165E74D");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EventLocation).HasMaxLength(200);
            entity.Property(e => e.EventTitle).HasMaxLength(200);
            entity.Property(e => e.EventType).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsMeeting).HasDefaultValue(false);
            entity.Property(e => e.MeetingLink).HasMaxLength(500);
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<CompanyModule>(entity =>
        {
            entity.HasKey(e => e.CompanyModuleId).HasName("PK__CompanyM__FCA1DFCF9745DAF0");

            entity.ToTable("CompanyModule", "superadmin");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanyNews>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__CompanyN__954EBDF3D8DF3570");

            entity.ToTable("CompanyNews", "adminmaster");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanyNews1>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__CompanyN__954EBDF39F10088B");

            entity.ToTable("CompanyNews", "news");

            entity.Property(e => e.AttachmentName).HasMaxLength(200);
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<CompanyNewsMaster>(entity =>
        {
            entity.HasKey(e => e.NewsId).HasName("PK__CompanyN__954EBDF3A45B54C7");

            entity.ToTable("CompanyNewsMaster", "adminmaster");

            entity.Property(e => e.AttachmentName).HasMaxLength(255);
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanyPoliciesMaster>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__CompanyP__2E1339A4B939DF19");

            entity.ToTable("CompanyPoliciesMaster", "adminmaster");

            entity.Property(e => e.AttachmentName).HasMaxLength(255);
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PolicyTitle).HasMaxLength(250);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompanyPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__CompanyP__2E1339A46FB83288");

            entity.ToTable("CompanyPolicies", "adminmaster");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.CompanyPolicies)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyPolicies_PolicyCategory");
        });

        modelBuilder.Entity<CompanyRegion>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__CompanyR__ACD844A3A69DC243");

            entity.ToTable("CompanyRegions", "superadmin");

            entity.HasIndex(e => new { e.CompanyId, e.RegionCode }, "UQ_Company_Region").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RegionCode).HasMaxLength(50);
            entity.Property(e => e.RegionName).HasMaxLength(200);
            entity.Property(e => e.State).HasMaxLength(100);

            entity.HasOne(d => d.Company).WithMany(p => p.CompanyRegions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Region_Company");
        });

        modelBuilder.Entity<CompanySubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__CompanyS__9A2B249D9B1FDDA5");

            entity.ToTable("CompanySubscriptions", "superadmin");

            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);

            entity.HasOne(d => d.Plan).WithMany(p => p.CompanySubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sub_Plan");
        });

        modelBuilder.Entity<CompanyUsageLog>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("PK__CompanyU__29B1972036C8C3DA");

            entity.ToTable("CompanyUsageLogs", "superadmin");

            entity.Property(e => e.LoggedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.StorageUsedMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("StorageUsedMB");
        });

        modelBuilder.Entity<CountryMaster>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__CountryM__10D160BF24940B4E");

            entity.ToTable("CountryMaster", "adminmaster");

            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CountryName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currency__14470B10B0F39A99");

            entity.ToTable("Currency", "adminmaster");

            entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Company).WithMany(p => p.Currencies)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Currency_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Currencies)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Currency_Region");
        });

        modelBuilder.Entity<CurrencyMaster>(entity =>
        {
            entity.HasKey(e => e.CurrencyId).HasName("PK__Currency__14470B10718AD089");

            entity.ToTable("CurrencyMaster", "adminmaster");

            entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CurrencyName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BCDAE5BFA0D");

            entity.ToTable("Department", "adminmaster");

            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Departments)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Department_Region");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.DesignationId).HasName("PK__Designat__BABD603EF2E4AF01");

            entity.ToTable("Designation", "adminmaster");

            entity.Property(e => e.DesignationId).HasColumnName("DesignationID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DesignationName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Designations)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designation_Company");

            entity.HasOne(d => d.Department).WithMany(p => p.Designations)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Designation_Department");

            entity.HasOne(d => d.Grade).WithMany(p => p.Designations)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK_Designation_Grade");

            entity.HasOne(d => d.Region).WithMany(p => p.Designations)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Designation_Region");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentType", "adminmaster");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailLog__3214EC078C2C0838");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__EmailTem__F87ADD27CA61EC3B");

            entity.ToTable("EmailTemplate");

            entity.HasIndex(e => e.TemplateCode, "UQ__EmailTem__0FDB50818D15F817").IsUnique();

            entity.Property(e => e.ChannelType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Email");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Subject)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.TemplateCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TemplateName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TemplateType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmailTemplate1>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__EmailTem__F87ADD27F6512702");

            entity.ToTable("EmailTemplates");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.TemplateName).HasMaxLength(100);
        });

        modelBuilder.Entity<EmailTemplateVariable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTem__3214EC0753B6FFA6");

            entity.Property(e => e.DisplayName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsRequired).HasDefaultValue(false);
            entity.Property(e => e.SampleValue)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.VariableName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Template).WithMany(p => p.EmailTemplateVariables)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("FK__EmailTemp__Templ__216BEC9A");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F1107C3AC76");

            entity.ToTable("Employees", "attendance");

            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<EmployeeAssetFilterMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07C6D1EAEE");

            entity.ToTable("EmployeeAssetFilterMaster", "employee");

            entity.Property(e => e.AssetName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeAttendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Employee__8B69261C45A0086B");

            entity.ToTable("EmployeeAttendance", "attendance");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(200);
            entity.Property(e => e.GraceTime)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.GrossTime)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ShiftName).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeBankDetail>(entity =>
        {
            entity.HasKey(e => e.BankDetailsId).HasName("PK__Employee__1759C3A7CC4ECD8D");

            entity.ToTable("EmployeeBankDetails", "employee");

            entity.Property(e => e.BankDetailsId).HasColumnName("BankDetailsID");
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AccountTypeId).HasColumnName("AccountTypeID");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IFSCCode");
            entity.Property(e => e.Micrcode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MICRCode");
            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Upiid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("UPIID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AccountType).WithMany(p => p.EmployeeBankDetails)
                .HasForeignKey(d => d.AccountTypeId)
                .HasConstraintName("FK_EmployeeBankDetails_AccountType");
        });

        modelBuilder.Entity<EmployeeCertification>(entity =>
        {
            entity.HasKey(e => e.CertificationId).HasName("PK__Employee__1237E58A3EFB1227");

            entity.ToTable("EmployeeCertifications", "employee");

            entity.Property(e => e.CertificationName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DocumentPath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CertificationType).WithMany(p => p.EmployeeCertifications)
                .HasForeignKey(d => d.CertificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeCertifications_CertificationTypes");
        });

        modelBuilder.Entity<EmployeeDailyWorkingHourDetail>(entity =>
        {
            entity.HasKey(e => e.WorkingHourDetailId).HasName("PK__Employee__88759D0980BDE0A7");

            entity.ToTable("EmployeeDailyWorkingHourDetail", "adminmaster");

            entity.HasIndex(e => new { e.WorkingHourHeaderId, e.DayOfWeek }, "UQ_Header_Day").IsUnique();

            entity.Property(e => e.WorkingHourDetailId).HasColumnName("WorkingHourDetailID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsWorkingDay).HasDefaultValue(true);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TotalMinutes).HasDefaultValue(0);
            entity.Property(e => e.WorkingHourHeaderId).HasColumnName("WorkingHourHeaderID");

            entity.HasOne(d => d.WorkingHourHeader).WithMany(p => p.EmployeeDailyWorkingHourDetails)
                .HasForeignKey(d => d.WorkingHourHeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DWH_Detail_Header");
        });

        modelBuilder.Entity<EmployeeDailyWorkingHourHeader>(entity =>
        {
            entity.HasKey(e => e.WorkingHourHeaderId).HasName("PK__Employee__069BEB0A438E2B24");

            entity.ToTable("EmployeeDailyWorkingHourHeader", "adminmaster");

            entity.HasIndex(e => new { e.EmployeeMasterId, e.ShiftId }, "UQ_Employee_Shift").IsUnique();

            entity.Property(e => e.WorkingHourHeaderId).HasColumnName("WorkingHourHeaderID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeMasterId).HasColumnName("EmployeeMasterID");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.EmployeeMaster).WithMany(p => p.EmployeeDailyWorkingHourHeaders)
                .HasForeignKey(d => d.EmployeeMasterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DWH_Employee");

            entity.HasOne(d => d.Shift).WithMany(p => p.EmployeeDailyWorkingHourHeaders)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DWH_Shift");
        });

        modelBuilder.Entity<EmployeeDdlist>(entity =>
        {
            entity.HasKey(e => e.DdlistId).HasName("PK__Employee__B012948F1F2224CA");

            entity.ToTable("EmployeeDDList", "employee");

            entity.Property(e => e.DdlistId).HasColumnName("DDListID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BankName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.BranchName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DdcopyFilePath)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DDCopyFilePath");
            entity.Property(e => e.Dddate).HasColumnName("DDDate");
            entity.Property(e => e.Ddnumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DDNumber");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.PayeeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<EmployeeDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07ACCE4782");

            entity.ToTable("EmployeeDocuments", "employee");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(200);
            entity.Property(e => e.DocumentNumber).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);
        });

        modelBuilder.Entity<EmployeeEducation>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PK__Employee__4BBE3805C31A0F6F");

            entity.ToTable("EmployeeEducation", "employee");

            entity.Property(e => e.Board)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CertificateFilePath)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Institution)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Result)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Specialization)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.ModeOfStudy).WithMany(p => p.EmployeeEducations)
                .HasForeignKey(d => d.ModeOfStudyId)
                .HasConstraintName("FK_EmployeeEducation_ModeOfStudyId");
        });

        modelBuilder.Entity<EmployeeEmergencyContact>(entity =>
        {
            entity.HasKey(e => e.EmergencyContactId).HasName("PK__Employee__E8A61DAE9DF0CB78");

            entity.ToTable("EmployeeEmergencyContact", "employee");

            entity.Property(e => e.EmergencyContactId).HasColumnName("EmergencyContactID");
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.AlternatePhone).HasMaxLength(20);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.ContactName).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Relationship).WithMany(p => p.EmployeeEmergencyContacts)
                .HasForeignKey(d => d.RelationshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeEmergencyContact_Relationship");
        });

        modelBuilder.Entity<EmployeeFamilyDetail>(entity =>
        {
            entity.HasKey(e => e.FamilyId).HasName("PK__Employee__41D82F6B95A186AE");

            entity.ToTable("EmployeeFamilyDetails", "employee");

            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Occupation).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Relationship).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07E2A7B319");

            entity.ToTable("EmployeeForms", "employee");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.DocumentType).WithMany(p => p.EmployeeForms)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeForms_AttachmentType");
        });

        modelBuilder.Entity<EmployeeFormEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07785F0902");

            entity.ToTable("EmployeeFormEmployees", "employee");

            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(100);

            entity.HasOne(d => d.Form).WithMany(p => p.EmployeeFormEmployees)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeF__FormI__2630A1B7");
        });

        modelBuilder.Entity<EmployeeFormEmployeeFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC070093B227");

            entity.ToTable("EmployeeFormEmployeeFiles", "employee");

            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(255);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<EmployeeFormFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07FA12F27F");

            entity.ToTable("EmployeeFormFiles", "employee");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.Form).WithMany(p => p.EmployeeFormFiles)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeF__FormI__2724C5F0");
        });

        modelBuilder.Entity<EmployeeImage>(entity =>
        {
            entity.ToTable("EmployeeImage", "employee");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmployeeImmigration>(entity =>
        {
            entity.HasKey(e => e.ImmigrationId).HasName("PK__Employee__A69E9F83C96C42CD");

            entity.ToTable("EmployeeImmigration", "employee");

            entity.Property(e => e.ImmigrationId).HasColumnName("ImmigrationID");
            entity.Property(e => e.ContactPerson).HasMaxLength(150);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.EmployerAddress).HasMaxLength(250);
            entity.Property(e => e.EmployerContact).HasMaxLength(100);
            entity.Property(e => e.EmployerName).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Nationality).HasMaxLength(100);
            entity.Property(e => e.OtherDocumentsPath).HasMaxLength(255);
            entity.Property(e => e.PassportCopyPath).HasMaxLength(255);
            entity.Property(e => e.PassportNumber).HasMaxLength(50);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.VisaCopyPath).HasMaxLength(255);
            entity.Property(e => e.VisaIssuingCountry).HasMaxLength(100);
            entity.Property(e => e.VisaNumber).HasMaxLength(50);
            entity.Property(e => e.VisaTypeId).HasColumnName("VisaTypeID");
        });

        modelBuilder.Entity<EmployeeJobHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC077A7D826B");

            entity.ToTable("EmployeeJobHistory", "employee");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Employer)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastCtc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LastCTC");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ReasonForLeaving)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UploadDocument)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Website)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeKpi>(entity =>
        {
            entity.HasKey(e => e.Kpiid).HasName("PK__Employee__72E692A15CDF2E63");

            entity.ToTable("EmployeeKPI", "Performance");

            entity.Property(e => e.Kpiid).HasColumnName("KPIId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DocumentEvidencePath)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeNameId)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PerformanceCycle)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProbationStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProgressType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SelfReviewSummary).IsUnicode(false);
        });

        modelBuilder.Entity<EmployeeKpiitem>(entity =>
        {
            entity.HasKey(e => e.KpiitemId).HasName("PK__Employee__428820F1FCFEE9D8");

            entity.ToTable("EmployeeKPIItems", "Performance");

            entity.Property(e => e.KpiitemId).HasColumnName("KPIItemId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Kpiid).HasColumnName("KPIId");
            entity.Property(e => e.Kpiobjective)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("KPIObjective");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).IsUnicode(false);
            entity.Property(e => e.Target)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TaskCompleted)
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne(d => d.Kpi).WithMany(p => p.EmployeeKpiitems)
                .HasForeignKey(d => d.Kpiid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeK__KPIId__3C1FE2D6");
        });

        modelBuilder.Entity<EmployeeLetter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC071B697CD9");

            entity.ToTable("EmployeeLetters", "employee");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DocumentName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.EmployeeLetters)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeLetters_AttachmentType");
        });

        modelBuilder.Entity<EmployeeLetterEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0781089790");

            entity.ToTable("EmployeeLetterEmployees", "employee");

            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeName).HasMaxLength(100);

            entity.HasOne(d => d.Letter).WithMany(p => p.EmployeeLetterEmployees)
                .HasForeignKey(d => d.LetterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Lette__290D0E62");
        });

        modelBuilder.Entity<EmployeeLetterFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07E37442C8");

            entity.ToTable("EmployeeLetterFiles", "employee");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.Letter).WithMany(p => p.EmployeeLetterFiles)
                .HasForeignKey(d => d.LetterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmployeeL__Lette__2A01329B");
        });

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(e => e.EmployeeMasterId).HasName("PK__Employee__EE32E15930A8E216");

            entity.ToTable("EmployeeMaster", "adminmaster");

            entity.Property(e => e.EmployeeMasterId).HasColumnName("EmployeeMasterID");
            entity.Property(e => e.CompanyId).HasColumnName("companyId");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.RegionId).HasColumnName("regionId");
            entity.Property(e => e.Role).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmployeeMasterCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__EmployeeM__Creat__6774552F");

            entity.HasOne(d => d.Manager).WithMany(p => p.EmployeeMasterManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__EmployeeM__Manag__668030F6");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.EmployeeMasters)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_EmployeeMaster_RoleMaster");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.EmployeeMasterUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("FK__EmployeeM__Updat__68687968");
        });

        modelBuilder.Entity<EmployeeNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC07D0D09C75");

            entity.ToTable("EmployeeNotifications", "employee");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(500);
        });

        modelBuilder.Entity<EmployeePersonalDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0794BDC778");

            entity.ToTable("EmployeePersonalDetails", "employee");

            entity.Property(e => e.AadhaarNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.BandGrade).HasMaxLength(50);
            entity.Property(e => e.BloodGroup).HasMaxLength(5);
            entity.Property(e => e.Citizenship).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DateOfJoining).HasColumnType("datetime");
            entity.Property(e => e.DrivingLicence).HasMaxLength(20);
            entity.Property(e => e.EmployeeType).HasMaxLength(50);
            entity.Property(e => e.EsicNumber).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.LinkedInProfile).HasMaxLength(150);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Pannumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PANNumber");
            entity.Property(e => e.PassportNumber).HasMaxLength(20);
            entity.Property(e => e.PermanentAddress).HasMaxLength(250);
            entity.Property(e => e.PersonalEmail).HasMaxLength(100);
            entity.Property(e => e.Pfnumber)
                .HasMaxLength(50)
                .HasColumnName("PFNumber");
            entity.Property(e => e.PlaceOfBirth).HasMaxLength(100);
            entity.Property(e => e.PresentAddress).HasMaxLength(250);
            entity.Property(e => e.PreviousExperienceText).HasMaxLength(250);
            entity.Property(e => e.ProfilePictureName).HasMaxLength(200);
            entity.Property(e => e.Religion).HasMaxLength(50);
            entity.Property(e => e.Uan)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("UAN");
            entity.Property(e => e.WorkPhone).HasMaxLength(15);

            entity.HasOne(d => d.Gender).WithMany(p => p.EmployeePersonalDetails)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePersonalDetails_Gender");

            entity.HasOne(d => d.MaritalStatus).WithMany(p => p.EmployeePersonalDetails)
                .HasForeignKey(d => d.MaritalStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeePersonalDetails_MaritalStatus");
        });

        modelBuilder.Entity<EmployeeReference>(entity =>
        {
            entity.HasKey(e => e.ReferenceId).HasName("PK__Employee__E1A99A790BE1BA95");

            entity.ToTable("EmployeeReferences", "employee");

            entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId)
                .HasMaxLength(100)
                .HasColumnName("EmailID");
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TitleOrDesignation).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<EmployeeResignation>(entity =>
        {
            entity.HasKey(e => e.ResignationId).HasName("PK__Employee__CD4E6DB5E8524D6B");

            entity.ToTable("EmployeeResignation", "employee");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.HrApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("hrApprovedDate");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.HrReason)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("hrReason");
            entity.Property(e => e.HrRejectedDate)
                .HasColumnType("datetime")
                .HasColumnName("hrRejectedDate");
            entity.Property(e => e.ManagerApprovedDate)
                .HasColumnType("datetime")
                .HasColumnName("managerApprovedDate");
            entity.Property(e => e.ManagerReason)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("managerReason");
            entity.Property(e => e.ManagerRejectedDate)
                .HasColumnType("datetime")
                .HasColumnName("managerRejectedDate");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.NoticePeriod).HasMaxLength(50);
            entity.Property(e => e.ResignationType).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<EmployeeSalary>(entity =>
        {
            entity.HasKey(e => e.EmployeeSalaryId).HasName("PK__Employee__09720DBF9F1DEC71");

            entity.ToTable("EmployeeSalary", "payroll");

            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Ctc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CTC");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasMaxLength(50);

            entity.HasOne(d => d.Structure).WithMany(p => p.EmployeeSalaries)
                .HasForeignKey(d => d.StructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmployeeSalary_Structure");
        });

        modelBuilder.Entity<EmployeeW4>(entity =>
        {
            entity.HasKey(e => e.W4Id).HasName("PK__employee__6B5941790F39BD8B");

            entity.ToTable("employee_w4s", "employee");

            entity.HasIndex(e => e.Ssn, "UQ__employee__DDDF0AE6E553B3C2").IsUnique();

            entity.Property(e => e.W4Id).HasColumnName("w4_id");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Deductions)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("deductions");
            entity.Property(e => e.DependentAmounts)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("dependent_amounts");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeSignature)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employee_signature");
            entity.Property(e => e.ExtraWithholding)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("extra_withholding");
            entity.Property(e => e.FilingStatus)
                .HasMaxLength(100)
                .HasColumnName("filing_status");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.FormDate).HasColumnName("form_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleInitial)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("middle_initial");
            entity.Property(e => e.MultipleJobsOrSpouse)
                .HasDefaultValue(false)
                .HasColumnName("multiple_jobs_or_spouse");
            entity.Property(e => e.OtherIncome)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("other_income");
            entity.Property(e => e.Ssn)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ssn");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("state");
            entity.Property(e => e.TotalDependents)
                .HasDefaultValue(0)
                .HasColumnName("total_dependents");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("zip_code");
        });

        modelBuilder.Entity<Employmenttype>(entity =>
        {
            entity.HasKey(e => e.EmploymenttypeId).HasName("PK__Employme__C384D40C3490B18A");

            entity.ToTable("Employmenttype", "adminmaster");

            entity.Property(e => e.EmploymenttypeId).HasColumnName("EmploymenttypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EmploymenttypeName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Employmenttypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employmenttype_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Employmenttypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employmenttype_Region");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C810B7C214A2");

            entity.ToTable("Events", "Events");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EventName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.EventType).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Events_EventTypes");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.EventTypeId).HasName("PK__EventTyp__A9216B1F163D306A");

            entity.ToTable("EventType", "adminmaster");

            entity.Property(e => e.EventTypeId).HasColumnName("EventTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EventTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.EventTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.EventTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventType_Region");
        });

        modelBuilder.Entity<EventType1>(entity =>
        {
            entity.HasKey(e => e.EventTypeId).HasName("PK__EventTyp__A9216B3FF378729E");

            entity.ToTable("EventTypes", "adminmaster");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EventTypeName).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exceptio__3214EC076267DEA6");

            entity.ToTable("Exception_Log", "logger");

            entity.Property(e => e.ActionName).HasMaxLength(150);
            entity.Property(e => e.BrowserInfo).HasMaxLength(500);
            entity.Property(e => e.ClientIp)
                .HasMaxLength(50)
                .HasColumnName("ClientIP");
            entity.Property(e => e.ControllerName).HasMaxLength(150);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ErrorCode).HasMaxLength(50);
            entity.Property(e => e.ErrorType).HasMaxLength(100);
            entity.Property(e => e.HostName).HasMaxLength(150);
            entity.Property(e => e.RequestPath).HasMaxLength(500);
            entity.Property(e => e.UserId).HasMaxLength(100);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expense__1445CFF37C3B716D");

            entity.ToTable("Expense", "expense");

            entity.Property(e => e.ExpenseId).HasColumnName("ExpenseID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(3)
                .HasDefaultValue("INR");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("departmentName");
            entity.Property(e => e.ExpenseCategoryId).HasColumnName("ExpenseCategoryID");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(250);
            entity.Property(e => e.ProjectName).HasMaxLength(250);
            entity.Property(e => e.Reason).HasMaxLength(1000);
            entity.Property(e => e.ReceiptPath).HasMaxLength(500);
            entity.Property(e => e.ReferenceNo).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Department).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Expense_Department");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.ExpenseCategoryId)
                .HasConstraintName("FK_Expense_Category");
        });

        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.ExpenseCategoryId).HasName("PK__ExpenseC__9C2C63D8C5451944");

            entity.ToTable("ExpenseCategory", "adminmaster");

            entity.Property(e => e.ExpenseCategoryId).HasColumnName("ExpenseCategoryID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExpenseCategoryName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<ExpenseCategoryType>(entity =>
        {
            entity.HasKey(e => e.ExpenseCategoryTypeId).HasName("PK__ExpenseC__CB9B6F4BEA774168");

            entity.ToTable("ExpenseCategoryType", "adminmaster");

            entity.Property(e => e.ExpenseCategoryTypeId).HasColumnName("ExpenseCategoryTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.ExpenseCategoryTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ExpenseCategoryTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseCategoryType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ExpenseCategoryTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseCategoryType_Region");
        });

        modelBuilder.Entity<ExpenseLimitConfig>(entity =>
        {
            entity.HasKey(e => e.ExpenseLimitConfigId).HasName("PK__ExpenseL__29D57023F52783FE");

            entity.ToTable("ExpenseLimitConfig", "adminmaster");

            entity.Property(e => e.ExpenseLimitConfigId).HasColumnName("ExpenseLimitConfigID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("INR");
            entity.Property(e => e.DailyLimit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.ExpenseCategoryId).HasColumnName("ExpenseCategoryID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MonthlyLimit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PerTransactionLimit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.ExpenseLimitConfigs)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_ExpenseLimit_Department");

            entity.HasOne(d => d.ExpenseCategory).WithMany(p => p.ExpenseLimitConfigs)
                .HasForeignKey(d => d.ExpenseCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseLimit_Category");
        });

        modelBuilder.Entity<ExpenseStatus>(entity =>
        {
            entity.HasKey(e => e.ExpenseStatusId).HasName("PK__ExpenseS__A8E82F40EB1C31FA");

            entity.ToTable("ExpenseStatus", "adminmaster");

            entity.Property(e => e.ExpenseStatusId).HasColumnName("ExpenseStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.ExpenseStatusName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ExpenseStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ExpenseStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpenseStatus_Region");
        });

        modelBuilder.Entity<FilingStatus>(entity =>
        {
            entity.HasKey(e => e.FilingStatusId).HasName("PK__FilingSt__93F42EE790AD58FD");

            entity.ToTable("FilingStatus", "adminmaster");

            entity.Property(e => e.FilingStatusId).HasColumnName("FilingStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.StatusName).HasMaxLength(150);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.GenderId).HasName("PK__Gender__4E24E817CD6A7C33");

            entity.ToTable("Gender", "adminmaster");

            entity.Property(e => e.GenderId).HasColumnName("GenderID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.GenderName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Genders)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gender_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Genders)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gender_Region");
        });

        modelBuilder.Entity<GeoLocation>(entity =>
        {
            entity.HasKey(e => e.GeoLocationId).HasName("PK__GeoLocat__81B966A36FF92290");

            entity.ToTable("GeoLocations", "adminmaster");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 14)");
            entity.Property(e => e.LocationName).HasMaxLength(200);
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 14)");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Grade__54F87A37980F6579");

            entity.ToTable("Grade", "adminmaster");

            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GradeName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<HelpDeskCategory>(entity =>
        {
            entity.HasKey(e => e.HelpDeskCategoryId).HasName("PK__HelpDesk__9F010540E4760B6A");

            entity.ToTable("HelpDeskCategory", "adminmaster");

            entity.Property(e => e.HelpDeskCategoryId).HasColumnName("HelpDeskCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.HelpDeskCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.HelpDeskCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HelpDeskCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.HelpDeskCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HelpDeskCategory_Region");
        });

        modelBuilder.Entity<HolidayList>(entity =>
        {
            entity.HasKey(e => e.HolidayListId).HasName("PK__HolidayL__1173F0DA06B2103F");

            entity.ToTable("HolidayList", "adminmaster");

            entity.Property(e => e.HolidayListId).HasColumnName("HolidayListID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HolidayListName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.HolidayLists)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HolidayList_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.HolidayLists)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HolidayList_Region");
        });

        modelBuilder.Entity<InterviewLevel>(entity =>
        {
            entity.HasKey(e => e.InterviewLevelsId).HasName("PK__Intervie__20193CF8D16BF20E");

            entity.ToTable("InterviewLevels", "adminmaster");

            entity.Property(e => e.InterviewLevelsId).HasColumnName("InterviewLevelsID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.InterviewLevels).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.InterviewLevels)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InterviewLevels_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.InterviewLevels)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InterviewLevels_Region");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__JobAppli__C93A4C990C323E90");

            entity.ToTable("JobApplications", "adminmaster");

            entity.Property(e => e.AppliedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CandidateName).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.ExperienceYears).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobTitle).HasMaxLength(150);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.ResumeUrl).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Applied");
            entity.Property(e => e.Technology).HasMaxLength(500);
        });

        modelBuilder.Entity<KpiCategory>(entity =>
        {
            entity.HasKey(e => e.KpiCategoryId).HasName("PK__KpiCateg__B31BD9B8A90774D5");

            entity.ToTable("KpiCategory", "adminmaster");

            entity.Property(e => e.KpiCategoryId).HasColumnName("KpiCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.KpiCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.KpiCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KpiCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.KpiCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KpiCategory_Region");
        });

        modelBuilder.Entity<LateLogin>(entity =>
        {
            entity.HasKey(e => e.LateLoginId).HasName("PK__LateLogi__585431B1596CDC50");

            entity.ToTable("LateLogin", "adminmaster");

            entity.Property(e => e.LateLoginId).HasColumnName("LateLoginID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LateLogin1)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("LateLogin");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.LateLogins)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LateLogin_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.LateLogins)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LateLogin_Region");
        });

        modelBuilder.Entity<LateLoginPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__LateLogi__2E1339A4CC92F01D");

            entity.ToTable("LateLoginPolicy", "adminmaster");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Lopdays)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("LOPDays");
            entity.Property(e => e.Loptype)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LOPType");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveRequestId).HasName("PK__LeaveReq__609421EEFBB026AB");

            entity.ToTable("LeaveRequests", "Leaves");

            entity.Property(e => e.AppliedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ApprovedRejectedDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.IsHalfDay).HasDefaultValue(false);
            entity.Property(e => e.Lopdays)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("LOPDays");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Reason)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalDays).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequests_LeaveType");

            entity.HasOne(d => d.ReportingManager).WithMany(p => p.LeaveRequestReportingManagers)
                .HasForeignKey(d => d.ReportingManagerId)
                .HasConstraintName("FK_LeaveRequests_ReportingManager");

            entity.HasOne(d => d.User).WithMany(p => p.LeaveRequestUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequests_Users");
        });

        modelBuilder.Entity<LeaveStatus>(entity =>
        {
            entity.HasKey(e => e.LeaveStatusId).HasName("PK__LeaveSta__75EE81DAEFAA5B65");

            entity.ToTable("LeaveStatus", "adminmaster");

            entity.Property(e => e.LeaveStatusId).HasColumnName("LeaveStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.LeaveStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.LeaveStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveStatus_Region");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK__LeaveTyp__43BE8FF408B84103");

            entity.ToTable("LeaveType", "adminmaster");

            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.DesignationId).HasColumnName("designationId");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveTypeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.LeaveTypes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveType_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.LeaveTypes)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveType_Region");
        });

        modelBuilder.Entity<LeaveTypeDesignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LeaveTyp__3214EC077AB13A2A");

            entity.ToTable("LeaveTypeDesignation", "adminmaster");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.DesignationId).HasColumnName("DesignationID");
            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<LeaveTypeGrade>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeGradeId).HasName("PK__LeaveTyp__862168A449F4E357");

            entity.ToTable("LeaveTypeGrade", "adminmaster");

            entity.Property(e => e.LeaveTypeGradeId).HasColumnName("LeaveTypeGradeID");
            entity.Property(e => e.GradeId).HasColumnName("GradeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LeaveTypeId).HasColumnName("LeaveTypeID");

            entity.HasOne(d => d.Grade).WithMany(p => p.LeaveTypeGrades)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveType__Grade__07AC1A97");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveTypeGrades)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LeaveType__Leave__08A03ED0");
        });

        modelBuilder.Entity<ManagerKpireview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ManagerK__74BC79CE0F124676");

            entity.ToTable("ManagerKPIReview", "Performance");

            entity.Property(e => e.AvgRating).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.KpiitemId).HasColumnName("KPIItemId");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<MaritalStatus>(entity =>
        {
            entity.HasKey(e => e.MaritalStatusId).HasName("PK__MaritalS__C8B1BA52ACB460F0");

            entity.ToTable("MaritalStatus", "adminmaster");

            entity.Property(e => e.MaritalStatusId).HasColumnName("MaritalStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaritalStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.MaritalStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaritalStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.MaritalStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaritalStatus_Region");
        });

        modelBuilder.Entity<MenuMaster>(entity =>
        {
            entity.HasKey(e => e.MenuId).HasName("PK__MenuMast__C99ED250BF9328B1");

            entity.ToTable("MenuMaster", "UM");

            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MenuName).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ParentMenuId).HasColumnName("ParentMenuID");
            entity.Property(e => e.Url).HasMaxLength(255);
        });

        modelBuilder.Entity<MenuRoleMaster>(entity =>
        {
            entity.HasKey(e => e.MenuRoleId).HasName("PK__MenuRole__880F2CC11A60BA0C");

            entity.ToTable("MenuRoleMaster", "UM");

            entity.HasIndex(e => new { e.RoleId, e.MenuId }, "UQ_MenuRole").IsUnique();

            entity.Property(e => e.MenuRoleId).HasColumnName("MenuRoleID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MenuId).HasColumnName("MenuID");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuRoleMasters)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuRoleMaster_Menu");

            entity.HasOne(d => d.Role).WithMany(p => p.MenuRoleMasters)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuRoleMaster_Role");
        });

        modelBuilder.Entity<MissedPunchRequest>(entity =>
        {
            entity.HasKey(e => e.MissedPunchRequestId).HasName("PK__MissedPu__8D1CBEB2D614AF0E");

            entity.ToTable("MissedPunchRequests", "employee");

            entity.Property(e => e.MissedPunchRequestId).HasColumnName("MissedPunchRequestID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.MissedType).HasMaxLength(50);
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
        });

        modelBuilder.Entity<MissedType>(entity =>
        {
            entity.HasKey(e => e.MissedTypeId).HasName("PK__MissedTy__284C499F08DB8C62");

            entity.ToTable("MissedType", "adminmaster");

            entity.Property(e => e.MissedTypeId).HasColumnName("MissedTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MissedType1)
                .HasMaxLength(100)
                .HasColumnName("MissedType");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
        });

        modelBuilder.Entity<ModeOfStudy>(entity =>
        {
            entity.ToTable("ModeOfStudy", "adminmaster");

            entity.HasIndex(e => new { e.ModeName, e.UserId }, "UQ_ModeName_User").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModeName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK__Module__2B7477A7E774C512");

            entity.ToTable("Module", "superadmin");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModuleName).HasMaxLength(200);
            entity.Property(e => e.ModuleType).HasMaxLength(50);
            entity.Property(e => e.Route).HasMaxLength(500);
        });

        modelBuilder.Entity<NewsCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__NewsCate__19093A0B647A735C");

            entity.ToTable("NewsCategory", "adminmaster");

            entity.Property(e => e.CategoryName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<OnboardingLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Onboardi__3214EC07CFCD7EF2");

            entity.ToTable("OnboardingLink");

            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(200);
        });

        modelBuilder.Entity<PayrollDetail>(entity =>
        {
            entity.HasKey(e => e.PayrollDetailId).HasName("PK__PayrollD__010127C962E7EA42");

            entity.ToTable("PayrollDetails", "payroll");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasMaxLength(50);

            entity.HasOne(d => d.Component).WithMany(p => p.PayrollDetails)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PayrollDetails_Component");

            entity.HasOne(d => d.Payroll).WithMany(p => p.PayrollDetails)
                .HasForeignKey(d => d.PayrollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PayrollDetails_Payroll");
        });

        modelBuilder.Entity<PayrollTransaction>(entity =>
        {
            entity.HasKey(e => e.PayrollId).HasName("PK__PayrollT__99DFC67245893592");

            entity.ToTable("PayrollTransactions", "payroll");

            entity.Property(e => e.AttendanceDeduction)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.GrossSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.IsDownloadApproved).HasDefaultValue(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegionId).HasMaxLength(50);
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Not Requested");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalDeductions).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PerformanceKpi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Performa__3214EC07BA29342B");

            entity.ToTable("PerformanceKPI");

            entity.Property(e => e.Achieved).HasMaxLength(200);
            entity.Property(e => e.Kpiname)
                .HasMaxLength(300)
                .HasColumnName("KPIName");
            entity.Property(e => e.Score).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Target).HasMaxLength(200);
            entity.Property(e => e.Weightage).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Performa__3214EC07801F9C7D");

            entity.ToTable("PerformanceReview");

            entity.Property(e => e.AppraisalYear).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(200);
            entity.Property(e => e.DepartmentProject).HasMaxLength(200);
            entity.Property(e => e.Designation).HasMaxLength(200);
            entity.Property(e => e.DocumentEvidence).HasMaxLength(500);
            entity.Property(e => e.FinalScore).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PerformanceCycle).HasMaxLength(50);
            entity.Property(e => e.ProbationStatus).HasMaxLength(50);
            entity.Property(e => e.ProgressType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Plan__755C22B746C94E40");

            entity.ToTable("Plan", "superadmin");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PlanName).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<PlanModule>(entity =>
        {
            entity.HasKey(e => e.PlanModuleId).HasName("PK__PlanModu__21AC114A5062B765");

            entity.ToTable("PlanModule", "superadmin");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<PlanRoleMenuMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanRole__3214EC07F7841CAA");

            entity.ToTable("PlanRoleMenuMapping", "UM");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<PolicyCategory>(entity =>
        {
            entity.HasKey(e => e.PolicyCategoryId).HasName("PK__PolicyCa__C0F36D7D9ECC7967");

            entity.ToTable("PolicyCategory", "adminmaster");

            entity.Property(e => e.PolicyCategoryId).HasColumnName("PolicyCategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PolicyCategoryName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.PolicyCategories)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyCategory_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.PolicyCategories)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PolicyCategory_Region");
        });

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.PriorityId).HasName("PK__Priority__D0A3D0DEE4ACBB64");

            entity.ToTable("Priority", "adminmaster");

            entity.Property(e => e.PriorityId).HasColumnName("PriorityID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.PriorityName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Company).WithMany(p => p.Priorities)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Priority_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Priorities)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Priority_Region");
        });

        modelBuilder.Entity<ProjectMaster>(entity =>
        {
            entity.HasKey(e => e.ProjectMasterId).HasName("PK__ProjectM__D51A037693DA21D5");

            entity.ToTable("ProjectMaster", "adminmaster");

            entity.Property(e => e.ProjectMasterId).HasColumnName("ProjectMasterID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Company).WithMany(p => p.ProjectMasters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMaster_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ProjectMasters)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectMaster_Region");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.ProjectStatusId).HasName("PK__ProjectS__F3B67D2DD336CCE8");

            entity.ToTable("ProjectStatus", "adminmaster");

            entity.Property(e => e.ProjectStatusId).HasColumnName("ProjectStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ProjectStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ProjectStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectStatus_Region");
        });

        modelBuilder.Entity<RecruitmentNoticePeriod>(entity =>
        {
            entity.HasKey(e => e.RecruitmentNoticePeriodId).HasName("PK__Recruitm__979FFDEFCBD950E6");

            entity.ToTable("RecruitmentNoticePeriod", "adminmaster");

            entity.Property(e => e.RecruitmentNoticePeriodId).HasColumnName("RecruitmentNoticePeriodID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.NoticePeriod).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Company).WithMany(p => p.RecruitmentNoticePeriods)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecruitmentNoticePeriod_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.RecruitmentNoticePeriods)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RecruitmentNoticePeriod_Region");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Region__ACD8444314C945F5");

            entity.ToTable("Region", "UM");

            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Regions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Region__CompanyI__4C564A9F");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.RelationshipId).HasName("PK__Relation__31FEB861D8EBF60C");

            entity.ToTable("Relationship", "adminmaster");

            entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RelationshipName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relationship_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Relationships)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Relationship_Region");
        });

        modelBuilder.Entity<Resignation>(entity =>
        {
            entity.HasKey(e => e.ResignationId).HasName("PK__Resignat__CD4E6DD5EE601C2C");

            entity.ToTable("Resignations", "adminmaster");

            entity.Property(e => e.ResignationId).HasColumnName("ResignationID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.ResignationType).HasMaxLength(200);
        });

        modelBuilder.Entity<ResignationTypeMaster>(entity =>
        {
            entity.HasKey(e => e.ResignationTypeId).HasName("PK__Resignat__4FEF0EA75832A0D7");

            entity.ToTable("ResignationTypeMaster", "adminmaster");

            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__RoleMast__8AFACE3A5FA1E932");

            entity.ToTable("RoleMaster", "UM");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasColumnName("regionId");
            entity.Property(e => e.RoleDescription).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Company).WithMany(p => p.RoleMasters)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_RoleMaster_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.RoleMasters)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_RoleMaster_Region");
        });

        modelBuilder.Entity<SalaryComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__SalaryCo__D79CF04EA102FAE3");

            entity.ToTable("SalaryComponents", "payroll");

            entity.Property(e => e.CalculationType).HasMaxLength(20);
            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.ComponentName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.PercentageOf).HasMaxLength(20);
            entity.Property(e => e.RegionId).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(20);
        });

        modelBuilder.Entity<SalaryStructure>(entity =>
        {
            entity.HasKey(e => e.StructureId).HasName("PK__SalarySt__4A1C07ABE21CB859");

            entity.ToTable("SalaryStructures", "payroll");

            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasMaxLength(50);
            entity.Property(e => e.StructureName).HasMaxLength(100);
        });

        modelBuilder.Entity<SalaryStructureComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SalarySt__3214EC079FC7E071");

            entity.ToTable("SalaryStructureComponents", "payroll");

            entity.Property(e => e.CalculationType).HasMaxLength(20);
            entity.Property(e => e.CompanyId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.RegionId).HasMaxLength(50);
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Component).WithMany(p => p.SalaryStructureComponents)
                .HasForeignKey(d => d.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Component");

            entity.HasOne(d => d.Structure).WithMany(p => p.SalaryStructureComponents)
                .HasForeignKey(d => d.StructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Structure");
        });

        modelBuilder.Entity<ScreeningResult>(entity =>
        {
            entity.HasKey(e => e.ScreeningResultId).HasName("PK__Screenin__4EBB59125B5AE574");

            entity.ToTable("ScreeningResult", "adminmaster");

            entity.Property(e => e.ScreeningResultId).HasColumnName("ScreeningResultID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Weekoff).HasMaxLength(20);

            entity.HasOne(d => d.Company).WithMany(p => p.ScreeningResults)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScreeningResult_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.ScreeningResults)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScreeningResult_Region");
        });

        modelBuilder.Entity<ShiftAllocation>(entity =>
        {
            entity.HasKey(e => e.ShiftAllocationId).HasName("PK__ShiftAll__3A3EFDAAC6CD046A");

            entity.ToTable("ShiftAllocation", "attendance");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.ShiftName).HasMaxLength(200);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ShiftMaster>(entity =>
        {
            entity.HasKey(e => e.ShiftId).HasName("PK__ShiftMas__C0A838E167271AD1");

            entity.ToTable("ShiftMaster", "adminmaster");

            entity.HasIndex(e => new { e.ShiftName, e.CompanyId, e.RegionId }, "UQ_ShiftName_Company_Region").IsUnique();

            entity.Property(e => e.ShiftId).HasColumnName("ShiftID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("userId");
        });

        modelBuilder.Entity<StageMaster>(entity =>
        {
            entity.HasKey(e => e.StageId).HasName("PK__StageMas__03EB7AD8C4A64016");

            entity.ToTable("StageMaster", "Recruitment");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StageName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__States__C3BA3B5A9C204C4B");

            entity.ToTable("States", "adminmaster");

            entity.Property(e => e.StateId).HasColumnName("StateID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.StateName).HasMaxLength(100);
        });

        modelBuilder.Entity<StateMaster>(entity =>
        {
            entity.HasKey(e => e.StateId);

            entity.ToTable("StateMaster", "adminmaster");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.StateName).HasMaxLength(150);
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22B736C45BD2");

            entity.ToTable("SubscriptionPlans", "superadmin");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<SubscriptionPlan1>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22B77D666942");

            entity.ToTable("SubscriptionPlans", "UM");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.PlanName).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.StorageLimitGb).HasColumnName("StorageLimitGB");
        });

        modelBuilder.Entity<SuperadminCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__superadm__2D971CAC794925B5");

            entity.ToTable("superadminCompanies", "superadmin");

            entity.HasIndex(e => e.CompanyCode, "UQ__superadm__11A0134BD808F728").IsUnique();

            entity.Property(e => e.CompanyCode).HasMaxLength(50);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.IndustryType).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__TaskAssi__7C6949B1A54C299D");

            entity.ToTable("TaskAssignments", "mytask");

            entity.Property(e => e.AssignedTo).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.TaskName).HasMaxLength(200);

            entity.HasOne(d => d.Status).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskAssignments_Status");
        });

        modelBuilder.Entity<TaskFile>(entity =>
        {
            entity.HasKey(e => e.TaskFileId).HasName("PK__TaskFile__BA7A8A5326EE85F8");

            entity.ToTable("TaskFile", "mytask");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskFiles)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_TaskFile_Task");
        });

        modelBuilder.Entity<TaskStatus>(entity =>
        {
            entity.HasKey(e => e.TaskStatusId).HasName("PK__TaskStat__C023DD0C2B3AE715");

            entity.ToTable("TaskStatus", "adminmaster");

            entity.Property(e => e.TaskStatusId).HasColumnName("TaskStatusID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TaskStatusName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Company).WithMany(p => p.TaskStatuses)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskStatus_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.TaskStatuses)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskStatus_Region");
        });

        modelBuilder.Entity<TaxSetting>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("PK__TaxSetti__711BE0AC779B74C2");

            entity.ToTable("TaxSettings", "adminmaster");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Rate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TaxName).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.TaxType).WithMany(p => p.TaxSettings)
                .HasForeignKey(d => d.TaxTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaxSettings_TaxType");
        });

        modelBuilder.Entity<TaxType>(entity =>
        {
            entity.HasKey(e => e.TaxTypeId).HasName("PK__TaxType__B5343F43C55B5432");

            entity.ToTable("TaxType", "adminmaster");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TaxTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC607E0A8CE78");

            entity.ToTable("Tickets", "HelpDesk");

            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.TicketNumber).HasMaxLength(20);

            entity.HasOne(d => d.Category).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_HelpDeskCategory");

            entity.HasOne(d => d.Priority).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.PriorityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_Priority");
        });

        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasKey(e => e.TimesheetId).HasName("PK__Timeshee__848CBE2D070FCEC5");

            entity.ToTable("Timesheets", "TS");

            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ManagerUser).WithMany(p => p.TimesheetManagerUsers)
                .HasForeignKey(d => d.ManagerUserId)
                .HasConstraintName("FK_Timesheets_Manager");

            entity.HasOne(d => d.User).WithMany(p => p.TimesheetUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Timesheets_User");
        });

        modelBuilder.Entity<TimesheetApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__Timeshee__328477F4125D50F0");

            entity.ToTable("TimesheetApprovals", "TS");

            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ApproverComments).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.ApproverUser).WithMany(p => p.TimesheetApprovals)
                .HasForeignKey(d => d.ApproverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimesheetApprovals_User");

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetApprovals)
                .HasForeignKey(d => d.TimesheetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TimesheetApprovals_Timesheet");
        });

        modelBuilder.Entity<TimesheetProject>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Timeshee__761ABEF097E4A471");

            entity.ToTable("TimesheetProjects", "TS");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.OthoursText)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("OTHoursText");
            entity.Property(e => e.Otminutes).HasColumnName("OTMinutes");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.TotalHoursText)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Timesheet).WithMany(p => p.TimesheetProjects)
                .HasForeignKey(d => d.TimesheetId)
                .HasConstraintName("FK_TimesheetProjects_Timesheet");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC45456CCE");

            entity.ToTable("Users", "UM");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(50)
                .HasColumnName("company_name");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DemoExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.DemoStartDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentId).HasColumnName("departmentId");
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.LoginType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.Module)
                .HasMaxLength(50)
                .HasColumnName("module");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Passwordchanged).HasColumnName("passwordchanged");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.ReportingHr).HasColumnName("ReportingHR");
            entity.Property(e => e.ReportingTo).HasColumnName("reportingTo");
            entity.Property(e => e.RoleId).HasDefaultValueSql("('Employee')");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserCompanyId).HasColumnName("userCompanyId");
            entity.Property(e => e.Userloginstatus).HasColumnName("userloginstatus");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__UserSubs__9A2B249DED332A97");

            entity.ToTable("UserSubscriptions", "UM");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Plan).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSubsc__PlanI__511AFFBC");
        });

        modelBuilder.Entity<VisaType>(entity =>
        {
            entity.HasKey(e => e.VisaTypeId).HasName("PK__VisaType__9522E679E76617E3");

            entity.ToTable("VisaType", "adminmaster");

            entity.Property(e => e.VisaTypeId).HasColumnName("VisaTypeID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VisaType1)
                .HasMaxLength(100)
                .HasColumnName("VisaType");
        });

        modelBuilder.Entity<VisaTypeMaster>(entity =>
        {
            entity.HasKey(e => e.VisaTypeId).HasName("PK__VisaType__9522E6791B527D40");

            entity.ToTable("VisaTypeMaster", "adminmaster");

            entity.Property(e => e.VisaTypeId).HasColumnName("VisaTypeID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.VisaTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Weekoff>(entity =>
        {
            entity.HasKey(e => e.WeekoffId).HasName("PK__Weekoff__382FA061E3A119CC");

            entity.ToTable("Weekoff", "adminmaster");

            entity.Property(e => e.WeekoffId).HasColumnName("WeekoffID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ModifiedAt).HasColumnType("datetime");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Weekoff1)
                .HasMaxLength(20)
                .HasColumnName("Weekoff");

            entity.HasOne(d => d.Company).WithMany(p => p.Weekoffs)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Weekoff_Company");

            entity.HasOne(d => d.Region).WithMany(p => p.Weekoffs)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Weekoff_Region");
        });

        modelBuilder.Entity<WfhremoteRequest>(entity =>
        {
            entity.HasKey(e => e.WfhrequestId).HasName("PK__WFHRemot__EC572C951339964F");

            entity.ToTable("WFHRemoteRequests", "employee");

            entity.Property(e => e.WfhrequestId).HasColumnName("WFHRequestID");
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DocumentPath).HasMaxLength(500);
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.EmployeeName).HasMaxLength(150);
            entity.Property(e => e.HrEmail).HasMaxLength(200);
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.ManagerRemarks).HasMaxLength(500);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RequestType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkAuthStatusMaster>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__WorkAuth__C8EE204333A84C3A");

            entity.ToTable("WorkAuthStatusMaster", "adminmaster");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(100);
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<DemoUserSubscriptionDto>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("vw_DemoUsersSubscriptionDetails", "UM");

            entity.Property(e => e.UserID)
                .HasColumnName("UserID");

            entity.Property(e => e.Company)
                .HasMaxLength(200);

            entity.Property(e => e.Name)
                .HasMaxLength(200);

            entity.Property(e => e.Email)
                .HasMaxLength(200);

            entity.Property(e => e.Phone)
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.DemoStart)
                .HasColumnType("datetime");

            entity.Property(e => e.DemoExpiry)
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
