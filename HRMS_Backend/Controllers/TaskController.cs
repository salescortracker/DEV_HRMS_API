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
            // 🔥 FILE SAVE HERE
            if (dto.Files != null && dto.Files.Count > 0)
            {
                string root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string path = Path.Combine(root, "Uploads", "Tasks");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                List<string> savedFiles = new List<string>();

                foreach (var file in dto.Files)
                {
                    string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    string fullPath = Path.Combine(path, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    savedFiles.Add(fileName);
                }

                // optional: pass to service if needed
            }

            return Ok(await _taskService.CreateAsync(dto));
        }

        [HttpPost("UpdateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskDto dto)
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
    }
}
