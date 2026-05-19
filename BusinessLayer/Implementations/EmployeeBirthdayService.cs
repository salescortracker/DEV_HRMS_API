using BusinessLayer.Interfaces;
using DataAccessLayer;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class EmployeeBirthdayService
        : IEmployeeBirthdayService
    {
        private readonly HRMSContext _context;
        private readonly IEmailService _emailService;

        public EmployeeBirthdayService(
            HRMSContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task SendTodayBirthdayWishesAsync()
        {
            var today = DateTime.Today;

            var employees = await _context.EmployeePersonalDetails
                .Where(x =>
    x.DateOfBirth.Day == today.Day &&
    x.DateOfBirth.Month == today.Month &&
                    !string.IsNullOrEmpty(x.PersonalEmail))
                .ToListAsync();

            foreach (var emp in employees)
            {
                string subject =
                    "🎂 Happy Birthday From Cortracker HRMS";

                string htmlBody = $@"
                <div style='font-family:Arial;padding:20px'>
                    <h2>
                        Happy Birthday {emp.FirstName}! 🎉
                    </h2>

                    <p>
                        Wishing you happiness, success,
                        and good health always.
                    </p>

                    <p>
                        Have a fantastic birthday celebration!
                    </p>

                    <br/>

                    <p>
                        Regards,<br/>
                        HR Team
                    </p>
                </div>";

                await _emailService.SendEmailAsync(
                    emp.PersonalEmail,
                    subject,
                    htmlBody
                );
            }
        }
    }
}
