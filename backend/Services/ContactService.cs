using ContactBook.Web.Data;
using ContactBook.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactBook.Web.Services
{
    /// <summary>
    /// 联系人服务实现类
    /// </summary>
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactService> _logger;

        public ContactService(ApplicationDbContext context, ILogger<ContactService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有联系人
        /// </summary>
        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            try
            {
                return await _context.Contacts
                    .Include(c => c.ContactMethods)
                    .OrderByDescending(c => c.IsFavorite)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取所有联系人时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取联系人
        /// </summary>
        public async Task<Contact?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Contacts
                    .Include(c => c.ContactMethods)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取联系人详情时发生错误，ID: {ContactId}", id);
                throw;
            }
        }

        /// <summary>
        /// 创建新联系人
        /// </summary>
        public async Task<Contact> CreateAsync(Contact contact)
        {
            try
            {
                contact.CreatedAt = DateTime.Now;
                contact.UpdatedAt = DateTime.Now;

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功创建联系人，ID: {ContactId}, 姓名: {Name}", contact.Id, contact.Name);
                return contact;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建联系人时发生错误，姓名: {Name}", contact.Name);
                throw;
            }
        }

        /// <summary>
        /// 更新联系人信息
        /// </summary>
        public async Task<Contact> UpdateAsync(Contact contact)
        {
            try
            {
                var existingContact = await _context.Contacts
                    .Include(c => c.ContactMethods)
                    .FirstOrDefaultAsync(c => c.Id == contact.Id);

                if (existingContact == null)
                {
                    throw new InvalidOperationException($"联系人不存在，ID: {contact.Id}");
                }

                // 更新基本信息
                existingContact.Name = contact.Name;
                existingContact.Company = contact.Company;
                existingContact.Position = contact.Position;
                existingContact.Notes = contact.Notes;
                existingContact.IsFavorite = contact.IsFavorite;
                existingContact.UpdatedAt = DateTime.Now;

                // 更新联系方式 - 先删除旧的
                if (existingContact.ContactMethods != null && existingContact.ContactMethods.Any())
                {
                    _context.ContactMethods.RemoveRange(existingContact.ContactMethods);
                    existingContact.ContactMethods.Clear();
                }

                // 添加新的联系方式
                if (contact.ContactMethods != null && contact.ContactMethods.Any())
                {
                    if (existingContact.ContactMethods == null)
                    {
                        existingContact.ContactMethods = new List<ContactMethod>();
                    }

                    foreach (var method in contact.ContactMethods)
                    {
                        method.Id = 0; // 重置ID，让EF Core生成新ID
                        method.ContactId = existingContact.Id;
                        method.CreatedAt = DateTime.Now;
                        existingContact.ContactMethods.Add(method);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("成功更新联系人，ID: {ContactId}, 姓名: {Name}", contact.Id, contact.Name);
                return existingContact;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新联系人时发生错误，ID: {ContactId}", contact.Id);
                throw;
            }
        }

        /// <summary>
        /// 删除联系人
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("尝试删除不存在的联系人，ID: {ContactId}", id);
                    return false;
                }

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                _logger.LogInformation("成功删除联系人，ID: {ContactId}, 姓名: {Name}", id, contact.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除联系人时发生错误，ID: {ContactId}", id);
                throw;
            }
        }

        /// <summary>
        /// 切换收藏状态
        /// </summary>
        public async Task<Contact> ToggleFavoriteAsync(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact == null)
                {
                    throw new InvalidOperationException($"联系人不存在，ID: {id}");
                }

                contact.IsFavorite = !contact.IsFavorite;
                contact.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("切换收藏状态，ID: {ContactId}, 新状态: {IsFavorite}", id, contact.IsFavorite);
                return contact;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切换收藏状态时发生错误，ID: {ContactId}", id);
                throw;
            }
        }

        /// <summary>
        /// 获取收藏的联系人
        /// </summary>
        public async Task<IEnumerable<Contact>> GetFavoritesAsync()
        {
            try
            {
                return await _context.Contacts
                    .Include(c => c.ContactMethods)
                    .Where(c => c.IsFavorite)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取收藏联系人时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 搜索联系人
        /// </summary>
        public async Task<IEnumerable<Contact>> SearchAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllAsync();
                }

                searchTerm = searchTerm.ToLower();

                return await _context.Contacts
                    .Include(c => c.ContactMethods)
                    .Where(c => c.Name.ToLower().Contains(searchTerm) ||
                               (c.Company != null && c.Company.ToLower().Contains(searchTerm)) ||
                               (c.Position != null && c.Position.ToLower().Contains(searchTerm)))
                    .OrderByDescending(c => c.IsFavorite)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜索联系人时发生错误，搜索词: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}