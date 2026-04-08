using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IVisatypeService
    {
        Task<List<VisaTypeDto>> GetVisaTypeList(int userId);
        Task<bool> CreateVisaType(VisaTypeDto dto);
        Task<bool> UpdateVisaType(VisaTypeDto dto);
        Task<bool> DeleteVisaType(int id);
    }
}
