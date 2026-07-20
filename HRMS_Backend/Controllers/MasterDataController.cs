using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDataController : ControllerBase
    {
        private readonly IEventTypeService _eventTypeService;
        private readonly IBloodGroupService _bloodGroupService;
        private readonly IDepartmentService _service;
        private readonly IGenderService _genderService;
        private readonly IadminService _adminService;
        private readonly ILogger<MasterDataController> _logger;
        private readonly IDesignationService _designationService;
        private readonly IKpiCategoryService _kpiCategoryService;
        private readonly IEmployeeMasterService _employeeService;
        private readonly ICertificationTypeService _certificationTypeService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IExpenseCategoryService _expensecategoryservice;
        private readonly IAssetStatusService _assetStatusService;
        private readonly IHelpdeskCategoryAdminService _helpdeskCategoryAdminService;
        private readonly IProjectStatusAdminService _projectStatusAdminService;
        private readonly IPriorityService _priorityService;
        private readonly IAttendanceStatusService _attendanceStatusService;
        private readonly IHolidayListService _holidayListService;
        private readonly IWeekoffService _weekoffService;
        private readonly ILeaveStatusService _leaveStatusService;
        private readonly IPolicyCategoryService _policyCategoryService;
        private readonly IResignationService _resignationService;
        private readonly IEventService _Eventservice;
        private readonly IModeOfStudyService _modeOfStudyService;
        private readonly ICompanyNewsPolicyService _companyNewsPolicyService;
        private readonly IRecruitmentNoticePeriodService _recruitmentNoticePeriodService;
        private readonly IScreeningResultService _screeningResultService;
        private readonly IInterviewLevelService _interviewLevelService;
        private readonly ICompanyNewsCategoryService _companyNewsCategoryService;
        private readonly IEmploymentTypeService _employmentTypeService;
        private readonly IGradeService _gradeService;
        private readonly IAssetTypeService _assetTypeService;
        private readonly IAssetCategoryService _assetCategoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IAttachmentTypeService _attachmentTypeService;
        private readonly IVisatypeService _visaTypeService;
        private readonly IAccountTypeService _accountTypeService;
        private readonly ITaskStatusService _taskStatusService;
        private readonly ICountryService _countryService;

        private readonly IProjectMasterService _projectMasterService;
             public MasterDataController(IGradeService GradeService, IEmploymentTypeService employmentTypeService, ICompanyNewsCategoryService companyNewsCategoryService, IRecruitmentNoticePeriodService recruitmentNoticePeriodService, IScreeningResultService screeningResultService, IInterviewLevelService interviewLevelService, ICompanyNewsPolicyService companyNewsPolicyService, IModeOfStudyService modeOfStudyService, IEventService Eventservice, IResignationService resignationService, IPolicyCategoryService policyCategoryService, ILeaveStatusService leaveStatusService, IHolidayListService holidayListService, IWeekoffService weekoffService, IAttendanceStatusService attendanceStatusService, IExpenseCategoryService expenseCategoryservice, IDepartmentService service, IDesignationService designationService, IGenderService genderService, IadminService adminService, ILeaveTypeService leaveTypeService, ILogger<MasterDataController> logger, IKpiCategoryService kpiCategoryService, IEmployeeMasterService employeeService, ICertificationTypeService certificationTypeService, IAssetStatusService assetStatusService, IBloodGroupService bloodGroupService, IHelpdeskCategoryAdminService helpdeskCategoryAdminService, IProjectStatusAdminService projectStatusAdminService, IPriorityService priorityService,
            IAssetTypeService assetTypeService, IAssetCategoryService assetCategoryService, ICurrencyService currencyService, IAttachmentTypeService attachmentTypeService, IVisatypeService visaTypeService, IProjectMasterService projectMasterService, IAccountTypeService accountTypeService
                 ,ITaskStatusService taskStatusService, IEventTypeService eventTypeService, ICountryService countryService )
        {
            _eventTypeService = eventTypeService;
            _taskStatusService = taskStatusService;
            _service = service;
            _Eventservice = Eventservice;
            _designationService = designationService;
            _genderService = genderService;
            _adminService = adminService;
            _logger = logger;
            _expensecategoryservice = expenseCategoryservice;
            _leaveTypeService = leaveTypeService;
            _kpiCategoryService = kpiCategoryService;
            _employeeService = employeeService;
            _certificationTypeService = certificationTypeService;
            _assetStatusService = assetStatusService;
            _bloodGroupService = bloodGroupService;
            _helpdeskCategoryAdminService = helpdeskCategoryAdminService;
            _projectStatusAdminService = projectStatusAdminService;
            _attendanceStatusService = attendanceStatusService;
            _priorityService = priorityService;
            _holidayListService = holidayListService;
            _weekoffService = weekoffService;
            _leaveStatusService = leaveStatusService;
            _policyCategoryService = policyCategoryService;
            _resignationService = resignationService;
            _modeOfStudyService = modeOfStudyService;
            _companyNewsPolicyService = companyNewsPolicyService;
            _recruitmentNoticePeriodService = recruitmentNoticePeriodService;
            _screeningResultService = screeningResultService;
            _interviewLevelService = interviewLevelService;
            _companyNewsCategoryService = companyNewsCategoryService;
            _employmentTypeService = employmentTypeService;
            _gradeService = GradeService;
            _assetTypeService = assetTypeService;
            _assetCategoryService = assetCategoryService;
            _currencyService = currencyService;
            _attachmentTypeService = attachmentTypeService;
            _visaTypeService = visaTypeService;
            _accountTypeService = accountTypeService;
            _projectMasterService = projectMasterService;
            _countryService = countryService;
        }
        #region Task Status

        [HttpGet("taskstatuses")]
        public async Task<IActionResult> GetTaskStatuses([FromQuery] int userId)
        {
            var result = await _taskStatusService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("taskstatuses/{id:int}")]
        public async Task<IActionResult> GetTaskStatusById(int id)
        {
            var result = await _taskStatusService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateTaskStatus")]
        public async Task<IActionResult> CreateTaskStatus([FromBody] TaskStatusDto dto)
        {
            var result = await _taskStatusService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateTaskStatus")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskStatusDto dto)
        {
            var result = await _taskStatusService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteTaskStatus")]
        public async Task<IActionResult> DeleteTaskStatus([FromQuery] int id)
        {
            var result = await _taskStatusService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [HttpGet("taskstatuses/by-company-region")]
        public async Task<IActionResult> GetByCompanyRegion([FromQuery] int companyId, [FromQuery] int regionId)
        {
            var result = await _taskStatusService.GetByCompanyRegion(companyId, regionId);
            return Ok(result);
        }

        #endregion
        //        #region InterviewLevels

        //        [HttpGet("interview-levels")]
        //        public async Task<IActionResult> GetInterviewLevels([FromQuery] int userId)
        //        {
        //            var result = await _interviewLevelService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateInterviewLevel")]
        //        public async Task<IActionResult> CreateInterviewLevel([FromBody] InterviewLevelDto dto)
        //        {
        //            var result = await _interviewLevelService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateInterviewLevel")]
        //        public async Task<IActionResult> UpdateInterviewLevel([FromBody] InterviewLevelDto dto)
        //        {
        //            var result = await _interviewLevelService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteInterviewLevel")]
        //        public async Task<IActionResult> DeleteInterviewLevel([FromQuery] int id)
        //        {
        //            var result = await _interviewLevelService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region ScreeningResult

        //        [HttpGet("screening-result")]
        //        public async Task<IActionResult> GetScreeningResults([FromQuery] int userId)
        //        {
        //            var result = await _screeningResultService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateScreeningResult")]
        //        public async Task<IActionResult> CreateScreeningResult([FromBody] ScreeningResultDto dto)
        //        {
        //            var result = await _screeningResultService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateScreeningResult")]
        //        public async Task<IActionResult> UpdateScreeningResult([FromBody] ScreeningResultDto dto)
        //        {
        //            var result = await _screeningResultService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteScreeningResult")]
        //        public async Task<IActionResult> DeleteScreeningResult([FromQuery] int id)
        //        {
        //            var result = await _screeningResultService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region Department Dropdown

        //        [HttpGet("GetDepartmentsForDropdown")]
        //        public async Task<IActionResult> GetDepartmentsForDropdown(
        //            int companyId,
        //            int regionId)
        //        {
        //            try
        //            {
        //                var result = await _designationService
        //                    .GetDepartmentsForDropdownAsync(companyId, regionId);

        //                if (!result.Success)
        //                    return BadRequest(new
        //                    {
        //                        success = false,
        //                        message = result.Message
        //                    });

        //                return Ok(new
        //                {
        //                    success = true,
        //                    message = result.Message,
        //                    data = result.Data
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error fetching department dropdown.");
        //                return StatusCode(500, new
        //                {
        //                    success = false,
        //                    message = "An unexpected error occurred while loading departments."
        //                });
        //            }
        //        }

        //        #endregion
        //        #region RecruitmentNoticePeriod

        //        [HttpGet("recruitmentnoticeperiod-list")]
        //        public async Task<IActionResult> GetRecruitmentNoticePeriodList([FromQuery] int userId)
        //        {
        //            var result = await _recruitmentNoticePeriodService.GetAllRecruitmentNoticePeriodService(userId);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateRecruitmentNoticePeriod")]
        //        public async Task<IActionResult> CreateRecruitmentNoticePeriod([FromBody] RecruitmentNoticePeriodDto dto)
        //        {
        //            var result = await _recruitmentNoticePeriodService.CreateRecruitmentNoticePeriodServiceAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateRecruitmentNoticePeriod")]
        //        public async Task<IActionResult> UpdateRecruitmentNoticePeriod([FromBody] RecruitmentNoticePeriodDto dto)
        //        {
        //            var result = await _recruitmentNoticePeriodService.UpdateRecruitmentNoticePeriodServiceAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteRecruitmentNoticePeriod")]
        //        public async Task<IActionResult> DeleteRecruitmentNoticePeriod([FromQuery] int id)
        //        {
        //            var result = await _recruitmentNoticePeriodService.DeleteRecruitmentNoticePeriodServiceAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region Departments
        //        // ✅ GET ALL (with optional filters later)
        //        [HttpGet("GetDepartments")]
        //        public async Task<IActionResult> GetDepartments(int userId)
        //        {
        //            try
        //            {
        //                var result = await _service.GetAllAsync(userId);

        //                if (result == null )
        //                    return NotFound(new { success = false, message = "No departments found." });

        //                return Ok(result);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error fetching department list.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching department list." });
        //            }
        //        }

        //        // ✅ GET BY ID
        //        [HttpGet("GetDepartmentsById/{id:int}")]
        //        public async Task<IActionResult> GetById(int id)
        //        {
        //            try
        //            {
        //                var result = await _service.GetByIdAsync(id);
        //                if (result == null)
        //                    return NotFound(new { success = false, message = $"Department with ID {id} not found." });

        //                return Ok(new { success = true, message = "Department details retrieved successfully.", data = result });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error retrieving department with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while fetching department details." });
        //            }
        //        }

        //        // ✅ CREATE
        //        [HttpPost("createDepartment")]
        //        public async Task<IActionResult> Create([FromBody] CreateUpdateDepartmentDto dto)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

        //            try
        //            {
        //                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
        //                var result = await _service.CreateAsync(dto);

        //                if (!result.Success)
        //                    return BadRequest(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error creating new department.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the department." });
        //            }
        //        }

        //        // ✅ UPDATE
        //        [HttpPost("updateDepartment/{id:int}")]
        //        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDepartmentDto dto)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(new { success = false, message = "Invalid input data." });

        //            try
        //            {
        //                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
        //                var result = await _service.UpdateAsync(dto);

        //                if (!result.Success)
        //                    return NotFound(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error updating department with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while updating the department." });
        //            }
        //        }

        //        // ✅ SOFT DELETE
        //        [HttpDelete("deleteDepartment/{id:int}")]
        //        public async Task<IActionResult> SoftDelete(int id)
        //        {
        //            try
        //            {
        //                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
        //                var result = await _service.SoftDeleteAsync(id);

        //                if (!result.Success)
        //                    return NotFound(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error deleting department with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while deleting the department." });
        //            }
        //        }

        //        // ✅ BULK INSERT
        //        [HttpPost("bulk-insert")]
        //        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDepartmentDto> dtos)
        //        {
        //            if (dtos == null || !dtos.Any())
        //                return BadRequest(new { success = false, message = "No records found to upload." });

        //            try
        //            {
        //                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
        //                var result = await _service.BulkInsertAsync(dtos, createdBy);

        //                return Ok(new { success = true, message = result.Message, insertedCount = result.Data });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error during bulk department upload.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
        //            }
        //        }
        //        #endregion
        //        #region Designations
        //        // ✅ GET ALL
        //        [HttpGet("GetDesignations")]
        //        public async Task<IActionResult> GetDesignations(int userId)
        //        {
        //            try
        //            {
        //                var result = await _designationService.GetAllAsync(userId);

        //                if (result == null )
        //                    return NotFound(new { success = false, message = "No designations found." });

        //                return Ok(result);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error fetching designation list.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching designations." });
        //            }
        //        }

        //        // ✅ GET BY ID
        //        [HttpGet("GetDesignationById/{id:int}")]
        //        public async Task<IActionResult> GetDesignationById(int id)
        //        {
        //            try
        //            {
        //                var result = await _designationService.GetByIdAsync(id);
        //                if (result == null)
        //                    return NotFound(new { success = false, message = $"Designation with ID {id} not found." });

        //                return Ok(new { success = true, message = "Designation details retrieved successfully.", data = result });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error retrieving designation with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while fetching designation details." });
        //            }
        //        }

        //        // ✅ CREATE
        //        [HttpPost("CreateDesignation")]
        //        public async Task<IActionResult> Create([FromBody] CreateUpdateDesignationDto dto)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

        //            try
        //            {
        //               // 🔒 TODO: Replace with logged-in user later
        //                var result = await _designationService.CreateAsync(dto);

        //                if (!result.Success)
        //                    return BadRequest(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error creating new designation.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the designation." });
        //            }
        //        }

        //        // ✅ UPDATE
        //        [HttpPost("UpdateDesignation/{id:int}")]
        //        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDesignationDto dto)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(new { success = false, message = "Invalid input data." });

        //            try
        //            {
        //               // 🔒 TODO: Replace with logged-in user later
        //                var result = await _designationService.UpdateAsync(id, dto);

        //                if (!result.Success)
        //                    return NotFound(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error updating designation with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while updating the designation." });
        //            }
        //        }

        //        // ✅ SOFT DELETE
        //        [HttpPost("DeleteDesignation/{id:int}")]
        //        public async Task<IActionResult> DeleteDesignation(int id)
        //        {
        //            try
        //            {
        //                 // 🔒 TODO: Replace with JWT user later
        //                var result = await _designationService.SoftDeleteAsync(id);

        //                if (!result.Success)
        //                    return NotFound(result);

        //                return Ok(new { success = true, message = result.Message });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, $"Error deleting designation with ID {id}.");
        //                return StatusCode(500, new { success = false, message = "An error occurred while deleting the designation." });
        //            }
        //        }
        //        // ✅ BULK INSERT
        //        [HttpPost("DesignationBulkInsert")]
        //        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDesignationDto> dtos)
        //        {
        //            if (dtos == null || !dtos.Any())
        //                return BadRequest(new { success = false, message = "No records found to upload." });

        //            try
        //            {
        //                var createdBy = 1; // TODO: Replace with JWT username later
        //                var result = await _designationService.BulkInsertAsync(dtos, createdBy);

        //                return Ok(new
        //                {
        //                    success = result.Success,
        //                    message = result.Message,
        //                    data = new
        //                    {
        //                        inserted = result.Data.inserted,
        //                        duplicates = result.Data.duplicates,
        //                        failed = result.Data.failed
        //                    }
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Error during bulk designation upload.");
        //                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
        //            }
        //        }

        //        #endregion
        //        #region Gender
        //        /// <summary>
        //        /// Gender Detail Retrieve
        //        /// </summary>
        //        /// <returns></returns>
        //        [HttpGet("GetGenderAll")]
        //        public async Task<IActionResult> GetGenderAll(int companyId,int regionId,int userId)
        //        {
        //            var result = await _genderService.GetAllAsync(companyId, regionId,userId);


        //            if (result==null)
        //                return NotFound("No gender records found.");

        //            return Ok(result);
        //        }
        //        /// <summary>
        //        /// Retrieve Gender details by id
        //        /// </summary>
        //        /// <param name="id"></param>
        //        /// <returns></returns>

        //        [HttpGet("GetGenderById/{id}")]
        //        public async Task<IActionResult> GetGenderById(int id)
        //        {
        //            var gender = await _genderService.GetGenderByIdAsync(id);
        //            if (gender == null) return NotFound("Gender not found");
        //            return Ok(gender);
        //        }
        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="filter"></param>
        //        /// <returns></returns>

        //        [HttpPost("GetGendersearch")]
        //        public async Task<IActionResult> Search([FromBody] object filter)
        //        {
        //            return Ok(await _genderService.SearchGenderAsync(filter));
        //        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="dto"></param>
        //        /// <returns></returns>
        //        [HttpPost("CreateGender")]
        //        public async Task<IActionResult> CreateGender([FromBody] GenderDto dto)
        //        {
        //            var result = await _genderService.AddGenderAsync(dto);
        //            if (result == null)
        //                return Ok(new { message = "Duplicate Record Found" });
        //            return Ok(new { message = "Gender created successfully", data = result });
        //        }

        //        [HttpPost("UpdateGender")]
        //        public async Task<IActionResult> UpdateGender([FromBody] GenderDto dto)
        //        {
        //            var result = await _genderService.UpdateGenderAsync(dto);
        //            if (result == null)
        //                return Ok(new { message = "Duplicate Record Found" });
        //            return Ok(new { message = "Gender updated successfully", data = result });
        //        }

        //        [HttpPost("DeleteGender")]
        //        public async Task<IActionResult> DeleteGender([FromQuery] int id)
        //        {
        //            bool success = await _genderService.DeleteGenderAsync(id);
        //            if (!success) return NotFound("Gender not found");

        //            return Ok(new { message = "Gender deleted successfully" });
        //        }
        //        #endregion
        //        #region KPICategory
        //        // =====================================================
        //        // KPI CATEGORY
        //        // =====================================================

        //        // GET ALL KPI CATEGORIES
        //        [HttpGet("kpi-categoriesbycmp")]
        //        public async Task<IActionResult> GetKpiCategoriescmp(int companyId,int regionId)
        //        {
        //            var result = await _kpiCategoryService.GetAllbycmpreg(companyId,regionId);
        //            return Ok(result);
        //        }
        //        [HttpGet("kpi-categories")]
        //        public async Task<IActionResult> GetKpiCategories(int userId)
        //        {
        //            var result = await _kpiCategoryService.GetAll(userId);
        //            return Ok(result);
        //        }



        //        // GET KPI CATEGORY BY ID
        //        [HttpGet("kpi-categories/{id:int}")]
        //        public async Task<IActionResult> GetKpiCategoryById(int id)
        //        {
        //            var result = await _kpiCategoryService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        // CREATE KPI CATEGORY
        //        [HttpPost("CreateKpiCategory")]
        //        public async Task<IActionResult> CreateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        //        {
        //            var result = await _kpiCategoryService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        // UPDATE KPI CATEGORY
        //        [HttpPost("UpdateKpiCategory")]
        //        public async Task<IActionResult> UpdateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        //        {
        //            var result = await _kpiCategoryService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        // DELETE KPI CATEGORY
        //        [HttpPost("DeleteKpiCategory")]
        //        public async Task<IActionResult> DeleteKpiCategory([FromQuery] int id)
        //        {
        //            var result = await _kpiCategoryService.DeleteAsync(id);
        //            return result.Success ? Ok(result) : NotFound(result);
        //        }
        //        #endregion

        //        #region BloodGroup

        //        #region Get All
        //        [HttpGet("GetAllBloodGroups")]
        //        public async Task<IActionResult> GetAllBloodGroups(int companyId)
        //        {
        //            var result = await _bloodGroupService.GetAllAsync(companyId);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        [HttpGet("GetAllCmpRegAsync")]
        //        public async Task<IActionResult> GetAllCmpRegAsync(int companyId,int regionId)
        //        {
        //            var result = await _bloodGroupService.GetAllCmpRegAsync(companyId,regionId);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        #endregion
        //        [HttpGet("GetAlluserIdAsync")]
        //        public async Task<IActionResult> GetAlluserIdAsync(int userId)
        //        {
        //            var result = await _bloodGroupService.GetAlluserIdAsync(userId);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result.Data);
        //        }

        //        #region Get By Id
        //        [HttpGet("GetBloodGroupsById/{id}")]
        //        public async Task<IActionResult> GetBloodGroupsById(int id)
        //        {
        //            var result = await _bloodGroupService.GetByIdAsync(id);

        //            if (!result.Success)
        //                return NotFound(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Search
        //        [HttpPost("SearchBloodGroups")]
        //        //public async Task<IActionResult> SearchBloodGroups([FromBody] BloodGroupDto filter)
        //        //{
        //        //    var result = await _bloodGroupService
        //        //        .SearchbloodgroupAsync(filter);

        //        //    return Ok(result);
        //        //}
        //        #endregion


        //        #region Add
        //        [HttpPost("AddBloodGroups")]
        //        public async Task<IActionResult> AddBloodGroups([FromBody] BloodGroupDto dto)
        //        {
        //            var result = await _bloodGroupService.CreateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Update
        //        [HttpPut("UpdateBloodGroups")]
        //        public async Task<IActionResult> UpdateBloodGroups(int id,
        //            [FromBody] BloodGroupDto dto)
        //        {
        //            if (id != dto.BloodGroupID)
        //                return BadRequest("ID mismatch");

        //            var result = await _bloodGroupService.UpdateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Delete
        //        [HttpDelete("DeleteBloodGroups/{id}")]
        //        public async Task<IActionResult> Delete(int id)
        //        => Ok(await _bloodGroupService.DeleteAsync(id));
        //        #endregion


        //        #endregion
        //        //---------------------------------Employee Master Details---------------------------------//
        //        #region Employee Master Details


        //        [HttpGet("GetAllEmployees/{userId}")]
        //        public async Task<IActionResult> GetAllEmployees(int userId)
        //        {
        //            var data = await _employeeService.GetAllEmployees(userId);
        //            return Ok(data);
        //        }

        //        // ================= CREATE =================
        //        [HttpPost("CreateEmployee")]
        //        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeMasterDto dto)
        //        {
        //            if (dto.CreatedBy == null || dto.CreatedBy <= 0)
        //                return BadRequest("Invalid user.");

        //            var data = await _employeeService.CreateEmployee(dto);
        //            return Ok(data);
        //        }

        //        // ================= UPDATE =================
        //        [HttpPost("UpdateEmployee/{id}/{userId}")]
        //        public async Task<IActionResult> UpdateEmployee(int id, int userId, [FromBody] EmployeeMasterDto dto)
        //        {
        //            var data = await _employeeService.UpdateEmployee(id, dto, userId);
        //            if (data == null)
        //                return NotFound("Record not found or not authorized.");

        //            return Ok(data);
        //        }

        //        // ================= DELETE =================
        //        [HttpPost("DeleteEmployee/{id}/{userId}")]
        //        public async Task<IActionResult> DeleteEmployee(int id, int userId)
        //        {
        //            var success = await _employeeService.DeleteEmployee(id, userId);
        //            if (!success)
        //                return NotFound("Record not found or not authorized.");

        //            return Ok(new { message = "Deleted successfully" });
        //        }

        //        // ================= MANAGERS =================
        //        [HttpGet("GetManagers/{userId}")]
        //        public async Task<IActionResult> GetManagers(int userId)
        //        {
        //            var data = await _employeeService.GetManagers(userId);
        //            return Ok(data);
        //        }

        //        #endregion

        //        //----------------------MY TEAM SECTION----------------------//
        //        [HttpGet("MyTeam/{managerUserId}")]
        //        public async Task<IActionResult> GetMyTeam(int managerUserId)
        //        {
        //            var tree = await _employeeService.GetMyTeamTreeAsync(managerUserId);
        //            if (tree == null) return NotFound(new { message = "Manager not found" });
        //            return Ok(tree);
        //        }
        //        // ===================== ASSET STATUS =====================

        //        /// <summary>
        //        /// Asset Status CRUD APIs
        //        /// </summary>
        //        [HttpGet("asset-status")]
        //        public async Task<IActionResult> GetAllAssetStatuses(
        //        [FromQuery] int companyId,
        //        [FromQuery] int regionId)
        //        {
        //            var result = await _assetStatusService.GetAllAsync(companyId, regionId);
        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Creates a new asset status
        //        /// </summary>
        //        [HttpPost("asset-status")]
        //        public async Task<IActionResult> CreateAssetStatus([FromBody] AssetStatusDto dto)
        //        {
        //            var id = await _assetStatusService.CreateAsync(dto);
        //            return Ok(id);
        //        }

        //        /// <summary>
        //        /// Updates an existing asset status
        //        /// </summary>
        //        [HttpPut("asset-status/{id}")]
        //        public async Task<IActionResult> UpdateAssetStatus(int id, [FromBody] AssetStatusDto dto)
        //        {
        //            dto.AssetStatusId = id;
        //            var updated = await _assetStatusService.UpdateAsync(dto);
        //            return updated ? Ok() : NotFound();
        //        }


        //        /// <summary>
        //        /// Deletes (soft delete) an asset status
        //        /// </summary>
        //        [HttpDelete("asset-status/{id}")]
        //        public async Task<IActionResult> DeleteAssetStatus(int id)
        //        {
        //            var deleted = await _assetStatusService.DeleteAsync(id);
        //            return deleted ? Ok() : NotFound();
        //        }

        //        #region ===================== CERTIFICATION TYPES =====================

        //        [HttpGet("certification-types")]
        //        public async Task<IActionResult> GetCertificationTypes(
        //            int companyId,
        //            int regionId)
        //        {
        //            var result = await _certificationTypeService
        //                .GetAllAsync(companyId, regionId);

        //            return Ok(result!=null?result.Data:result);
        //        }

        //        [HttpGet("GetCmpregionAllAsync")]
        //        public async Task<IActionResult> GetCmpregionAllAsync(
        //           int companyId,
        //           int regionId)
        //        {
        //            var result = await _certificationTypeService
        //                .GetCmpregionAllAsync(companyId, regionId);

        //            return Ok(result != null ? result.Data : result);
        //        }

        //        [HttpGet("certification-types/{id:int}")]
        //        public async Task<IActionResult> GetCertificationTypeById(int id)
        //        {
        //            var result = await _certificationTypeService.GetByIdAsync(id);

        //            if (!result.Success)
        //                return NotFound(result);

        //            return Ok(result);
        //        }

        //        [HttpPost("CreateCertificationType")]
        //        public async Task<IActionResult> CreateCertificationType(
        //            [FromBody] CreateUpdateCertificationTypeDto dto
        //            )
        //        {
        //            var result = await _certificationTypeService.CreateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateCertificationType")]
        //        public async Task<IActionResult> UpdateCertificationType(

        //            [FromBody] CreateUpdateCertificationTypeDto dto
        //            )
        //        {
        //            var result = await _certificationTypeService
        //                .UpdateAsync( dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteCertificationType")]
        //        public async Task<IActionResult> DeleteCertificationType([FromQuery] int id)
        //        {
        //            var result = await _certificationTypeService.DeleteAsync(id);

        //            if (!result.Success)
        //                return NotFound(result);

        //            return Ok(result);
        //        }

        //        [HttpPost("certification-types/bulk")]
        //        public async Task<IActionResult> BulkInsertCertificationTypes(
        //            [FromBody] IEnumerable<CreateUpdateCertificationTypeDto> dtos,
        //            [FromQuery] int createdBy)
        //        {
        //            var result = await _certificationTypeService
        //                .BulkInsertAsync(dtos, createdBy);

        //            return Ok(result);
        //        }

        //        #endregion
        //        #region LeaveType
        //        [HttpGet("GetLeaveType")]
        //        public async Task<IActionResult> GetLeaveType()
        //        {
        //            // call service without parameters
        //            var data = await _leaveTypeService.GetLeaveTypesAsync();
        //            return Ok(data);
        //        }
        //        [HttpGet("GetLeaveTypesByuserIdAsync")]
        //        public async Task<IActionResult> GetLeaveTypesByuserIdAsync(int userId)
        //        {
        //            // call service without parameters
        //            var data = await _leaveTypeService.GetLeaveTypesByuserIdAsync(userId);
        //            return Ok(data);
        //        }
        //        [HttpGet("GetCRLeaveTypesAsync")]
        //        public async Task<IActionResult> GetCRLeaveTypesAsync(
        //    int companyId,
        //    int regionId)
        //        {
        //            var result = await _leaveTypeService.GetCRLeaveTypesAsync(companyId, regionId);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateLeaveType")]
        //        public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeDto dto)
        //        {
        //            var result = await _leaveTypeService.CreateLeaveTypeAsync(dto);
        //            return result ? Ok() : BadRequest();
        //        }

        //        [HttpPost("UpdateLeaveType")]
        //        public async Task<IActionResult> UpdateLeaveType([FromBody] LeaveTypeDto dto)
        //        {
        //            var result = await _leaveTypeService.UpdateLeaveTypeAsync(dto);
        //            return result ? Ok() : BadRequest();
        //        }

        //        [HttpPost("DeleteLeaveType")]
        //        public async Task<IActionResult> DeleteLeaveType([FromQuery] int id)
        //        {
        //            var result = await _leaveTypeService.DeleteLeaveTypeAsync(id);

        //            if (!result)
        //                return NotFound("Leave Type not found or already deleted");

        //            return Ok(new { message = "Leave Type deleted successfully" });
        //        }



        //        #endregion
        //        #region expenseCategory
        //        [HttpGet("GetexpenseCategoryAll")]
        //        public async Task<IActionResult> GetexpenseCategoryAll(int userId)
        //        {
        //            var result = await _expensecategoryservice.GetAllAsync(userId);
        //            return Ok(result);
        //        }

        //        [HttpPost("AddexpenseCategory")]
        //        public async Task<IActionResult> AddexpenseCategory([FromBody] ExpenseCategoryDto dto)
        //        {
        //            var result = await _expensecategoryservice.AddAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateexpenseCategory")]
        //        public async Task<IActionResult> UpdateexpenseCategory([FromBody] ExpenseCategoryDto dto)
        //        {
        //            var result = await _expensecategoryservice.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteexpenseCategory")]
        //        public async Task<IActionResult> DeleteexpenseCategory([FromQuery] int id)
        //        {
        //            var result = await _expensecategoryservice.DeleteAsync(id);
        //            return Ok(result);
        //        }
        //        #endregion

        //        // ===============================
        //        // GET ALL
        //        // ===============================
        //        [HttpGet("project-status")]
        //        public async Task<IActionResult> GetAllProjects([FromQuery] int userId)
        //        {
        //            var result = await _projectStatusAdminService.GetAllProject(userId);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // GET BY ID
        //        // ===============================
        //        [HttpGet("project-status/{id}")]
        //        public async Task<IActionResult> GetByIdProject(int id)
        //        {
        //            var result = await _projectStatusAdminService.GetByIdProjectAsync(id);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // CREATE
        //        // ===============================
        //        [HttpPost("project-status")]
        //        public async Task<IActionResult> CreateProject([FromBody] ProjectStatusDto dto)
        //        {
        //            var result = await _projectStatusAdminService.CreateProjectAsync(dto);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // UPDATE
        //        // ===============================
        //        [HttpPut("project-status/{id}")]
        //        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectStatusDto dto)
        //        {
        //            dto.ProjectStatusId = id;
        //            var result = await _projectStatusAdminService.UpdateProjectAsync(dto);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // DELETE
        //        // ===============================
        //        [HttpDelete("project-status/{id}")]
        //        public async Task<IActionResult> DeleteProject(int id)
        //        {
        //            var result = await _projectStatusAdminService.DeleteProjectAsync(id);
        //            return result.Success ? Ok(result) : NotFound(result);
        //        }
        //        #region Priority
        //        // =====================================================
        //        // PRIORITY
        //        // =====================================================

        //        [HttpGet("priorities")]
        //        public async Task<IActionResult> GetPriorities([FromQuery] int userId)
        //        {
        //            var result = await _priorityService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("priorities/{id:int}")]
        //        public async Task<IActionResult> GetPriorityById(int id)
        //        {
        //            var result = await _priorityService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreatePriority")]
        //        public async Task<IActionResult> CreatePriority([FromBody] PriorityDto dto)
        //        {
        //            var result = await _priorityService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdatePriority")]
        //        public async Task<IActionResult> UpdatePriority([FromBody] PriorityDto dto)
        //        {
        //            var result = await _priorityService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeletePriority")]
        //        public async Task<IActionResult> DeletePriority([FromQuery] int id)
        //        {
        //            var result = await _priorityService.DeleteAsync(id);
        //            return result.Success ? Ok(result) : NotFound(result);
        //        }

        //        #endregion

        //        #region Helpdesk
        //        // ===============================
        //        // GET ALL
        //        // ===============================
        //        [HttpGet("helpdesk-category")]
        //        public async Task<IActionResult> GetAll([FromQuery] int userId)
        //        {
        //            var result = await _helpdeskCategoryAdminService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // GET BY ID
        //        // ===============================
        //        [HttpGet("helpdesk-category/{id}")]
        //        public async Task<IActionResult> GetByIds(int id)
        //        {
        //            var result = await _helpdeskCategoryAdminService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // CREATE
        //        // ===============================
        //        [HttpPost("helpdesk-category")]
        //        public async Task<IActionResult> Create([FromBody] CreateUpdateHelpdeskCategoryDto dto)
        //        {
        //            var result = await _helpdeskCategoryAdminService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // UPDATE
        //        // ===============================
        //        [HttpPut("helpdesk-category/{id}")]
        //        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateHelpdeskCategoryDto dto)
        //        {
        //            dto.HelpdeskCategoryID = id;
        //            var result = await _helpdeskCategoryAdminService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        // ===============================
        //        // DELETE
        //        // ===============================
        //        [HttpDelete("helpdesk-category/{id}")]
        //        public async Task<IActionResult> helpdeskcategory(int id)
        //        {
        //            var result = await _helpdeskCategoryAdminService.DeleteAsync(id);
        //            return result.Success ? Ok(result) : NotFound(result);
        //        }
        //        #endregion

        //        #region AttendanceStatus

        //        [HttpGet("GetAllAttendanceStatus")]
        //        public async Task<IActionResult> GetAll(int companyId, int regionId)
        //        {
        //            var result = await _attendanceStatusService.GetAllAsync(companyId, regionId);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }

        //        [HttpGet("GetAttendanceStatusById/{id}")]
        //        public async Task<IActionResult> GetAttendanceStatusById(int id)
        //        {
        //            var result = await _attendanceStatusService.GetByIdAsync(id);

        //            if (!result.Success)
        //                return NotFound(result);

        //            return Ok(result);
        //        }

        //        [HttpPost("AddAttendanceStatus")]
        //        public async Task<IActionResult> AddAttendanceStatus(
        //      [FromBody] AttendanceStatusDto dto)
        //        {
        //            var result = await _attendanceStatusService.CreateAsync(dto);

        //            if (!result.Success)
        //                return Conflict(result);   // 409 for duplicate

        //            return CreatedAtAction(
        //                nameof(GetAttendanceStatusById),
        //                new { id = result.Data.AttendanceStatusId },
        //                result);
        //        }

        //        [HttpPut("UpdateAttendanceStatus")]
        //        public async Task<IActionResult> UpdateAttendanceStatus(int id, [FromBody] AttendanceStatusDto dto)
        //        {

        //            var result = await _attendanceStatusService.UpdateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }

        //        [HttpDelete("DeleteAttendanceStatus/{id}")]
        //        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        //        {
        //            return Ok(await _attendanceStatusService.DeleteAsync(id));
        //        }

        //        #endregion

        //        #region Weekoff

        //        [HttpGet("weekoff-list")]
        //        public async Task<IActionResult> GetWeekoffList([FromQuery] int userId)
        //        {
        //            var result = await _weekoffService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("weekoff-list/{id:int}")]
        //        public async Task<IActionResult> GetWeekoffById(int id)
        //        {
        //            var result = await _weekoffService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateWeekoff")]
        //        public async Task<IActionResult> CreateWeekoff([FromBody] WeekoffDto dto)
        //        {
        //            var result = await _weekoffService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateWeekoff")]
        //        public async Task<IActionResult> UpdateWeekoff([FromBody] WeekoffDto dto)
        //        {
        //            var result = await _weekoffService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteWeekoff")]
        //        public async Task<IActionResult> DeleteWeekoff([FromQuery] int id)
        //        {
        //            var result = await _weekoffService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region HolidayList

        //        [HttpGet("holiday-list")]
        //        public async Task<IActionResult> GetHolidayList([FromQuery] int userId)
        //        {
        //            var result = await _holidayListService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("holiday-list/{id:int}")]
        //        public async Task<IActionResult> GetHolidayById(int id)
        //        {
        //            var result = await _holidayListService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateHoliday")]
        //        public async Task<IActionResult> CreateHoliday([FromBody] CreateUpdateHolidayListDto dto)
        //        {
        //            var result = await _holidayListService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateHoliday")]
        //        public async Task<IActionResult> UpdateHoliday([FromBody] CreateUpdateHolidayListDto dto)
        //        {
        //            var result = await _holidayListService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteHoliday")]
        //        public async Task<IActionResult> DeleteHoliday([FromQuery] int id)
        //        {
        //            var result = await _holidayListService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region LeaveStatus

        //        #region Get All
        //        [HttpGet("GetAllLeaveStatus")]
        //        public async Task<IActionResult> GetAllLeaveStatus(int companyId, int regionId)
        //        {

        //            var result = await _leaveStatusService
        //                .GetAllLeaveStatusAsync(companyId, regionId);

        //            if (result == null)
        //                return StatusCode(500, "Service returned null response");

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }

        //        #endregion


        //        #region Get By Id
        //        [HttpGet("GetLeaveStatusById/{id}")]
        //        public async Task<IActionResult> GetLeaveStatusById(int id)
        //        {
        //            var result = await _leaveStatusService
        //                .GetByIdAsync(id);

        //            if (!result.Success)
        //                return NotFound(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Add
        //        [HttpPost("AddLeaveStatus")]
        //        public async Task<IActionResult> AddLeaveStatus(
        //            [FromBody] LeaveStatusDto dto)
        //        {
        //            var result = await _leaveStatusService
        //                .CreateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Update
        //        [HttpPut("UpdateLeaveStatus")]
        //        public async Task<IActionResult> UpdateLeaveStatus(
        //            int id,
        //            [FromBody] LeaveStatusDto dto)
        //        {
        //            if (id != dto.LeaveStatusID)
        //                return BadRequest("ID mismatch");

        //            var result = await _leaveStatusService
        //                .UpdateAsync(dto);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);
        //        }
        //        #endregion


        //        #region Delete
        //        [HttpDelete("DeleteLeaveStatus/{id}")]
        //        public async Task<IActionResult> DeleteLeaveStatus(int id)
        //        {
        //            //return Ok(await _leaveStatusService.DeleteAsync(id));
        //            var result = await _leaveStatusService.DeleteAsync(id);

        //            if (!result.Success)
        //                return BadRequest(result);

        //            return Ok(result);

        //        }
        //        #endregion

        //        #endregion
        //        #region PolicyCategory

        //        [HttpGet("policy-category")]
        //        public async Task<IActionResult> GetPolicyCategories([FromQuery] int userId)
        //        {
        //            var result = await _policyCategoryService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("policy-category/{id:int}")]
        //        public async Task<IActionResult> GetPolicyCategoryById(int id)
        //        {
        //            var result = await _policyCategoryService.GetByIdAsync(id);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreatePolicyCategory")]
        //        public async Task<IActionResult> CreatePolicyCategory([FromBody] CreateUpdatePolicyCategoryDto dto)
        //        {
        //            var result = await _policyCategoryService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdatePolicyCategory")]
        //        public async Task<IActionResult> UpdatePolicyCategory([FromBody] CreateUpdatePolicyCategoryDto dto)
        //        {
        //            var result = await _policyCategoryService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeletePolicyCategory")]
        //        public async Task<IActionResult> DeletePolicyCategory([FromQuery] int id)
        //        {
        //            var result = await _policyCategoryService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        //-------------------------------RESIGNATIONMASTER-------------------------------//

        //        #region Resignations

        //        [HttpGet("GetResignations")]
        //        public IActionResult GetResignations(int companyId, int regionId)
        //        {
        //            var data = _resignationService.GetAll(companyId, regionId);
        //            return Ok(data);
        //        }

        //        [HttpGet("GetResignationById/{id:int}")]
        //        public IActionResult GetResignationById(int id)
        //        {
        //            var data = _resignationService.GetById(id);

        //            if (data == null)
        //                return NotFound();

        //            return Ok(data);
        //        }

        //        [HttpPost("CreateResignation")]
        //        public IActionResult CreateResignation([FromForm] ResignationDto dto, [FromQuery] int userId)
        //        {
        //            var success = _resignationService.Create(dto, userId);
        //            return success ? Ok(new { message = "Created successfully" }) : BadRequest();
        //        }

        //        [HttpPost("UpdateResignation/{id:int}")]
        //        public IActionResult UpdateResignation(int id, [FromForm] ResignationDto dto, [FromQuery] int userId)
        //        {
        //            var success = _resignationService.Update(id, dto, userId);
        //            return success ? Ok(new { message = "Updated successfully" }) : NotFound();
        //        }

        //        [HttpPost("DeleteResignation/{id:int}")]
        //        public IActionResult DeleteResignation(int id, [FromQuery] int userId)
        //        {
        //            var success = _resignationService.Delete(id, userId);
        //            return success ? Ok(new { message = "Deleted successfully" }) : NotFound();
        //        }

        //        #endregion

        //        // ✅ 1️⃣ Get All Events
        //        [HttpGet("GetEventsAll")]
        //        public async Task<IActionResult> GetEventsAll()
        //        {
        //            var events = await _Eventservice.GetAllAsync();
        //            return Ok(events);
        //        }

        //        // ✅ 2️⃣ Get Event By Id
        //        [HttpGet("GetEventById/{id}")]
        //        public async Task<IActionResult> GetEventById(int id)
        //        {
        //            var eventData = await _Eventservice.GetByIdAsync(id);

        //            if (eventData == null)
        //                return NotFound(new { message = "Event not found" });

        //            return Ok(eventData);
        //        }

        //        // ✅ 3️⃣ Create Event
        //        [HttpPost("CreateEvents")]
        //        public async Task<IActionResult> CreateEvents([FromBody] EventDTO dto)
        //        {
        //            if (!ModelState.IsValid)
        //                return BadRequest(ModelState);

        //            var createdEvent = await _Eventservice.CreateAsync(dto);

        //            return CreatedAtAction(
        //                nameof(GetById),
        //                new { id = createdEvent.EventId },
        //                createdEvent);
        //        }

        //        // ✅ 4️⃣ Update Event
        //        [HttpPost("UpdateEvents")]
        //        public async Task<IActionResult> UpdateEvents([FromBody] EventDTO dto)
        //        {

        //            var updatedEvent = await _Eventservice.UpdateAsync(dto);

        //            if (updatedEvent == null)
        //                return NotFound(new { message = "Event not found" });

        //            return Ok(updatedEvent);
        //        }

        //        // ✅ 5️⃣ Delete Event
        //        [HttpPost("DeleteEvents")]
        //        public async Task<IActionResult> DeleteEvents([FromQuery] int id)
        //        {
        //            var deleted = await _Eventservice.DeleteAsync(id);

        //            if (!deleted)
        //                return NotFound(new { message = "Event not found" });

        //            return Ok(new { message = "Event deleted successfully" });
        //        }
        //        #region ModeOfStudy
        //        
        //        #endregion
        //        [HttpGet("birthdays")]
        //        public async Task<IActionResult> GetBirthdays(
        //int companyId,
        //int regionId)
        //        {
        //            var result = await _holidayListService
        //                .GetBirthdaysByCompanyAndRegion(companyId, regionId);

        //            return Ok(result);
        //        }



        //        [HttpGet("Getholidaybycompanyidandregionid")]
        //        public async Task<IActionResult> Getholidaybycompanyidandregionid(int CompanyID, int RegionId)
        //        {
        //            var result = await _holidayListService.Getholidaybycompanyidandregionid(CompanyID, RegionId);
        //            return Ok(result);
        //        }




        //        [HttpGet("Geteventsbycompanyidandregionid")]
        //        public async Task<IActionResult> Geteventsbycompanyidandregionid(int CompanyID, int RegionId)
        //        {
        //            var result = await _holidayListService.Getholidaybycompanyidandregionid(CompanyID, RegionId);
        //            return Ok(result);
        //        }



        //        [HttpGet("approved-by-user/{userId}")]
        //        public async Task<IActionResult> GetApprovedLeavesByUserId(int userId)
        //        {
        //            var result = await _holidayListService.GetApprovedLeavesbyUserid(userId);
        //            return Ok(result);
        //        }
        //        #region Company News APIs

        //        [HttpGet("GetAllNews")]
        //        public async Task<IActionResult> GetAllNews(int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetAllNewsAsync(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("GetTodayNews")]
        //        public async Task<IActionResult> GetTodayNews(int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetTodayNewsAsync(userId);
        //            return Ok(result);
        //        }

        //        [HttpGet("GetNewsById")]
        //        public async Task<IActionResult> GetNewsById(int id, int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetNewsByIdAsync(id, userId);
        //            if (result == null) return NotFound();
        //            return Ok(result);
        //        }

        //        [HttpPost("SaveNews")]
        //        public async Task<IActionResult> SaveNews([FromBody] CompanyNewsMasterDto dto)
        //        {
        //            var result = await _companyNewsPolicyService.AddNewsAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateNews/{id}")]
        //        public async Task<IActionResult> UpdateNews(int id, [FromBody] CompanyNewsMasterDto dto)
        //        {
        //            var result = await _companyNewsPolicyService.UpdateNewsAsync(id, dto);
        //            return Ok(result);
        //        }

        //        [HttpDelete("DeleteNews/{id}")]
        //        public async Task<IActionResult> DeleteNews(int id, int userId)
        //        {
        //            var result = await _companyNewsPolicyService.DeleteNewsAsync(id, userId);
        //            if (!result) return NotFound();
        //            return NoContent();
        //        }

        //        #endregion


        //        #region Company Policies APIs

        //        /// <summary>
        //        /// Get all policies based on UserId
        //        /// </summary>
        //        [HttpGet("GetAllPolicies")]
        //        public async Task<IActionResult> GetAllPolicies(int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetAllPoliciesAsync(userId);
        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Get only today's posted policies based on UserId
        //        /// (Business Logic: PostedDate == Today AND IsActive == true)
        //        /// </summary>
        //        [HttpGet("GetTodayPolicies")]
        //        public async Task<IActionResult> GetTodayPolicies(int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetTodayPoliciesAsync(userId);
        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Get policy by Id & UserId
        //        /// </summary>
        //        [HttpGet("GetPolicyById")]
        //        public async Task<IActionResult> GetPolicyById(int id, int userId)
        //        {
        //            var result = await _companyNewsPolicyService.GetPolicyByIdAsync(id, userId);

        //            if (result == null)
        //                return NotFound();

        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Create new policy
        //        /// </summary>
        //        [HttpPost("SavePolicy")]
        //        public async Task<IActionResult> SavePolicy([FromBody] CompanyPolicyMasterDto dto)
        //        {
        //            var result = await _companyNewsPolicyService.AddPolicyAsync(dto);
        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Update existing policy
        //        /// </summary>
        //        [HttpPost("UpdatePolicy/{id}")]
        //        public async Task<IActionResult> UpdatePolicy(int id, [FromBody] CompanyPolicyMasterDto dto)
        //        {
        //            var result = await _companyNewsPolicyService.UpdatePolicyAsync(id, dto);
        //            return Ok(result);
        //        }

        //        /// <summary>
        //        /// Delete policy based on Id & UserId
        //        /// </summary>
        //        [HttpDelete("DeletePolicy/{id}")]
        //        public async Task<IActionResult> DeletePolicy(int id, int userId)
        //        {
        //            var result = await _companyNewsPolicyService.DeletePolicyAsync(id, userId);

        //            if (!result)
        //                return NotFound();

        //            return NoContent();
        //        }

        //        #endregion

        //        [HttpGet("companynewscategory-list/{userId}")]
        //        public async Task<IActionResult> GetCompanyNewsCategoryList(int userId)
        //        {
        //            var data = await _companyNewsCategoryService.GetAllCompanyNewsCategoryAsync(userId);
        //            return Ok(data);
        //        }

        //        [HttpPost("CreateCompanyNewsCategory")]
        //        public async Task<IActionResult> CreateCompanyNewsCategory([FromBody] CategoryDto dto)
        //        {
        //            var result = await _companyNewsCategoryService.CreateCompanyNewsCategoryAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateCompanyNewsCategory")]
        //        public async Task<IActionResult> UpdateCompanyNewsCategory(CategoryDto dto)
        //        {
        //            var result = await _companyNewsCategoryService.UpdateCompanyNewsCategoryAsync(dto.CategoryId, dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteCompanyNewsCategory")]
        //        public async Task<IActionResult> DeleteCompanyNewsCategory(int id)
        //        {
        //            var result = await _companyNewsCategoryService.DeleteCompanyNewsCategoryAsync(id);
        //            return Ok(result);
        //        }

        //        [HttpGet("companynewscategory-by-company-region")]
        //        public async Task<IActionResult> GetCompanyNewsCategoryByCompanyRegion(int companyId, int regionId)
        //        {
        //            var data = await _companyNewsCategoryService.GetCategoriesByCompanyRegion(companyId, regionId);
        //            return Ok(data);
        //        }
        //        [HttpGet("weekoffCalender")]
        //        public async Task<IActionResult> GetWeekoffs([FromQuery] int companyId, int regionId)
        //        {
        //            var result = await _weekoffService.GetAllWeekOffs(companyId, regionId);
        //            return Ok(result);
        //        }
        //        [HttpGet("holiday-listCalender")]
        //        public async Task<IActionResult> GetHolidayListCalender([FromQuery] int companyId, int regionId)
        //        {
        //            var result = await _holidayListService.GetAllInCalender(companyId, regionId);
        //            return Ok(result);
        //        }
        //        #region EmploymentType

        //        [HttpGet("employment-type")]
        //        public async Task<IActionResult> GetEmploymentTypes([FromQuery] int userId)
        //        {
        //            var result = await _employmentTypeService.GetAll(userId);
        //            return Ok(result);
        //        }

        //        [HttpPost("CreateEmploymentType")]
        //        public async Task<IActionResult> CreateEmploymentType([FromBody] EmploymentTypeDto dto)
        //        {
        //            var result = await _employmentTypeService.CreateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("UpdateEmploymentType")]
        //        public async Task<IActionResult> UpdateEmploymentType([FromBody] EmploymentTypeDto dto)
        //        {
        //            var result = await _employmentTypeService.UpdateAsync(dto);
        //            return Ok(result);
        //        }

        //        [HttpPost("DeleteEmploymentType")]
        //        public async Task<IActionResult> DeleteEmploymentType([FromQuery] int id)
        //        {
        //            var result = await _employmentTypeService.DeleteAsync(id);
        //            return Ok(result);
        //        }

        //        #endregion
        //        #region Grade

        //        [HttpGet("GetGradeAll")]
        //        public async Task<IActionResult> GetGradeAll(int companyId)
        //        {
        //            var result = await _gradeService.GetAllAsync(companyId);
        //            return Ok(result);
        //        }

        //        [HttpGet("GetGradeById/{id}")]
        //        public async Task<IActionResult> GetGradeById(int id)
        //        {
        //            var data = await _gradeService.GetByIdAsync(id);
        //            if (data == null) return NotFound();
        //            return Ok(data);
        //        }

        //        [HttpPost("CreateGrade")]
        //        public async Task<IActionResult> CreateGrade([FromBody] GradeDto dto)
        //        {
        //            var result = await _gradeService.AddAsync(dto);
        //            if (result == null)
        //                return Ok(new { message = "Duplicate Record Found" });

        //            return Ok(new { message = "Grade created", data = result });
        //        }

        //        [HttpPost("UpdateGrade")]
        //        public async Task<IActionResult> UpdateGrade([FromBody] GradeDto dto)
        //        {
        //            var result = await _gradeService.UpdateAsync(dto);
        //            return Ok(new { message = "Grade updated", data = result });
        //        }

        //        [HttpPost("DeleteGrade")]
        //        public async Task<IActionResult> DeleteGrade(int id)
        //        {
        //            var success = await _gradeService.DeleteAsync(id);
        //            if (!success) return NotFound();

        //            return Ok(new { message = "Deleted successfully" });
        //        }

        //        #endregion
        //        //[HttpGet("GetDesignationsbycompanycode")]
        //        //public async Task<IActionResult> GetDesignationsbycompanycode(int companyId, int regionId)
        //        //{
        //        //    var data = await _leaveTypeService.GetDesignationsAsync(companyId, regionId);
        //        //    return Ok(data);
        //        //}
        #region InterviewLevels

        [HttpGet("interview-levels")]
        public async Task<IActionResult> GetInterviewLevels([FromQuery] int userId)
        {
            var result = await _interviewLevelService.GetAll(userId);
            return Ok(result);
        }

        [HttpPost("CreateInterviewLevel")]
        public async Task<IActionResult> CreateInterviewLevel([FromBody] InterviewLevelDto dto)
        {
            var result = await _interviewLevelService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateInterviewLevel")]
        public async Task<IActionResult> UpdateInterviewLevel([FromBody] InterviewLevelDto dto)
        {
            var result = await _interviewLevelService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteInterviewLevel")]
        public async Task<IActionResult> DeleteInterviewLevel([FromQuery] int id)
        {
            var result = await _interviewLevelService.DeleteAsync(id);
            return Ok(result);
        }

        #endregion
        #region ScreeningResult

        [HttpGet("screening-result")]
        public async Task<IActionResult> GetScreeningResults([FromQuery] int userId)
        {
            var result = await _screeningResultService.GetAll(userId);
            return Ok(result);
        }

        [HttpPost("CreateScreeningResult")]
        public async Task<IActionResult> CreateScreeningResult([FromBody] ScreeningResultDto dto)
        {
            var result = await _screeningResultService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateScreeningResult")]
        public async Task<IActionResult> UpdateScreeningResult([FromBody] ScreeningResultDto dto)
        {
            var result = await _screeningResultService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteScreeningResult")]
        public async Task<IActionResult> DeleteScreeningResult([FromQuery] int id)
        {
            var result = await _screeningResultService.DeleteAsync(id);
            return Ok(result);
        }

        #endregion
        #region Department Dropdown

        [HttpGet("GetDepartmentsForDropdown")]
        public async Task<IActionResult> GetDepartmentsForDropdown(
            int companyId,
            int regionId)
        {
            try
            {
                var result = await _designationService
                    .GetDepartmentsForDropdownAsync(companyId, regionId);

                if (!result.Success)
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department dropdown.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while loading departments."
                });
            }
        }

        #endregion
        #region RecruitmentNoticePeriod

        [HttpGet("recruitmentnoticeperiod-list")]
        public async Task<IActionResult> GetRecruitmentNoticePeriodList([FromQuery] int userId)
        {
            var result = await _recruitmentNoticePeriodService.GetAllRecruitmentNoticePeriodService(userId);
            return Ok(result);
        }

        [HttpPost("CreateRecruitmentNoticePeriod")]
        public async Task<IActionResult> CreateRecruitmentNoticePeriod([FromBody] RecruitmentNoticePeriodDto dto)
        {
            var result = await _recruitmentNoticePeriodService.CreateRecruitmentNoticePeriodServiceAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateRecruitmentNoticePeriod")]
        public async Task<IActionResult> UpdateRecruitmentNoticePeriod([FromBody] RecruitmentNoticePeriodDto dto)
        {
            var result = await _recruitmentNoticePeriodService.UpdateRecruitmentNoticePeriodServiceAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteRecruitmentNoticePeriod")]
        public async Task<IActionResult> DeleteRecruitmentNoticePeriod([FromQuery] int id)
        {
            var result = await _recruitmentNoticePeriodService.DeleteRecruitmentNoticePeriodServiceAsync(id);
            return Ok(result);
        }

        #endregion
        #region Departments
        // ✅ GET ALL (with optional filters later)
        [HttpGet("GetDepartments")]
        public async Task<IActionResult> GetDepartments(int userId)
        {
            try
            {
                var result = await _service.GetAllAsync(userId);

                if (result == null)
                    return NotFound(new { success = false, message = "No departments found." });

                return Ok(new { success = true, message = "Departments retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching department list." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDepartmentsById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Department with ID {id} not found." });

                return Ok(new { success = true, message = "Department details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching department details." });
            }
        }

        // ✅ CREATE
        [HttpPost("createDepartment")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.CreateAsync(dto);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new department.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the department." });
            }
        }

        // ✅ UPDATE
        [HttpPost("updateDepartment/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDepartmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                var modifiedBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.UpdateAsync(dto);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating department with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the department." });
            }
        }

        [HttpPost("deleteDepartment/{id:int}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department with ID {id}");
                return StatusCode(500, new { success = false, message = "Server error" });
            }
        }

        // ✅ BULK INSERT
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDepartmentDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = "system"; // 🔒 TODO: Replace with JWT user later
                var result = await _service.BulkInsertAsync(dtos, createdBy);

                return Ok(new { success = true, message = result.Message, insertedCount = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk department upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }
        #endregion
        #region Designations
        // ✅ GET ALL
        [HttpGet("GetDesignations")]
        public async Task<IActionResult> GetDesignations(int userId)
        {
            try
            {
                var result = await _designationService.GetAllAsync(userId);

                if (result == null)
                    return NotFound(new { success = false, message = "No designations found." });

                return Ok(new { success = true, message = "Designations retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching designation list.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while fetching designations." });
            }
        }

        // ✅ GET BY ID
        [HttpGet("GetDesignationById/{id:int}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var result = await _designationService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Designation with ID {id} not found." });

                return Ok(new { success = true, message = "Designation details retrieved successfully.", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while fetching designation details." });
            }
        }

        // ✅ CREATE
        [HttpPost("CreateDesignation")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data. Please check your fields." });

            try
            {
                // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.CreateAsync(dto);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new designation.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred while creating the designation." });
            }
        }

        // ✅ UPDATE
        [HttpPost("UpdateDesignation/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateDesignationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input data." });

            try
            {
                // 🔒 TODO: Replace with logged-in user later
                var result = await _designationService.UpdateAsync(id, dto);

                if (!result.Success)
                    return NotFound(result);

                return Ok(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating designation with ID {id}.");
                return StatusCode(500, new { success = false, message = "An error occurred while updating the designation." });
            }
        }

        // ✅ SOFT DELETE
        [HttpPost("DeleteDesignation/{id:int}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var result = await _designationService.SoftDeleteAsync(id);

                // ❌ Not Found is not correct here
                if (!result.Success)
                    return BadRequest(result); // ✅ FIXED

                return Ok(result); // ✅ return full ApiResponse<bool>
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting designation with ID {id}.");

                return StatusCode(500, new ApiResponse<bool>(
                    false,
                    "An error occurred while deleting the designation.",
                    false));
            }
        }
        // ✅ BULK INSERT
        [HttpPost("DesignationBulkInsert")]
        public async Task<IActionResult> BulkInsert([FromBody] IEnumerable<CreateUpdateDesignationDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest(new { success = false, message = "No records found to upload." });

            try
            {
                var createdBy = 1; // TODO: Replace with JWT username later
                var result = await _designationService.BulkInsertAsync(dtos, createdBy);

                return Ok(new
                {
                    success = result.Success,
                    message = result.Message,
                    data = new
                    {
                        inserted = result.Data.inserted,
                        duplicates = result.Data.duplicates,
                        failed = result.Data.failed
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk designation upload.");
                return StatusCode(500, new { success = false, message = "An unexpected error occurred during bulk upload." });
            }
        }

        #endregion
        #region Gender
        /// <summary>
        /// Gender Detail Retrieve
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetGenderAll")]
        public async Task<IActionResult> GetGenderAll(int companyId, int regionId, int userId)
        {
            var result = await _genderService.GetAllAsync(companyId, regionId, userId);


            if (result == null)
                return NotFound("No gender records found.");

            return Ok(result);
        }
        /// <summary>
        /// Retrieve Gender details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetGenderById/{id}")]
        public async Task<IActionResult> GetGenderById(int id)
        {
            var gender = await _genderService.GetGenderByIdAsync(id);
            if (gender == null) return NotFound("Gender not found");
            return Ok(gender);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>

        [HttpPost("GetGendersearch")]
        public async Task<IActionResult> Search([FromBody] object filter)
        {
            return Ok(await _genderService.SearchGenderAsync(filter));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CreateGender")]
        public async Task<IActionResult> CreateGender([FromBody] GenderDto dto)
        {
            var result = await _genderService.AddGenderAsync(dto);
            if (result == null)
                return Ok(new { message = "Duplicate Record Found" });
            return Ok(new { message = "Gender created successfully", data = result });
        }

        [HttpPost("UpdateGender")]
        public async Task<IActionResult> UpdateGender([FromBody] GenderDto dto)
        {
            var result = await _genderService.UpdateGenderAsync(dto);
            if (result == null)
                return Ok(new { message = "Duplicate Record Found" });
            return Ok(new { message = "Gender updated successfully", data = result });
        }

        [HttpPost("DeleteGender")]
        public async Task<IActionResult> DeleteGender([FromQuery] int id)
        {
            var result = await _genderService.DeleteGenderAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        #endregion
        #region KPICategory
        // =====================================================
        // KPI CATEGORY
        // =====================================================

        // GET ALL KPI CATEGORIES
        [HttpGet("kpi-categoriesbycmp")]
        public async Task<IActionResult> GetKpiCategoriescmp(int companyId, int regionId)
        {
            var result = await _kpiCategoryService.GetAllbycmpreg(companyId, regionId);
            return Ok(result);
        }
        [HttpGet("kpi-categories")]
        public async Task<IActionResult> GetKpiCategories(int userId)
        {
            var result = await _kpiCategoryService.GetAll(userId);
            return Ok(result);
        }



        // GET KPI CATEGORY BY ID
        [HttpGet("kpi-categories/{id:int}")]
        public async Task<IActionResult> GetKpiCategoryById(int id)
        {
            var result = await _kpiCategoryService.GetByIdAsync(id);
            return Ok(result);
        }

        // CREATE KPI CATEGORY
        [HttpPost("CreateKpiCategory")]
        public async Task<IActionResult> CreateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.CreateAsync(dto);
            return Ok(result);
        }

        // UPDATE KPI CATEGORY
        [HttpPost("UpdateKpiCategory")]
        public async Task<IActionResult> UpdateKpiCategory([FromBody] CreateUpdateKpiCategoryDto dto)
        {
            var result = await _kpiCategoryService.UpdateAsync(dto);
            return Ok(result);
        }

        // DELETE KPI CATEGORY
        [HttpPost("DeleteKpiCategory")]
        public async Task<IActionResult> DeleteKpiCategory([FromQuery] int id)
        {
            var result = await _kpiCategoryService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #endregion

        #region EventType

        [HttpGet("GetEventTypeAll")]
        public async Task<IActionResult> GetEventTypeAll(
            int companyId,
            int regionId,
            int userId)
        {
            var result = await _eventTypeService
                .GetAllAsync(companyId, regionId, userId);

            return Ok(result);
        }

        [HttpGet("GetEventTypeById/{id}")]
        public async Task<IActionResult> GetEventTypeById(int id)
        {
            var result = await _eventTypeService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("CreateEventType")]
        public async Task<IActionResult> CreateEventType(
            [FromBody] EventTypeDto dto)
        {
            var result = await _eventTypeService.AddAsync(dto);

            if (result == null)
                return Ok(new
                {
                    message = "Duplicate Record Found"
                });

            return Ok(new
            {
                message = "Event Type created successfully",
                data = result
            });
        }

        [HttpPost("UpdateEventType")]
        public async Task<IActionResult> UpdateEventType(
            [FromBody] EventTypeDto dto)
        {
            var result = await _eventTypeService.UpdateAsync(dto);

            return Ok(new
            {
                message = "Event Type updated successfully",
                data = result
            });
        }

        [HttpPost("DeleteEventType")]
        public async Task<IActionResult> DeleteEventType(
            [FromQuery] int id)
        {
            bool success = await _eventTypeService.DeleteAsync(id);

            if (!success)
                return NotFound();

            return Ok(new
            {
                message = "Event Type deleted successfully"
            });
        }

        #endregion

        #region BloodGroup

        #region Get All
        [HttpGet("GetAllBloodGroups")]
        public async Task<IActionResult> GetAllBloodGroups(int companyId)
        {
            var result = await _bloodGroupService.GetAllAsync(companyId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("GetAllCmpRegAsync")]
        public async Task<IActionResult> GetAllCmpRegAsync(int companyId, int regionId)
        {
            var result = await _bloodGroupService.GetAllCmpRegAsync(companyId, regionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion
        [HttpGet("GetAlluserIdAsync")]
        public async Task<IActionResult> GetAlluserIdAsync(int userId)
        {
            var result = await _bloodGroupService.GetAlluserIdAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        #region Get By Id
        [HttpGet("GetBloodGroupsById/{id}")]
        public async Task<IActionResult> GetBloodGroupsById(int id)
        {
            var result = await _bloodGroupService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        #endregion


        #region Search
        [HttpPost("SearchBloodGroups")]
        //public async Task<IActionResult> SearchBloodGroups([FromBody] BloodGroupDto filter)
        //{
        //    var result = await _bloodGroupService
        //        .SearchbloodgroupAsync(filter);

        //    return Ok(result);
        //}
        #endregion


        #region Add
        [HttpPost("AddBloodGroups")]
        public async Task<IActionResult> AddBloodGroups([FromBody] BloodGroupDto dto)
        {
            var result = await _bloodGroupService.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion


        #region Update
        [HttpPut("UpdateBloodGroups")]
        public async Task<IActionResult> UpdateBloodGroups(int id,
            [FromBody] BloodGroupDto dto)
        {
            if (id != dto.BloodGroupID)
                return BadRequest("ID mismatch");

            var result = await _bloodGroupService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion


        #region Delete
        [HttpDelete("DeleteBloodGroups/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bloodGroupService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion


        #endregion
        //---------------------------------Employee Master Details---------------------------------//
        #region Employee Master Details


        [HttpGet("GetAllEmployees/{userId}")]
        public async Task<IActionResult> GetAllEmployees(int userId)
        {
            var data = await _employeeService.GetAllEmployees(userId);
            return Ok(data);
        }

        // ================= CREATE =================
        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeMasterDto dto)
        {
            if (dto.CreatedBy == null || dto.CreatedBy <= 0)
                return BadRequest("Invalid user.");

            var data = await _employeeService.CreateEmployee(dto);
            return Ok(data);
        }

        // ================= UPDATE =================
        [HttpPost("UpdateEmployee/{id}/{userId}")]
        public async Task<IActionResult> UpdateEmployee(int id, int userId, [FromBody] EmployeeMasterDto dto)
        {
            var data = await _employeeService.UpdateEmployee(id, dto, userId);
            if (data == null)
                return NotFound("Record not found or not authorized.");

            return Ok(data);
        }

        // ================= DELETE =================
        [HttpPost("DeleteEmployee/{id}/{userId}")]
        public async Task<IActionResult> DeleteEmployee(int id, int userId)
        {
            var success = await _employeeService.DeleteEmployee(id, userId);
            if (!success)
                return NotFound("Record not found or not authorized.");

            return Ok(new { message = "Deleted successfully" });
        }

        // ================= MANAGERS =================
        [HttpGet("GetManagers/{userId}")]
        public async Task<IActionResult> GetManagers(int userId)
        {
            var data = await _employeeService.GetManagers(userId);
            return Ok(data);
        }

        #endregion

        //----------------------MY TEAM SECTION----------------------//
        [HttpGet("MyTeam/{managerUserId}")]
        public async Task<IActionResult> GetMyTeam(int managerUserId)
        {
            var tree = await _employeeService.GetMyTeamTreeAsync(managerUserId);
            if (tree == null) return NotFound(new { message = "Manager not found" });
            return Ok(tree);
        }
        [HttpGet("asset-status")]
        public async Task<IActionResult> GetAllAssetStatus(int userId)
        {
            return Ok(await _assetStatusService.GetAll(userId));
        }

        [HttpPost("CreateAssetStatus")]
        public async Task<IActionResult> CreateAsssetStatus(AssetStatusDto dto)
        {
            return Ok(await _assetStatusService.CreateAsync(dto));
        }

        [HttpPost("UpdateAssetStatus")]
        public async Task<IActionResult> UpdateAsssetStatus(AssetStatusDto dto)
        {
            return Ok(await _assetStatusService.UpdateAsync(dto));
        }

        [HttpPost("DeleteAssetStatus")]
        public async Task<IActionResult> DeleteAsssetStatus(int id)
        {
            var result = await _assetStatusService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        #region CertificationType

        [HttpGet("certification-type-list")]
        public async Task<IActionResult> GetCertificationTypes([FromQuery] int userId)
        {
            var result = await _certificationTypeService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("certification-type/{id:int}")]
        public async Task<IActionResult> GetCertificationTypeById(int id)
        {
            var result = await _certificationTypeService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateCertificationType")]
        public async Task<IActionResult> CreateCertificationType([FromBody] CreateUpdateCertificationTypeDto dto)
        {
            var result = await _certificationTypeService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateCertificationType")]
        public async Task<IActionResult> UpdateCertificationType([FromBody] CreateUpdateCertificationTypeDto dto)
        {
            var result = await _certificationTypeService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteCertificationType")]
        public async Task<IActionResult> DeleteCertificationType([FromQuery] int id)
        {
            var result = await _certificationTypeService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("GetCmpregionAllAsync")]
        public async Task<IActionResult> GetCmpregionAllAsync(
    [FromQuery] int companyId,
    [FromQuery] int regionId)
        {
            var result = await _certificationTypeService.GetCmpregionAllAsync(companyId, regionId);
            return Ok(result);
        }


        #endregion

        #region LeaveType
        [HttpGet("GetLeaveType")]
        public async Task<IActionResult> GetLeaveType()
        {
            // call service without parameters
            var data = await _leaveTypeService.GetLeaveTypesAsync();
            return Ok(data);
        }
        [HttpGet("GetLeaveTypesByuserIdAsync")]
        public async Task<IActionResult> GetLeaveTypesByuserIdAsync(int userId)
        {
            // call service without parameters
            var data = await _leaveTypeService.GetLeaveTypesByuserIdAsync(userId);
            return Ok(data);
        }
        [HttpGet("GetCRLeaveTypesAsync")]
        public async Task<IActionResult> GetCRLeaveTypesAsync(
    int companyId,
    int regionId)
        {
            var result = await _leaveTypeService.GetCRLeaveTypesAsync(companyId, regionId);
            return Ok(result);
        }

        [HttpPost("CreateLeaveType")]
        public async Task<IActionResult> CreateLeaveType([FromBody] LeaveTypeDto dto)
        {
            try
            {
                var result = await _leaveTypeService.CreateLeaveTypeAsync(dto);

                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to create Leave Type."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Leave Type created successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("UpdateLeaveType")]
        public async Task<IActionResult> UpdateLeaveType([FromBody] LeaveTypeDto dto)
        {
            try
            {
                var result = await _leaveTypeService.UpdateLeaveTypeAsync(dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Leave Type not found."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Updated successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("DeleteLeaveType")]
        public async Task<IActionResult> DeleteLeaveType([FromQuery] int id)
        {
            var result = await _leaveTypeService.DeleteLeaveTypeAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }



        #endregion
        #region expenseCategory
        [HttpGet("GetexpenseCategoryAll")]
        public async Task<IActionResult> GetexpenseCategoryAll(int userId)
        {
            var result = await _expensecategoryservice.GetAllAsync(userId);
            return Ok(result);
        }

        [HttpPost("AddexpenseCategory")]
        public async Task<IActionResult> AddexpenseCategory([FromBody] ExpenseCategoryDto dto)
        {
            var result = await _expensecategoryservice.AddAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateexpenseCategory")]
        public async Task<IActionResult> UpdateexpenseCategory([FromBody] ExpenseCategoryDto dto)
        {
            var result = await _expensecategoryservice.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteexpenseCategory")]
        public async Task<IActionResult> DeleteexpenseCategory([FromQuery] int id)
        {
            var result = await _expensecategoryservice.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion

        // ===============================
        // GET ALL
        // ===============================
        [HttpGet("project-status")]
        public async Task<IActionResult> GetAllProjects([FromQuery] int userId)
        {
            var result = await _projectStatusAdminService.GetAllProject(userId);
            return Ok(result);
        }

        // ===============================
        // GET BY ID
        // ===============================
        [HttpGet("project-status/{id}")]
        public async Task<IActionResult> GetByIdProject(int id)
        {
            var result = await _projectStatusAdminService.GetByIdProjectAsync(id);
            return Ok(result);
        }

        // ===============================
        // CREATE
        // ===============================
        [HttpPost("project-status")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectStatusDto dto)
        {
            var result = await _projectStatusAdminService.CreateProjectAsync(dto);
            return Ok(result);
        }

        // ===============================
        // UPDATE
        // ===============================
        [HttpPost("UpdateProject/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectStatusDto dto)
        {
            dto.ProjectStatusId = id;
            var result = await _projectStatusAdminService.UpdateProjectAsync(dto);
            return Ok(result);
        }

        // ===============================
        // DELETE
        // ===============================
        [HttpPost("DeleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectStatusAdminService.DeleteProjectAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #region Priority
        // =====================================================
        // PRIORITY
        // =====================================================

        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities([FromQuery] int userId)
        {
            var result = await _priorityService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("priorities/{id:int}")]
        public async Task<IActionResult> GetPriorityById(int id)
        {
            var result = await _priorityService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreatePriority")]
        public async Task<IActionResult> CreatePriority([FromBody] PriorityDto dto)
        {
            var result = await _priorityService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdatePriority")]
        public async Task<IActionResult> UpdatePriority([FromBody] PriorityDto dto)
        {
            var result = await _priorityService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeletePriority")]
        public async Task<IActionResult> DeletePriority([FromQuery] int id)
        {
            var result = await _priorityService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        #endregion

        #region Helpdesk
        // ===============================
        // GET ALL
        // ===============================
        [HttpGet("helpdesk-category")]
        public async Task<IActionResult> GetAll([FromQuery] int userId)
        {
            var result = await _helpdeskCategoryAdminService.GetAll(userId);
            return Ok(result);
        }

        // ===============================
        // GET BY ID
        // ===============================
        [HttpGet("helpdesk-category/{id}")]
        public async Task<IActionResult> GetByIds(int id)
        {
            var result = await _helpdeskCategoryAdminService.GetByIdAsync(id);
            return Ok(result);
        }

        // ===============================
        // CREATE
        // ===============================
        [HttpPost("helpdesk-category")]
        public async Task<IActionResult> Create([FromBody] CreateUpdateHelpdeskCategoryDto dto)
        {
            var result = await _helpdeskCategoryAdminService.CreateAsync(dto);
            return Ok(result);
        }

        // ===============================
        // UPDATE
        // ===============================
        [HttpPost("Updatehelpdeskcategory/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateHelpdeskCategoryDto dto)
        {
            dto.HelpdeskCategoryID = id;
            var result = await _helpdeskCategoryAdminService.UpdateAsync(dto);
            return Ok(result);
        }

        // ===============================
        // DELETE
        // ===============================
        [HttpPost("Deletehelpdeskcategory/{id}")]
        public async Task<IActionResult> helpdeskcategory(int id)
        {
            var result = await _helpdeskCategoryAdminService.DeleteAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        #endregion

        #region AttendanceStatus

        [HttpGet("GetAllAttendanceStatus")]
        public async Task<IActionResult> GetAllAttendanceStatus(int userId)
        {
            var result = await _attendanceStatusService.GetAllAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetAttendanceStatusById/{id}")]
        public async Task<IActionResult> GetAttendanceStatusById(int id)
        {
            var result = await _attendanceStatusService.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("AddAttendanceStatus")]
        public async Task<IActionResult> AddAttendanceStatus(
      [FromBody] AttendanceStatusDto dto)
        {
            var result = await _attendanceStatusService.CreateAsync(dto);

            if (!result.Success)
                return Conflict(result);   // 409 for duplicate

            return CreatedAtAction(
                nameof(GetAttendanceStatusById),
                new { id = result.Data.AttendanceStatusId },
                result);
        }

        [HttpPost("UpdateAttendanceStatus")]
        public async Task<IActionResult> UpdateAttendanceStatus([FromBody] AttendanceStatusDto dto)
        {

            var result = await _attendanceStatusService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("DeleteAttendanceStatus/{id}")]
        public async Task<IActionResult> DeleteAttendanceStatus(int id)
        {
            return Ok(await _attendanceStatusService.DeleteAsync(id));
        }

        #endregion

        #region Weekoff

        [HttpGet("weekoff-list")]
        public async Task<IActionResult> GetWeekoffList([FromQuery] int userId)
        {
            var result = await _weekoffService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("weekoff-list/{id:int}")]
        public async Task<IActionResult> GetWeekoffById(int id)
        {
            var result = await _weekoffService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateWeekoff")]
        public async Task<IActionResult> CreateWeekoff([FromBody] WeekoffDto dto)
        {
            var result = await _weekoffService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateWeekoff")]
        public async Task<IActionResult> UpdateWeekoff([FromBody] WeekoffDto dto)
        {
            var result = await _weekoffService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteWeekoff")]
        public async Task<IActionResult> DeleteWeekoff([FromQuery] int id)
        {
            var result = await _weekoffService.DeleteAsync(id);
            return Ok(result);
        }

        #endregion
        #region HolidayList

        [HttpGet("holiday-list")]
        public async Task<IActionResult> GetHolidayList([FromQuery] int userId)
        {
            var result = await _holidayListService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("holiday-list/{id:int}")]
        public async Task<IActionResult> GetHolidayById(int id)
        {
            var result = await _holidayListService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateHoliday")]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateUpdateHolidayListDto dto)
        {
            var result = await _holidayListService.CreateAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("UpdateHoliday")]
        public async Task<IActionResult> UpdateHoliday([FromBody] CreateUpdateHolidayListDto dto)
        {
            var result = await _holidayListService.UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("DeleteHoliday")]
        public async Task<IActionResult> DeleteHoliday([FromQuery] int id)
        {
            var result = await _holidayListService.DeleteAsync(id);
            return Ok(result);
        }

        #endregion
        #region LeaveStatus

        #region Get All
        [HttpGet("GetAllLeaveStatus")]
        public async Task<IActionResult> GetAllLeaveStatus(int companyId, int regionId)
        {

            var result = await _leaveStatusService
                .GetAllLeaveStatusAsync(companyId, regionId);

            if (result == null)
                return StatusCode(500, "Service returned null response");

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion


        #region Get By Id
        [HttpGet("GetLeaveStatusById/{id}")]
        public async Task<IActionResult> GetLeaveStatusById(int id)
        {
            var result = await _leaveStatusService
                .GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        #endregion


        #region Add
        [HttpPost("AddLeaveStatus")]
        public async Task<IActionResult> AddLeaveStatus(
            [FromBody] LeaveStatusDto dto)
        {
            var result = await _leaveStatusService
                .CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion


        #region Update
        [HttpPut("UpdateLeaveStatus")]
        public async Task<IActionResult> UpdateLeaveStatus(
            int id,
            [FromBody] LeaveStatusDto dto)
        {
            if (id != dto.LeaveStatusID)
                return BadRequest("ID mismatch");

            var result = await _leaveStatusService
                .UpdateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion


        #region Delete
        [HttpDelete("DeleteLeaveStatus/{id}")]
        public async Task<IActionResult> DeleteLeaveStatus(int id)
        {
            //return Ok(await _leaveStatusService.DeleteAsync(id));
            var result = await _leaveStatusService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);

        }
        #endregion

        #endregion
        #region PolicyCategory

        [HttpGet("policy-category")]
        public async Task<IActionResult> GetPolicyCategories([FromQuery] int userId)
        {
            var result = await _policyCategoryService.GetAll(userId);
            return Ok(result);
        }
        [HttpGet("policy/company-region")]
        public async Task<IActionResult> GetByCompanyAndRegion(
            [FromQuery] int companyId,
            [FromQuery] int regionId)
        {
            var result = await _policyCategoryService
                .GetByCompanyAndRegion(companyId, regionId);

            return Ok(result);
        }

        [HttpGet("policy-category/{id:int}")]
        public async Task<IActionResult> GetPolicyCategoryById(int id)
        {
            var result = await _policyCategoryService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreatePolicyCategory")]
        public async Task<IActionResult> CreatePolicyCategory([FromBody] CreateUpdatePolicyCategoryDto dto)
        {
            var result = await _policyCategoryService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdatePolicyCategory")]
        public async Task<IActionResult> UpdatePolicyCategory([FromBody] CreateUpdatePolicyCategoryDto dto)
        {
            var result = await _policyCategoryService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeletePolicyCategory/{id}")]
        public async Task<IActionResult> DeletePolicyCategory(int id)
        {
            var result = await _policyCategoryService.DeleteAsync(id);
            return Ok(result);
        }
        [HttpGet("policy-category/company-region")]
        public async Task<IActionResult> GetByCompanyRegionPolicyCategory(
    [FromQuery] int companyId,
    [FromQuery] int regionId)
        {
            var result = await _policyCategoryService
                .GetByCompanyRegion(companyId, regionId);

            return Ok(result);
        }
        #endregion
        //-------------------------------RESIGNATIONMASTER-------------------------------//

        #region Resignations
        [HttpGet("GetAllResignations/{userId:int}")]
        public IActionResult GetAllResignations(int userId)
        {
            var data = _resignationService.GetAllResignations(userId);
            return Ok(data);
        }
        [HttpGet("GetResignations")]
        public IActionResult GetResignations(int companyId, int regionId)
        {
            var data = _resignationService.GetAll(companyId, regionId);
            return Ok(data);
        }

        [HttpGet("GetResignationById/{id:int}")]
        public IActionResult GetResignationById(int id)
        {
            var data = _resignationService.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost("CreateResignation")]
        public IActionResult CreateResignation([FromForm] ResignationDto dto, [FromQuery] int userId)
        {
            var success = _resignationService.Create(dto, userId);
            //if (!success)
            //    return BadRequest(new { message = "Resignation type already exists!" });

            if (!success)
                return BadRequest(new
                {
                    message = "Resignation Type already exists for the selected Company and Region."
                });

            return Ok(new { message = "Created successfully" });

        }

        [HttpPost("UpdateResignation/{id:int}")]
        public IActionResult UpdateResignation(int id, [FromForm] ResignationDto dto, [FromQuery] int userId)
        {
            var success = _resignationService.Update(id, dto, userId);

            if (!success)
                return BadRequest(new { message = "Resignation type already exists!" });

            return Ok(new { message = "Updated successfully" });
        }

        [HttpPost("DeleteResignation/{id:int}")]
        public IActionResult DeleteResignation(int id, [FromQuery] int userId)
        {
            var success = _resignationService.Delete(id, userId);
            return success ? Ok(new { message = "Deleted successfully" }) : NotFound();
        }

        #endregion

        // ✅ 1️⃣ Get All Events
        [HttpGet("GetEventsAll")]
        public async Task<IActionResult> GetEventsAll()
        {
            var events = await _Eventservice.GetAllAsync();
            return Ok(events);
        }

        // ✅ 2️⃣ Get Event By Id
        [HttpGet("GetEventById/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventData = await _Eventservice.GetByIdAsync(id);

            if (eventData == null)
                return NotFound(new { message = "Event not found" });

            return Ok(eventData);
        }

        // ✅ 3️⃣ Create Event
        [HttpPost("CreateEvents")]
        public async Task<IActionResult> CreateEvents([FromBody] EventDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEvent = await _Eventservice.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdEvent.EventId },
                createdEvent);
        }

        // ✅ 4️⃣ Update Event
        [HttpPost("UpdateEvents")]
        public async Task<IActionResult> UpdateEvents([FromBody] EventDTO dto)
        {

            var updatedEvent = await _Eventservice.UpdateAsync(dto);

            if (updatedEvent == null)
                return NotFound(new { message = "Event not found" });

            return Ok(updatedEvent);
        }

        // ✅ 5️⃣ Delete Event
        [HttpPost("DeleteEvents")]
        public async Task<IActionResult> DeleteEvents([FromQuery] int id)
        {
            var deleted = await _Eventservice.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Event not found" });

            return Ok(new { message = "Event deleted successfully" });
        }
        #region ModeOfStudy
        [HttpGet("GetAllModeOfStudy")]
        public async Task<IActionResult> GetAllModeOfStudy([FromQuery] int userId)
        {
            var data = await _modeOfStudyService.GetAllModeOfStudytAsync(userId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetModeOfStudy(int id)
        {
            var data = await _modeOfStudyService.GetByIdModeOfStudytAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateModeOfStudy")]
        public async Task<IActionResult> CreateModeOfStudy([FromBody] ModeOfStudyDto dto)
        {
            try
            {
                var result = await _modeOfStudyService.CreateModeOfStudytAsync(dto);

                return result
                    ? Ok(new { success = true })
                    : BadRequest("Creation Failed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPut("UpdateModeOfStudy")]
        public async Task<IActionResult> UpdateModeOfStudy([FromBody] ModeOfStudyDto dto)
        {
            var result = await _modeOfStudyService.UpdateModeOfStudytAsync(dto);

            return result
                ? Ok(new { success = true, message = "Updated Successfully" })
                : NotFound(new { success = false, message = "Update Failed" });
        }

        [HttpPost("DeleteModeOfStudy")]
        public async Task<IActionResult> DeleteModeOfStudy([FromQuery] int id)
        {
            var result = await _modeOfStudyService.DeleteModeOfStudytAsync(id);

            return result
                ? Ok(new { success = true, message = "Deleted Successfully" })
                : NotFound(new { success = false, message = "Delete Failed" });
        }
        #endregion
        [HttpGet("birthdays")]
        public async Task<IActionResult> GetBirthdays(
int companyId,
int regionId)
        {
            var result = await _holidayListService
                .GetBirthdaysByCompanyAndRegion(companyId, regionId);

            return Ok(result);
        }



        [HttpGet("Getholidaybycompanyidandregionid")]
        public async Task<IActionResult> Getholidaybycompanyidandregionid(int CompanyID, int RegionId)
        {
            var result = await _holidayListService.Getholidaybycompanyidandregionid(CompanyID, RegionId);
            return Ok(result);
        }




        [HttpGet("Geteventsbycompanyidandregionid")]
        public async Task<IActionResult> Geteventsbycompanyidandregionid(int CompanyID, int RegionId)
        {
            var result = await _holidayListService.Getholidaybycompanyidandregionid(CompanyID, RegionId);
            return Ok(result);
        }



        [HttpGet("approved-by-user/{userId}")]
        public async Task<IActionResult> GetApprovedLeavesByUserId(int userId)
        {
            var result = await _holidayListService.GetApprovedLeavesbyUserid(userId);
            return Ok(result);
        }
        #region Company News APIs

        [HttpGet("GetAllNews")]
        public async Task<IActionResult> GetAllNews(int userId)
        {
            var result = await _companyNewsPolicyService.GetAllNewsAsync(userId);
            return Ok(result);
        }

        //[HttpGet("GetTodayNews")]
        //public async Task<IActionResult> GetTodayNews(int userId)
        //{
        //    var result = await _companyNewsPolicyService.GetTodayNewsAsync(userId);
        //    return Ok(result);
        //}

        [HttpGet("GetTodayNews")]
        public async Task<IActionResult> GetTodayNews(int companyId, int regionId)
        {
            var result = await _companyNewsPolicyService.GetTodayNewsAsync(companyId, regionId);
            return Ok(result);
        }

        [HttpGet("GetNewsById")]
        public async Task<IActionResult> GetNewsById(int id, int userId)
        {
            var result = await _companyNewsPolicyService.GetNewsByIdAsync(id, userId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("SaveNews")]
        public async Task<IActionResult> SaveNews([FromForm] CompanyNewsMasterDto dto)
        {
            var result = await _companyNewsPolicyService.AddNewsAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateNews/{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromForm] CompanyNewsMasterDto dto)
        {
            var result = await _companyNewsPolicyService.UpdateNewsAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("DeleteNews/{id}")]
        public async Task<IActionResult> DeleteNews(int id, int userId)
        {
            var result = await _companyNewsPolicyService.DeleteNewsAsync(id, userId);
            if (!result) return NotFound();
            return NoContent();
        }

        #endregion


        #region Company Policies APIs

        /// <summary>
        /// Get all policies based on UserId
        /// </summary>
        [HttpGet("GetAllPolicies")]
        public async Task<IActionResult> GetAllPolicies(int userId)
        {
            var result = await _companyNewsPolicyService.GetAllPoliciesAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Get only today's posted policies based on UserId
        /// (Business Logic: PostedDate == Today AND IsActive == true)
        /// </summary>
        //[HttpGet("GetTodayPolicies")]
        //public async Task<IActionResult> GetTodayPolicies(int userId)
        //{
        //    var result = await _companyNewsPolicyService.GetTodayPoliciesAsync(userId);
        //    return Ok(result);
        //}


        [HttpGet("GetTodayPolicies")]
        public async Task<IActionResult> GetTodayPolicies(int companyId, int regionId, int UserId)
        {
            var result = await _companyNewsPolicyService
                .GetTodayPoliciesAsync(companyId, regionId, UserId);

            return Ok(result);
        }

        /// <summary>
        /// Get policy by Id & UserId
        /// </summary>
        [HttpGet("GetPolicyById")]
        public async Task<IActionResult> GetPolicyById(int id, int userId)
        {
            var result = await _companyNewsPolicyService.GetPolicyByIdAsync(id, userId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create new policy
        /// </summary>
        [HttpPost("SavePolicy")]
        public async Task<IActionResult> SavePolicy([FromBody] CompanyPolicyMasterDto dto)
        {
            var result = await _companyNewsPolicyService.AddPolicyAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Update existing policy
        /// </summary>
        [HttpPost("UpdatePolicy/{id}")]
        public async Task<IActionResult> UpdatePolicy(int id, [FromBody] CompanyPolicyMasterDto dto)
        {
            var result = await _companyNewsPolicyService.UpdatePolicyAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete policy based on Id & UserId
        /// </summary>
        [HttpDelete("DeletePolicy/{id}")]
        public async Task<IActionResult> DeletePolicy(int id, int userId)
        {
            var result = await _companyNewsPolicyService.DeletePolicyAsync(id, userId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        #endregion

        [HttpGet("companynewscategory-list/{userId}")]
        public async Task<IActionResult> GetCompanyNewsCategoryList(int userId)
        {
            var data = await _companyNewsCategoryService.GetAllCompanyNewsCategoryAsync(userId);
            return Ok(data);
        }

        [HttpPost("CreateCompanyNewsCategory")]
        public async Task<IActionResult> CreateCompanyNewsCategory([FromBody] CategoryDto dto)
        {
            var result = await _companyNewsCategoryService.CreateCompanyNewsCategoryAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateCompanyNewsCategory")]
        public async Task<IActionResult> UpdateCompanyNewsCategory(CategoryDto dto)
        {
            var result = await _companyNewsCategoryService.UpdateCompanyNewsCategoryAsync(dto.CategoryId, dto);
            return Ok(result);
        }

        [HttpPost("DeleteCompanyNewsCategory")]
        public async Task<IActionResult> DeleteCompanyNewsCategory(int id)
        {
            var result = await _companyNewsCategoryService.DeleteCompanyNewsCategoryAsync(id);
            return Ok(result);
        }

        [HttpGet("companynewscategory-by-company-region")]
        public async Task<IActionResult> GetCompanyNewsCategoryByCompanyRegion(int companyId, int regionId)
        {
            var data = await _companyNewsCategoryService.GetCategoriesByCompanyRegion(companyId, regionId);
            return Ok(data);
        }
        [HttpGet("weekoffCalender")]
        public async Task<IActionResult> GetWeekoffs([FromQuery] int companyId, int regionId)
        {
            var result = await _weekoffService.GetAllWeekOffs(companyId, regionId);
            return Ok(result);
        }
        [HttpGet("holiday-listCalender")]
        public async Task<IActionResult> GetHolidayListCalender([FromQuery] int companyId, int regionId)
        {
            var result = await _holidayListService.GetAllInCalender(companyId, regionId);
            return Ok(result);
        }
        #region EmploymentType

        [HttpGet("employment-type")]
        public async Task<IActionResult> GetEmploymentTypes([FromQuery] int userId)
        {
            var result = await _employmentTypeService.GetAll(userId);
            return Ok(result);
        }

        [HttpPost("CreateEmploymentType")]
        public async Task<IActionResult> CreateEmploymentType([FromBody] EmploymentTypeDto dto)
        {
            var result = await _employmentTypeService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateEmploymentType")]
        public async Task<IActionResult> UpdateEmploymentType([FromBody] EmploymentTypeDto dto)
        {
            var result = await _employmentTypeService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteEmploymentType")]
        public async Task<IActionResult> DeleteEmploymentType([FromQuery] int id)
        {
            var result = await _employmentTypeService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion

        #region Grade

        [HttpGet("GetGradeAll")]
        public async Task<IActionResult> GetGradeAll(int companyId)
        {
            var result = await _gradeService.GetAllAsync(companyId);
            return Ok(result);
        }

        [HttpGet("GetGradeById/{id}")]
        public async Task<IActionResult> GetGradeById(int id)
        {
            var data = await _gradeService.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost("CreateGrade")]
        public async Task<IActionResult> CreateGrade([FromBody] GradeDto dto)
        {
            var result = await _gradeService.AddAsync(dto);
            if (result == null)
                return Ok(new { message = "Duplicate Record Found" });

            return Ok(new { message = "Grade created", data = result });
        }

        [HttpPost("UpdateGrade")]
        public async Task<IActionResult> UpdateGrade([FromBody] GradeDto dto)
        {
            var result = await _gradeService.UpdateAsync(dto);
            return Ok(new { message = "Grade updated", data = result });
        }

        [HttpPost("DeleteGrade")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var success = await _gradeService.DeleteAsync(id);
            if (!success.Success)
                return BadRequest(success);
            return Ok(new { message = "Deleted successfully" });
        }
        [HttpGet("GetGradesByCompanyRegion")]
        public async Task<IActionResult> GetGradesByCompanyRegion(int companyId, int regionId)
        {
            var data = await _gradeService.GetGradesByCompanyRegionAsync(companyId, regionId);
            return Ok(data);
        }

        #endregion
        [HttpGet("GetDesignationsbycompanycode")]
        public async Task<IActionResult> GetDesignationsbycompanycode(int companyId, int regionId)
        {
            var data = await _leaveTypeService.GetDesignationsAsync(companyId, regionId);
            return Ok(data);
        }

        #region AssetType
        [HttpGet("asset-types")]
        public async Task<IActionResult> GetAssetTypes(int userId)
        {
            return Ok(await _assetTypeService.GetAll(userId));
        }

        [HttpPost("CreateAssetType")]
        public async Task<IActionResult> CreateAssetType([FromBody] AssetTypeDto dto)
        {
            return Ok(await _assetTypeService.CreateAsync(dto));
        }

        [HttpPost("UpdateAssetType")]
        public async Task<IActionResult> UpdateAssetType([FromBody] AssetTypeDto dto)
        {
            return Ok(await _assetTypeService.UpdateAsync(dto));
        }

        [HttpPost("DeleteAssetType")]
        public async Task<IActionResult> DeleteAssetType([FromQuery] int id)
        {
            var result = await _assetTypeService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("assettypesfilter")]
        public async Task<IActionResult> GetByCompanyRegion(
    int companyId,
    int regionId, int assetCategoryId)
        {
            return Ok(await _assetTypeService.GetByCompanyRegion(companyId, regionId, assetCategoryId));
        }
        [HttpGet("assetcategoriestype")]
        public async Task<IActionResult> GetAssetCategoriestype(int userId)
        {
            return Ok(await _assetTypeService.GetAssetCategoriestype(userId));
        }
        #endregion

        #region Asset Category

        [HttpGet("asset-categories")]
        public async Task<IActionResult> GetAssetCategories([FromQuery] int userId)
        {
            var result = await _assetCategoryService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("asset-categories/{id:int}")]
        public async Task<IActionResult> GetAssetCategoryById(int id)
        {
            var result = await _assetCategoryService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateAssetCategory")]
        public async Task<IActionResult> CreateAssetCategory([FromBody] AssetCategoryDto dto)
        {
            var result = await _assetCategoryService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateAssetCategory")]
        public async Task<IActionResult> UpdateAssetCategory([FromBody] AssetCategoryDto dto)
        {
            var result = await _assetCategoryService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteAssetCategory")]
        public async Task<IActionResult> DeleteAssetCategory([FromQuery] int id)
        {
            var result = await _assetCategoryService.DeleteAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpGet("assetcategoryfilter")]
        public async Task<IActionResult> AssetCategoryDropDown(
  int companyId,
  int regionId)
        {
            return Ok(await _assetCategoryService.AssetCategoryDropDown(companyId, regionId));
        }
        #endregion

        #region Currency
        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies([FromQuery] int userId)
        {
            return Ok(await _currencyService.GetAll(userId));
        }

        [HttpPost("CreateCurrency")]
        public async Task<IActionResult> CreateCurrency([FromBody] CurrencyDto dto)
        {
            return Ok(await _currencyService.CreateAsync(dto));
        }

        [HttpPost("UpdateCurrency")]
        public async Task<IActionResult> UpdateCurrency([FromBody] CurrencyDto dto)
        {
            return Ok(await _currencyService.UpdateAsync(dto));
        }

        [HttpPost("DeleteCurrency")]
        public async Task<IActionResult> DeleteCurrency([FromQuery] int id)
        {
            var result = await _currencyService.DeleteAsync(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("currencyfilter")]
        public async Task<IActionResult> CurrencyDropDown(
   int companyId,
   int regionId)
        {
            return Ok(await _currencyService.CurrencyDropDown(companyId, regionId));
        }
        #endregion
        [HttpGet("employment-type/filter")]
        public async Task<IActionResult> GetemploymentTypeByCompanyRegion([FromQuery] int companyId, [FromQuery] int regionId)
        {
            var result = await _employmentTypeService.GetByCompanyRegion(companyId, regionId);
            return Ok(result);
        }

        #region AttachmnentType 

        [HttpGet("GetByUserAttachment")]
        public async Task<IActionResult> GetByUserAttachmentType(int userId)
        {
            var data = await _attachmentTypeService.GetAllByUserAttachmentTypeAsync(userId);
            return Ok(data);
        }

        [HttpPost("CreateAttachmnet")]
        public async Task<IActionResult> CreateAttachmentType([FromBody] AttachmentTypeDto dto)
        {
            try
            {
                var result = await _attachmentTypeService.CreateAttachmentTypeAsync(dto);

                if (!result)
                    return BadRequest("Create failed");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPut("UpdateAttachmnet")]
        public async Task<IActionResult> UpdateAttachmentType([FromBody] AttachmentTypeDto dto)
        {
            var result = await _attachmentTypeService.UpdateAttachmentTypeAsync(dto);
            if (!result) return BadRequest("Update failed");
            return Ok();
        }

        [HttpDelete("DeleteAttachmnet/{id}")]
        public async Task<IActionResult> DeleteAttachmentType(int id)
        {
            var result = await _attachmentTypeService.DeleteAttachmentTypeAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetAttachmentByCategory")]
        public async Task<IActionResult> GetAttachmentByCategory(
       string category,
       int companyId,
       int regionId)
        {
            var data = await _attachmentTypeService.GetByCategoryAsync(category, companyId, regionId);
            return Ok(data);
        }

        [HttpGet("GetDocuments")]
        public async Task<IActionResult> GetDocuments(int companyId, int regionId)
        {
            var result = await _attachmentTypeService.GetDocumentsAsync(companyId, regionId);

            return Ok(result);
        }


        #endregion

        [HttpGet("visatype-list/{userId}")]
        public async Task<IActionResult> GetVisaTypeList(int userId)
        {
            var data = await _visaTypeService.GetVisaTypeList(userId);
            return Ok(new { data });
        }

        [HttpPost("CreateVisaType")]
        public async Task<IActionResult> CreateVisaType([FromBody] VisaTypeDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data");

            var result = await _visaTypeService.CreateVisaType(dto);

            if (result)
                return Ok(new { message = "Visa Type created successfully" });

            return BadRequest("Failed to create Visa Type");
        }

        [HttpPost("UpdateVisaType")]
        public async Task<IActionResult> UpdateVisaType([FromBody] VisaTypeDto dto)
        {
            if (dto == null || dto.VisaTypeId == 0)
                return BadRequest("Invalid data");

            var result = await _visaTypeService.UpdateVisaType(dto);

            if (result)
                return Ok(new { message = "Visa Type updated successfully" });

            return NotFound("Visa Type not found");
        }

        [HttpPost("DeleteVisaType")]
        public async Task<IActionResult> DeleteVisaType(int id)
        {
            if (id == 0)
                return BadRequest("Invalid Id");

            var result = await _visaTypeService.DeleteVisaType(id);

            if (result)
                return Ok(new { message = "Visa Type deleted successfully" });

            return NotFound("Visa Type not found");
        }
        [HttpGet("GetAllAccountType")]
        public async Task<IActionResult> GetAccountTypeList(int userId)
        {
            var data = await _accountTypeService.GetAccountTypeList(userId);
            return Ok(data);
        }
        [HttpPost("CreateAccountType")]
        public async Task<IActionResult> CreateAccountType([FromBody] AccountTypeDto dto)
        {
            var result = await _accountTypeService.CreateAccountType(dto);

            if (result)
                return Ok(new { message = "Account Type created successfully" });

            return BadRequest("Failed to create Account Type");
        }
        [HttpPut("UpdateAccountType")]
        public async Task<IActionResult> UpdateAccountType([FromBody] AccountTypeDto dto)
        {
            var result = await _accountTypeService.UpdateAccountType(dto);

            if (!result)
                return BadRequest(new { message = "Account Type already exists" });

            return Ok(new { message = "Account Type updated successfully" });
        }
        [HttpPost("DeleteAccountType/{id}")]
        public async Task<IActionResult> DeleteAccountType(int id)
        {
            var result = await _accountTypeService.DeleteAccountType(id);

            if (result)
                return Ok(new { message = "Account Type deleted successfully" });

            return BadRequest(new { message = "Cannot delete. It is assigned to employee bank details." });
        }

        [HttpGet("GetAccountTypes")]
        public async Task<IActionResult> GetAccountTypes(int companyId, int regionId)
        {
            var data = await _accountTypeService.GetAccountTypesByCompanyRegion(companyId, regionId);
            return Ok(data);
         }
        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjectsMasters(int userId)
        {
            var result = await _projectMasterService.GetAllProjectsMasters(userId);
            return Ok(result);
        }
        [HttpPost("CreateProject")]

        public async Task<IActionResult> CreateProjectMaster([FromBody] ProjectMasterDto dto)
        {
            var result = await _projectMasterService.CreateProject(dto);
            return Ok(result);
        }

        [HttpPut("UpdateProject")]
        public async Task<IActionResult> UpdateProjectMaster([FromBody] ProjectMasterDto dto)
        {
            var result = await _projectMasterService.UpdateProject(dto);
            return Ok(result);
        }
        [HttpPost("DeleteProjectMaster/{id}")]
        public async Task<IActionResult> DeleteProjectMaster(int id)
        {
            var result = await _projectMasterService.DeleteProject(id);

            if (!result)
                return BadRequest(new { success = false, message = "Project is assigned to tasks or not found" });

            return Ok(new { success = true, message = "Project deleted successfully" });
        }

        [HttpGet("GetProjectsByCompanyRegion")]
        public async Task<IActionResult> GetProjectsByCompanyRegion(int companyId, int regionId)
        {
            var projects = await _projectMasterService.GetProjectsByCompanyRegion(companyId, regionId);
            return Ok(new { success = true, data = projects });
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries([FromQuery] int userId)
        {
            var result = await _countryService.GetAll(userId);
            return Ok(result);
        }

        [HttpGet("countries/{id:int}")]
        public async Task<IActionResult> GetCountryById(int id)
        {
            var result = await _countryService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("CreateCountry")]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto dto)
        {
            var result = await _countryService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("UpdateCountry")]
        public async Task<IActionResult> UpdateCountry([FromBody] CountryDto dto)
        {
            var result = await _countryService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpPost("DeleteCountry")]
        public async Task<IActionResult> DeleteCountry([FromQuery] int id)
        {
            var result = await _countryService.DeleteAsync(id);

            return result.Success
                ? Ok(result)
                : NotFound(result);
        }
        [HttpGet("countries/by-company-region")]
        public async Task<IActionResult> GetCountriesByCompanyRegion(
    [FromQuery] int companyId,
    [FromQuery] int regionId)
        {
            var result = await _countryService
                .GetByCompanyRegion(companyId, regionId);

            return Ok(result);
        }

    }
}
