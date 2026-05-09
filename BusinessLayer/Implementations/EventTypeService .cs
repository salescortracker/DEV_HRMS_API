using BusinessLayer.Common;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
   public class EventTypeService: IEventTypeService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly HRMSContext _context;

            public EventTypeService(
                IUnitOfWork unitOfWork,
                HRMSContext context)
            {
                _unitOfWork = unitOfWork;
                _context = context;
            }

            public async Task<ApiResponse<IEnumerable<EventTypeDto>>> GetAllAsync(
                int companyId,
                int regionId,
                int userId)
            {
                try
                {
                    var list = await _unitOfWork.Repository<EventType>()
                        .FindAsync(x =>
                            !x.IsDeleted &&
                            x.UserId == userId);

                    var dto = list.Select(x => new EventTypeDto
                    {
                        eventTypeID = x.EventTypeId,
                        eventTypeName = x.EventTypeName,
                        description = x.Description,
                        isActive = x.IsActive,
                        companyID = x.CompanyId,
                        regionId = x.RegionId,
                        companyName = _context.Companies
                            .FirstOrDefault(c => c.CompanyId == x.CompanyId)
                            ?.CompanyName,

                        regionName = _context.Regions
                            .FirstOrDefault(r => r.RegionId == x.RegionId)
                            ?.RegionName
                    });

                    return new ApiResponse<IEnumerable<EventTypeDto>>(
                        dto,
                        "Event Types retrieved successfully.");
                }
                catch (Exception ex)
                {
                    return new ApiResponse<IEnumerable<EventTypeDto>>(
                        null!,
                        ex.Message,
                        false);
                }
            }

            public async Task<EventTypeDto?> GetByIdAsync(int id)
            {
                var entity = await _unitOfWork
                    .Repository<EventType>()
                    .GetByIdAsync(id);

                if (entity == null)
                    return null;

                return MapToDto(entity);
            }

            public async Task<EventTypeDto> AddAsync(EventTypeDto dto)
            {
                var duplicate = _context.EventTypes.Any(x =>
                    x.EventTypeName.ToLower() ==
                    dto.eventTypeName.ToLower()
                    &&
                    x.CompanyId == dto.companyID
                    &&
                    x.RegionId == dto.regionId
                    &&
                    !x.IsDeleted);

                if (duplicate)
                    return null;

                var entity = new EventType
                {
                    EventTypeName = dto.eventTypeName,
                    Description = dto.description,
                    IsActive = dto.isActive,
                    CompanyId = dto.companyID,
                    RegionId = dto.regionId,
                    UserId = dto.userId
                };

                await _unitOfWork.Repository<EventType>()
                    .AddAsync(entity);

                await _unitOfWork.CompleteAsync();

                return MapToDto(entity);
            }

            public async Task<EventTypeDto> UpdateAsync(EventTypeDto dto)
            {
                var entity = await _unitOfWork
                    .Repository<EventType>()
                    .GetByIdAsync(dto.eventTypeID);

                if (entity == null)
                    throw new Exception("Event Type not found");

                entity.EventTypeName = dto.eventTypeName;
                entity.Description = dto.description;
                entity.IsActive = dto.isActive;
                entity.CompanyId = dto.companyID;
                entity.RegionId = dto.regionId;
                entity.ModifiedAt = DateTime.UtcNow;

                _unitOfWork.Repository<EventType>()
                    .Update(entity);

                await _unitOfWork.CompleteAsync();

                return MapToDto(entity);
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var entity = await _unitOfWork
                    .Repository<EventType>()
                    .GetByIdAsync(id);

                if (entity == null)
                    return false;

                _unitOfWork.Repository<EventType>()
                    .Remove(entity);

                await _unitOfWork.CompleteAsync();

                return true;
            }

            private EventTypeDto MapToDto(EventType x)
            {
                return new EventTypeDto
                {
                    eventTypeID = x.EventTypeId,
                    eventTypeName = x.EventTypeName,
                    description = x.Description,
                    isActive = x.IsActive,
                    companyID = x.CompanyId,
                    regionId = x.RegionId
                };
            
        }
        }
}
