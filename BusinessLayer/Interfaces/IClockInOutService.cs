using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IClockInOutService
    {
        Task<IEnumerable<ClockInOutDto>> GetAllAsync();
        Task<ClockInOutDto?> GetByIdAsync(int id);
        Task<IEnumerable<ClockInOutDto>> GetTodayByEmployeeAsync(string employeeCode, int companyId, int regionId);

        Task<IEnumerable<ClockInOutDto>>
GetAttendanceByDateRangeAsync(
    string employeeCode,
    int companyId,
    int regionId,
    DateOnly fromDate,
    DateOnly toDate);
        Task<ClockInOutDto> AddAsync(
    ClockInOutCreateDto dto,
    int userId
);
        Task<bool> DeleteAsync(int id, int userId);
        Task<IEnumerable<object>> GetWeeklyByEmployeeAsync(string employeeCode);
    }
}
