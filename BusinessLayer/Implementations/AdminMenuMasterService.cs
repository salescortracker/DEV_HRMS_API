using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Interfaces;
using DataAccessLayer.DBContext;
using DataAccessLayer.Repositories.GeneralRepository;

namespace BusinessLayer.Implementations
{
    public class AdminMenuMasterService : IAdminMenuMasterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminMenuMasterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AdminMenuMasterDto>> GetAllMenusAsync()
        {
            var menus = await _unitOfWork.Repository<AdminMenuMaster>().GetAllAsync();

            return menus.Select(x => new AdminMenuMasterDto
            {
                MenuID = x.MenuId,
                MenuName = x.MenuName,
                ParentMenuID = x.ParentMenuId,
                Url = x.Url,
                Icon = x.Icon,
                OrderNo = x.OrderNo,
                IsActive = x.IsActive,
                //CanView = x.CanView,
                //CanAdd = x.CanAdd,
                //CanEdit = x.CanEdit,
                //CanDelete = x.CanDelete,
                //CanApprove = x.CanApprove
            });
        }

        public async Task<AdminMenuMasterDto?> GetMenuByIdAsync(int id)
        {
            var x = await _unitOfWork.Repository<AdminMenuMaster>().GetByIdAsync(id);

            if (x == null)
                return null;

            return new AdminMenuMasterDto
            {
                MenuID = x.MenuId,
                MenuName = x.MenuName,
                ParentMenuID = x.ParentMenuId,
                Url = x.Url,
                Icon = x.Icon,
                OrderNo = x.OrderNo,
                IsActive = x.IsActive,
                //CanView = x.CanView,
                //CanAdd = x.CanAdd,
                //CanEdit = x.CanEdit,
                //CanDelete = x.CanDelete,
                //CanApprove = x.CanApprove
            };
        }

        public async Task<AdminMenuMasterDto> AddMenuAsync(AdminMenuMasterDto dto, int createdBy)
        {
            var entity = new AdminMenuMaster
            {
                MenuName = dto.MenuName,
                ParentMenuId = dto.ParentMenuID,
                Url = dto.Url,
                Icon = dto.Icon,
                OrderNo = dto.OrderNo,
                IsActive = dto.IsActive,
                //CanView = dto.CanView,
                //CanAdd = dto.CanAdd,
                //CanEdit = dto.CanEdit,
                //CanDelete = dto.CanDelete,
                //CanApprove = dto.CanApprove,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Repository<AdminMenuMaster>().AddAsync(entity);

            await _unitOfWork.CompleteAsync();

            dto.MenuID = entity.MenuId;

            return dto;
        }

        public async Task<AdminMenuMasterDto> UpdateMenuAsync(int id, AdminMenuMasterDto dto, int modifiedBy)
        {
            var entity = await _unitOfWork.Repository<AdminMenuMaster>().GetByIdAsync(id);

            if (entity == null)
                throw new Exception("Menu not found");

            entity.MenuName = dto.MenuName;
            entity.ParentMenuId = dto.ParentMenuID;
            entity.Url = dto.Url;
            entity.Icon = dto.Icon;
            entity.OrderNo = dto.OrderNo;
            entity.IsActive = dto.IsActive;
            //entity.CanView = dto.CanView;
            //entity.CanAdd = dto.CanAdd;
            //entity.CanEdit = dto.CanEdit;
            //entity.CanDelete = dto.CanDelete;
            //entity.CanApprove = dto.CanApprove;
            entity.ModifiedBy = modifiedBy;
            entity.ModifiedAt = DateTime.Now;

            _unitOfWork.Repository<AdminMenuMaster>().Update(entity);

            await _unitOfWork.CompleteAsync();

            return dto;
        }

        public async Task<bool> DeleteMenuAsync(int id)
        {
            var entity = await _unitOfWork.Repository<AdminMenuMaster>().GetByIdAsync(id);

            if (entity == null)
                return false;

            _unitOfWork.Repository<AdminMenuMaster>().Remove(entity);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
