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
    public class AccountTypeService : IAccountTypeService
    {
        private readonly HRMSContext _context;

        public AccountTypeService(HRMSContext context)
        {
            _context = context;
        }

        // 🔹 GET LIST
        public async Task<List<AccountTypeDto>> GetAccountTypeList(int userId)
        {
            return await _context.AccountTypes
                .Where(x => x.IsDeleted == false &&
                            x.CreatedBy == userId)
                .Select(x => new AccountTypeDto
                {
                    AccountTypeId = x.AccountTypeId,
                    CompanyId = x.CompanyId,
                    CompanyName = _context.Companies
                                    .Where(c => c.CompanyId == x.CompanyId)
                                    .Select(c => c.CompanyName)
                                    .FirstOrDefault(),
                    RegionName = _context.Regions
                                    .Where(r => r.RegionId == x.RegionId)
                                    .Select(r => r.RegionName)
                                    .FirstOrDefault(),
                    RegionId = x.RegionId,
                    AccountType1 = x.AccountType1,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    UserId = x.CreatedBy,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    ModifiedBy = x.ModifiedBy,
                    ModifiedAt = x.ModifiedAt
                }).ToListAsync();
        }

        // 🔹 CREATE
        public async Task<bool> CreateAccountType(AccountTypeDto dto)
        {
            var entity = new AccountType
            {
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                AccountType1 = dto.AccountType1,
                Description = dto.Description,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedBy = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.AccountTypes.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 UPDATE
        public async Task<bool> UpdateAccountType(AccountTypeDto dto)
        {
            var entity = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.AccountTypeId == dto.AccountTypeId);

            if (entity == null)
                return false;

            entity.CompanyId = dto.CompanyId;
            entity.RegionId = dto.RegionId;
            entity.AccountType1 = dto.AccountType1;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.ModifiedBy = dto.UserId;
            entity.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 DELETE (Soft Delete)
        public async Task<bool> DeleteAccountType(int id)
        {
            var entity = await _context.AccountTypes
                .FirstOrDefaultAsync(x => x.AccountTypeId == id);

            if (entity == null)
                return false;

            entity.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AccountTypeDto>> GetAccountTypesByCompanyRegion(int companyId, int regionId)
        {
            return await _context.AccountTypes
                .Where(x => x.CompanyId == companyId
                         && x.RegionId == regionId
                         && x.IsActive)
                .Select(x => new AccountTypeDto
                {
                    AccountTypeId = x.AccountTypeId,
                    AccountType1 = x.AccountType1
                })
                .ToListAsync();
        }
    }
    
}

