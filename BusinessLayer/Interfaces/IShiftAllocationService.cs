using BusinessLayer.Common;
using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IShiftAllocationService
    {
        // -------------------------------
        // SHIFT MASTER  (Shift Setup)
        // -------------------------------
        Task<IEnumerable<ShiftMasterDto>> GetAllShiftsAsync(int userId);
        Task<ShiftMasterDto?> GetShiftByIdAsync(int shiftId);
        Task<bool> AddShiftAsync(ShiftMasterDto dto);
        Task<bool> UpdateShiftAsync(ShiftMasterDto dto);
        Task<ApiResponse<bool>> DeleteShiftAsync(int shiftId);
        Task<bool> ActivateShiftAsync(int shiftId);
        Task<bool> DeactivateShiftAsync(int shiftId);
        Task<IEnumerable<ShiftMasterDto>> GetShiftsForDropdownAsync(int companyId, int regionId);

        // -------------------------------
        // SHIFT ALLOCATION (Main Table)
        // -------------------------------
        Task<ShiftAllocationDto?> GetAllocationByIdAsync(int id);
        Task<bool> AllocateShiftAsync(ShiftAllocationDto dto);
        Task<bool> UpdateAllocationAsync(ShiftAllocationDto dto);
        Task<EmployeeShiftDto?> GetEmployeeShiftByEmployeeCodeAsync(string employeeCode);
        Task<EmployeeShiftDto?> GetEmployeeShiftByEmployeeCodeAsync(string employeeCode, int companyId, int regionId);
        Task<bool> DeleteAllocationAsync(int id);
        Task<IEnumerable<ShiftAllocationDto>> GetAllAllocationsAsync(int userId);
        Task<IEnumerable<ShiftAllocationDto>> GetAllocationsAsync(int companyId, int regionId);
        // -------------------------------
        // USER INFO (FullName, EmployeeCode)
        // -------------------------------
        Task<UserReadDto?> GetUserByIdAsync(int userId);

        // -------------------------------
        // AUDIT LOG
        // -------------------------------
        //Task<bool> AddAuditLogAsync(string action, string description, int? createdBy);
    }
}
