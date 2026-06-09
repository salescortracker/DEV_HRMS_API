using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;

namespace BusinessLayer.Interfaces
{
    public interface IBiometricService
    {
        Task<List<AttendanceLog>> GetAttendanceLogs();
    }
}
