using BusinessLayer.Common;
using BusinessLayer.DTOs;


namespace BusinessLayer.Interfaces
{
    public interface IAttachmentTypeService
    {
        Task<IEnumerable<AttachmentTypeDto>> GetAllByUserAttachmentTypeAsync(int userId);
        Task<bool> CreateAttachmentTypeAsync(AttachmentTypeDto dto);
        Task<bool> UpdateAttachmentTypeAsync(AttachmentTypeDto dto);
        Task<ApiResponse<string>> DeleteAttachmentTypeAsync(int id);

        Task<IEnumerable<AttachmentTypeDto>> GetByCategoryAsync(string category, int companyId, int regionId);

        Task<List<AttachmentTypeDto>> GetDocumentsAsync(int companyId, int regionId);
    }
}
