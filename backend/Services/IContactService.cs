using ContactBook.Web.Models;

namespace ContactBook.Web.Services
{
    /// <summary>
    /// 联系人服务接口
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// 获取所有联系人
        /// </summary>
        Task<IEnumerable<Contact>> GetAllAsync();

        /// <summary>
        /// 根据ID获取联系人
        /// </summary>
        Task<Contact?> GetByIdAsync(int id);

        /// <summary>
        /// 创建新联系人
        /// </summary>
        Task<Contact> CreateAsync(Contact contact);

        /// <summary>
        /// 更新联系人信息
        /// </summary>
        Task<Contact> UpdateAsync(Contact contact);

        /// <summary>
        /// 删除联系人
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 切换收藏状态
        /// </summary>
        Task<Contact> ToggleFavoriteAsync(int id);

        /// <summary>
        /// 获取收藏的联系人
        /// </summary>
        Task<IEnumerable<Contact>> GetFavoritesAsync();

        /// <summary>
        /// 搜索联系人
        /// </summary>
        Task<IEnumerable<Contact>> SearchAsync(string searchTerm);
    }
}