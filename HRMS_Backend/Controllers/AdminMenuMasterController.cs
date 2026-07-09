using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminMenuMasterController : ControllerBase
    {
        private readonly IAdminMenuMasterService _service;

        public AdminMenuMasterController(IAdminMenuMasterService service)
        {
            _service = service;
        }

        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await _service.GetAllMenusAsync();

            return Ok(data);
        }

        [HttpPost("CreateMenu")]
        public async Task<IActionResult> CreateMenu(AdminMenuMasterDto dto)
        {
            var data = await _service.AddMenuAsync(dto, 1);

            return Ok(data);
        }

        [HttpPost("UpdateMenu/{id}")]
        public async Task<IActionResult> UpdateMenu(int id, AdminMenuMasterDto dto)
        {
            var data = await _service.UpdateMenuAsync(id, dto, 1);

            return Ok(data);
        }

        [HttpPost("DeleteMenu/{id}")]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var deleted = await _service.DeleteMenuAsync(id);

            return Ok(deleted);
        }
    }
}
