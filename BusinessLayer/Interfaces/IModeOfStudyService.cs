using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IModeOfStudyService
    {
        Task<IEnumerable<ModeOfStudyDto>> GetAllModeOfStudytAsync(int userId); 

        Task<ModeOfStudyDto?> GetByIdModeOfStudytAsync(int id);

        Task<bool> CreateModeOfStudytAsync(ModeOfStudyDto dto);

        Task<bool> UpdateModeOfStudytAsync(ModeOfStudyDto dto);

        Task<bool> DeleteModeOfStudytAsync(int id);


    }
}
