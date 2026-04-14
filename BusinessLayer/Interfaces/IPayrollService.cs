using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IPayrollService
    {
        Task<List<PayrollTransactionDto>> PreviewPayrollAsync(ProcessPayrollRequestDto dto, int userId);
        Task<string> ProcessPayrollAsync(ProcessPayrollRequestDto dto, int userId);
        Task<List<PayrollTransactionDto>> GetPayrollByMonthAsync(int month, int year, int userId);
        Task<List<PayrollTransactionDto>> GetPayslipsByRange(PayslipFilterDto dto);
        Task SendPayslipRequestEmail(SendPayslipDto dto);

        // ✅ 1. Get Pending Requests for HR
        Task<List<HrRequestDto>> GetPendingRequests(int companyId, int regionId, string hrEmail);

        // ✅ 2. Approve / Reject Payslips
        Task ApproveRejectPayslips(HrApproveRejectDto dto);

        // ✅ 3. Get All Payrolls with Filters
        Task<List<PayrollTransactionDto>> GetAllPayrolls(HrPayrollFilterDto dto);

    }
}
