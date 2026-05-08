using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
   public interface IEventTypeService
    {
        Task<ApiResponse<IEnumerable<EventTypeDto>>> GetAllAsync(
           int companyId,
           int regionId,
           int userId);

        Task<EventTypeDto?> GetByIdAsync(int id);

        Task<EventTypeDto> AddAsync(EventTypeDto dto);

        Task<EventTypeDto> UpdateAsync(EventTypeDto dto);

        Task<bool> DeleteAsync(int id);

    }
}
