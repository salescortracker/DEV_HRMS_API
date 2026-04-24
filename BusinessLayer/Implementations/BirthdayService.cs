using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class BirthdayService: IBirthdayService
    {
        //private readonly HRMSContext _context;

        //public BirthdayService(HRMSContext context)
        //{
        //    _context = context;
        //}

        //public async Task<List<BirthdayDto>> GetAll()
        //{
        //    return await _context.BirthdayEmployees
        //        .Where(x => x.IsActive == true)
        //        .Select(x => new BirthdayDto
        //        {
        //            Id = x.Id,
        //            CompanyId = x.CompanyId,
        //            RegionId = x.RegionId,
        //            EmployeeId = x.EmployeeId,
        //            FirstName = x.FirstName,
        //            Email = x.Email,
        //            DateOfBirth = x.DateOfBirth,
        //            IsActive = x.IsActive
        //        }).ToListAsync();
        //}

        //public async Task<List<BirthdayDto>> GetToday()
        //{
        //    var today = DateTime.Today;

        //    return await _context.BirthdayEmployees
        //        .Where(x => x.IsActive==true)
                        
        //        .Select(x => new BirthdayDto
        //        {
        //            Id = x.Id,
        //            FirstName = x.FirstName,
        //            Email = x.Email,
        //            DateOfBirth = x.DateOfBirth
        //        }).ToListAsync();
        //}

        //public async Task<bool> Create(BirthdayDto dto)
        //{
        //    var entity = new BirthdayEmployee
        //    {
        //        CompanyId = dto.CompanyId,
        //        RegionId = dto.RegionId,
        //        EmployeeId = dto.EmployeeId,
        //        FirstName = dto.FirstName,
        //        Email = dto.Email,
        //        DateOfBirth = dto.DateOfBirth,
        //        IsActive = true
        //    };

        //    _context.BirthdayEmployees.Add(entity);
        //    return await _context.SaveChangesAsync() > 0;
        //}

        //public async Task<bool> Delete(int id)
        //{
        //    var entity = await _context.BirthdayEmployees.FindAsync(id);
        //    if (entity == null) return false;

        //    entity.IsActive = false;
        //    return await _context.SaveChangesAsync() > 0;
        //}
    }
}
