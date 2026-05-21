using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks(int userId)
        {
            return Ok(await _taskService.GetAll(userId));
        }

        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask([FromForm] TaskDto dto)
        {
            return Ok(await _taskService.CreateAsync(dto));
        }
        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromForm] TaskDto dto)
        {
            return Ok(await _taskService.UpdateAsync(dto));
        }

        [HttpPost("DeleteTask")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            return Ok(await _taskService.DeleteAsync(id));
        }
        [HttpGet("mytasks")]
        public async Task<IActionResult> GetMyTasks(int userId)
        {
            return Ok(await _taskService.GetMyTasks(userId));
        }
        [HttpGet("report")]
        public async Task<IActionResult> GetTaskReport(
     int companyId,
     int regionId,
     int? employeeId,
     int? statusId,
     int? priorityId,
     DateTime? fromDate,
     DateTime? toDate)
        {
            var result = await _taskService.GetTaskReport(
                companyId,
                regionId,
                employeeId,
                statusId,
                priorityId,
                fromDate,
                toDate
            );

            return Ok(result);
        }

    }
}
