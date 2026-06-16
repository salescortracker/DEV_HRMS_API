using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(int userId)
        {
            var companies = await _unitOfWork.Repository<Company>().GetAllAsync();

            return companies
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CompanyId)
                .Select(MapToDto);
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(int id)
        {
            var company = await _unitOfWork.Repository<Company>().GetByIdAsync(id);
            return company == null ? null : MapToDto(company);
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompaniesAsync(object filter)
        {
            var props = filter.GetType().GetProperties();
            var allCompanies = await _unitOfWork.Repository<Company>().GetAllAsync();
            var query = allCompanies.AsQueryable();

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(filter);

                if (value == null) continue;

                switch (name)
                {
                    case nameof(Company.CompanyName):
                        query = query.Where(c =>
                            c.CompanyName != null &&
                            c.CompanyName.Contains(value.ToString()!));
                        break;

                    case nameof(Company.CompanyCode):
                        query = query.Where(c =>
                            c.CompanyCode == value.ToString());
                        break;

                    case nameof(Company.IsActive):
                        bool isActive = Convert.ToBoolean(value);
                        query = query.Where(c => c.IsActive == isActive);
                        break;
                }
            }

            return query.Select(MapToDto);
        }

        public async Task<CompanyDto> AddCompanyAsync(CompanyDto dto)
        {
            var entity = new Company
            {
                CompanyName = dto.companyName,
                CompanyCode = dto.companyCode,
                IndustryType = dto.industryType,
                Headquarters = dto.headquarters,
                IsActive = dto.isActive,
                UserId = dto.userId,
                CreatedDate = DateTime.UtcNow,
                CompanyContact = dto.CompanyContact,
                CompanyEmail = dto.CompanyEmail,
                CompanyAddress = dto.CompanyAddress,
                CompanyLogo = dto.CompanyLogo
            };

            await _unitOfWork.Repository<Company>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        //public async Task<IEnumerable<Company>> AddCompaniesAsync(List<CompanyDto> dtos)
        //{
        //    var companies = dtos.Select(dto => new Company
        //    {
        //        CompanyName = dto.companyName,
        //        CompanyCode = dto.companyCode,
        //        IndustryType = dto.industryType,
        //        Headquarters = dto.headquarters,
        //        IsActive = dto.isActive,
        //        UserId = dto.userId,
        //        CreatedDate = DateTime.UtcNow,
        //        CompanyContact = dto.CompanyContact,
        //        CompanyEmail = dto.CompanyEmail,
        //        CompanyAddress = dto.CompanyAddress,
        //        CompanyLogo = dto.CompanyLogo
        //    }).ToList();

        //    await _unitOfWork.Repository<Company>().AddRangeAsync(companies);
        //    await _unitOfWork.CompleteAsync();

        //    return companies;
        //}
        public async Task<IEnumerable<Company>> AddCompaniesAsync(List<CompanyDto> dtos)
        {
            var companies = dtos.Select(dto => new Company
            {
                CompanyName = dto.companyName,
                CompanyCode = dto.companyCode,
                IndustryType = dto.industryType,
                Headquarters = dto.headquarters,
                IsActive = dto.isActive,
                UserId = dto.userId,
                CreatedDate = DateTime.UtcNow,
                CompanyContact = dto.CompanyContact,
                CompanyEmail = dto.CompanyEmail,
                CompanyAddress = dto.CompanyAddress,
                CompanyLogo = dto.CompanyLogo
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.CompanyCode))
            .ToList();

            // Duplicate CompanyCode check in uploaded file
            var duplicateCodes = companies
                .Where(x => !string.IsNullOrWhiteSpace(x.CompanyCode))
                .GroupBy(x => x.CompanyCode.Trim().ToLower())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateCodes.Any())
            {
                throw new Exception($"Duplicate CompanyCode found: {string.Join(", ", duplicateCodes)}");
            }
            foreach (var company in companies)
            {
                Console.WriteLine($"Name={company.CompanyName}, Code={company.CompanyCode},UserId={company.UserId}, Contact={company.CompanyContact}, Email={company.CompanyEmail}");
            }

            await _unitOfWork.Repository<Company>().AddRangeAsync(companies);
            await _unitOfWork.CompleteAsync();

            return companies;
        }
        public async Task<CompanyDto> UpdateCompanyAsync(int id, CompanyDto dto)
        {
            var entity = await _unitOfWork.Repository<Company>().GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Company not found");

            entity.CompanyName = dto.companyName;
            entity.CompanyCode = dto.companyCode;
            entity.IndustryType = dto.industryType;
            entity.Headquarters = dto.headquarters;
            entity.IsActive = dto.isActive;
            entity.UserId = dto.userId;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.CompanyContact = dto.CompanyContact;
            entity.CompanyEmail = dto.CompanyEmail;
            entity.CompanyAddress = dto.CompanyAddress;
            entity.CompanyLogo = dto.CompanyLogo;

            _unitOfWork.Repository<Company>().Update(entity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(entity);
        }

        //public async Task<bool> DeleteCompanyAsync(int id)
        //{
        //    try
        //    {
        //        var entity = await _unitOfWork.Repository<Company>().GetByIdAsync(id);

        //        if (entity == null) return false;

        //        _unitOfWork.Repository<Company>().Remove(entity);
        //        await _unitOfWork.CompleteAsync();

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Company>().GetByIdAsync(id);

            if (entity == null)
                return false;

            try
            {
                _unitOfWork.Repository<Company>().Remove(entity);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log error (replace with Serilog if you use it)
                Console.WriteLine($"DeleteCompany Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // Important: don't hide error
                throw;
            }
        }

        private CompanyDto MapToDto(Company c)
        {
            return new CompanyDto
            {
                CompanyId = c.CompanyId,
                companyName = c.CompanyName,
                companyCode = c.CompanyCode,
                industryType = c.IndustryType,
                headquarters = c.Headquarters,
                isActive = c.IsActive,
                userId = c.UserId,
                CompanyContact = c.CompanyContact,
                CompanyEmail = c.CompanyEmail,
                CompanyAddress = c.CompanyAddress,
                CompanyLogo = c.CompanyLogo
            };
        }
    }
}
