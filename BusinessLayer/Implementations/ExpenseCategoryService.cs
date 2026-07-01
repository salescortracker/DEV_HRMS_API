using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Implementations
{
    public class ExpenseCategoryService: IExpenseCategoryService
    {
        private readonly HRMSContext _context;

        public ExpenseCategoryService(HRMSContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<ExpenseCategoryDto>>> GetAllAsync(
          int userId)
        {
            var list = await (from x in _context.ExpenseCategories
                              join c in _context.Companies
                                  on x.CompanyId equals c.CompanyId into comp
                              from c in comp.DefaultIfEmpty()

                              join r in _context.Regions
                                  on x.RegionId equals r.RegionId into reg
                              from r in reg.DefaultIfEmpty()

                              where x.UserId == userId
                              orderby x.SortOrder
                              select new ExpenseCategoryDto
                              {
                                  ExpenseCategoryID = x.ExpenseCategoryId,
                                  ExpenseCategoryName = x.ExpenseCategoryName,
                                  IsActive = x.IsActive,
                                  SortOrder = x.SortOrder,
                                  Description = x.Description,
                                  CompanyId = x.CompanyId,
                                  RegionId = x.RegionId,
                                  userId = (int)x.UserId,

                                  // ✅ Added fields
                                  CompanyName = c != null ? c.CompanyName : null,
                                  RegionName = r != null ? r.RegionName : null
                              }).ToListAsync();

            //var list = await _context.ExpenseCategories
            //    .Where(x => x.UserId == userId)
            //    .OrderBy(x => x.SortOrder)
            //    .Select(x => new ExpenseCategoryDto
            //    {
            //        ExpenseCategoryID = x.ExpenseCategoryId,
            //        ExpenseCategoryName = x.ExpenseCategoryName,
            //        IsActive = x.IsActive,
            //        SortOrder = x.SortOrder,
            //        Description = x.Description,
            //        CompanyId = x.CompanyId,
            //        RegionId = x.RegionId,
            //        userId = (int)x.UserId,
            //    })
            //    .ToListAsync();

            return new ApiResponse<IEnumerable<ExpenseCategoryDto>>(list);
        }

        public async Task<ApiResponse<bool>> AddAsync(ExpenseCategoryDto dto)
        {
            var entity = new ExpenseCategory
            {
                ExpenseCategoryName = dto.ExpenseCategoryName,
                Description = dto.Description,
                SortOrder = dto.SortOrder,
                IsActive = dto.IsActive,
                CompanyId = dto.CompanyId,
                RegionId = dto.RegionId,
                CreatedDate = DateTime.UtcNow,
                UserId = dto.userId
            };

            _context.ExpenseCategories.Add(entity);
            await _context.SaveChangesAsync();

            return new ApiResponse<bool>(true);
        }

        public async Task<ApiResponse<bool>> UpdateAsync(ExpenseCategoryDto dto)
        {
            var entity = await _context.ExpenseCategories
                .FirstOrDefaultAsync(x => x.ExpenseCategoryId == dto.ExpenseCategoryID);

            if (entity == null)
                return new ApiResponse<bool>(false, "Record not found");

            entity.ExpenseCategoryName = dto.ExpenseCategoryName;
            entity.Description = dto.Description;
            entity.SortOrder = dto.SortOrder;
            entity.IsActive = dto.IsActive;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new ApiResponse<bool>(true);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int expenseCategoryId)
        {
            try
            {
                var entity = await _context.ExpenseCategories
                    .FirstOrDefaultAsync(x => x.ExpenseCategoryId == expenseCategoryId);

                if (entity == null)
                {
                    return new ApiResponse<bool>(
                        false,
                        "Expense Category not found.",
                        false);
                }

                var isAssigned = await _context.Expenses
                    .AnyAsync(x => x.ExpenseCategoryId == expenseCategoryId);

                if (isAssigned)
                {
                    return new ApiResponse<bool>(
                        false,
                        "You cannot delete this expense category. It is assigned to one or more expenses.",
                        false);
                }

                _context.ExpenseCategories.Remove(entity);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>(
                    true,
                    "Expense Category deleted successfully.",
                    true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(
                    false,
                    ex.Message,
                    false);
            }
        }
    }
}
