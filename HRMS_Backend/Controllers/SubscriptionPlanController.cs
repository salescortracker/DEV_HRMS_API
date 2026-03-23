using BusinessLayer.DTOs;
using BusinessLayer.Implementations;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionPlanController : ControllerBase
    {
        private readonly ISubscriptionPlanService _service;
        private readonly IUserSubscriptionService _userservice;
        public SubscriptionPlanController(IUserSubscriptionService userservice,ISubscriptionPlanService service)
        {
            _service = service;
            _userservice = userservice;

        }

        [HttpGet("GetPlans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _service.GetPlansAsync();
            return Ok(plans);
        }

        [HttpGet("GetPlanById/{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var plan = await _service.GetPlanByIdAsync(id);

            if (plan == null)
                return NotFound();

            return Ok(plan);
        }

        [HttpPost("CreatePlan")]
        public async Task<IActionResult> CreatePlan([FromBody] SubscriptionPlanDto dto)
        {
            var result = await _service.CreatePlanAsync(dto);

            return Ok(result);
        }

        [HttpPut("UpdatePlan/{id}")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] SubscriptionPlanDto dto)
        {
            var result = await _service.UpdatePlanAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("DeletePlan/{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var result = await _service.DeletePlanAsync(id);

            if (!result)
                return NotFound();

            return Ok("Deleted successfully");
        }

        [HttpPost("ApplyPlan")]
        public async Task<IActionResult> ApplyPlan([FromBody] UserSubscriptionDto dto)
        {
            var result = await _userservice.ApplyPlanAsync(dto);

            return Ok(result);
        }
        [HttpPost("SavePlanMenus")]
        public async Task<IActionResult> SavePlanMenus([FromBody] PlanMenuRequestDto request)
        {
            var result = await _service.SavePlanMenus(request);
            return Ok(result);
        }
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus = await _service.GetAllMenus();
            return Ok(menus);
        }
        [HttpGet("GetMenusByType/{type}")]
        public async Task<IActionResult> GetMenusByType(string type)
        {
            var menus = await _service.GetMenusByType(type);
            return Ok(menus);
        }
    }
}
