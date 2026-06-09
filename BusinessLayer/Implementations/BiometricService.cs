using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;

namespace BusinessLayer.Implementations
{
    public class BiometricService : IBiometricService
    {
        private readonly CZKEM _device;

        public BiometricService()
        {
            _device = new CZKEM();
        }

        public async Task<List<AttendanceLog>> GetAttendanceLogs()
        {
            var logs = new List<AttendanceLog>();

            bool connected = _device.Connect_Net(
                "192.168.1.201",
                4370);

            if (!connected)
                throw new Exception("Unable to connect.");

            bool result = _device.ReadGeneralLogData(1);

            string enrollNo;
            int verifyMode, inOutMode, year, month, day, hour, minute, second, workCode;

          
            _device.Disconnect();

            return logs;
        }
    }
}
