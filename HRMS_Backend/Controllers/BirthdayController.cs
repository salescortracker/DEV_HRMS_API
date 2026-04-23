using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BirthdayController: ControllerBase
    {
        //private readonly IBirthdayService _service;

        //public BirthdayController(IBirthdayService service)
        //{
        //    _service = service;
        //}

        //[HttpGet("GetAllBirthday")]
        //public async Task<IActionResult> GetAll()
        //{
        //    return Ok(await _service.GetAll());
        //}

        //[HttpGet("GetTodayBirthday")]
        //public async Task<IActionResult> GetToday()
        //{
        //    return Ok(await _service.GetToday());
        //}

        //[HttpPost("CreateBirthday")]
        //public async Task<IActionResult> Create([FromBody] BirthdayDto dto)
        //{
        //    var result = await _service.Create(dto);
        //    return Ok(result);
        //}

        //[HttpPost("DeleteBirthday/{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _service.Delete(id);
        //    return Ok(result);
        //}
    }
}
