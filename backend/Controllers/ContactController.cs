using ContactBook.Web.Models;
using ContactBook.Web.Models.ViewModels;
using ContactBook.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Web.Controllers
{
    /// <summary>
    /// 联系人控制器 - 处理联系人的CRUD操作
    /// </summary>
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactService contactService, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        // GET: Contact
        /// <summary>
        /// 联系人列表页面 - 支持搜索和收藏筛选
        /// </summary>
        public async Task<IActionResult> Index(string searchTerm, bool? showFavoritesOnly)
        {
            try
            {
                IEnumerable<Contact> contacts;

                if (showFavoritesOnly == true)
                {
                    // 只显示收藏的联系人
                    contacts = await _contactService.GetFavoritesAsync();
                }
                else if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    // 搜索联系人
                    contacts = await _contactService.SearchAsync(searchTerm);
                }
                else
                {
                    // 显示所有联系人
                    contacts = await _contactService.GetAllAsync();
                }

                ViewBag.SearchTerm = searchTerm;
                ViewBag.ShowFavoritesOnly = showFavoritesOnly;

                return View(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取联系人列表时发生错误");
                TempData["Error"] = "获取联系人列表失败，请稍后重试。";
                return View(new List<Contact>());
            }
        }

        // GET: Contact/Details/5
        /// <summary>
        /// 联系人详情页面
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var contact = await _contactService.GetByIdAsync(id.Value);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取联系人详情时发生错误，ID: {ContactId}", id);
                TempData["Error"] = "获取联系人详情失败，请稍后重试。";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Contact/Create
        /// <summary>
        /// 创建联系人页面
        /// </summary>
        public IActionResult Create()
        {
            var viewModel = new ContactViewModel
            {
                ContactMethods = new List<ContactMethodViewModel>
                {
                    new ContactMethodViewModel() // 默认添加一个联系方式
                }
            };
            return View(viewModel);
        }

        // POST: Contact/Create
        /// <summary>
        /// 创建联系人 - 处理表单提交
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contact = new Contact
                    {
                        Name = viewModel.Name,
                        Company = viewModel.Company,
                        Position = viewModel.Position,
                        Notes = viewModel.Notes,
                        IsFavorite = viewModel.IsFavorite,
                        ContactMethods = viewModel.ContactMethods.Select(cm => new ContactMethod
                        {
                            Type = cm.Type,
                            Value = cm.Value,
                            Label = cm.Label
                        }).ToList()
                    };

                    await _contactService.CreateAsync(contact);
                    TempData["Success"] = "联系人创建成功！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "创建联系人时发生错误");
                    ModelState.AddModelError("", "创建联系人失败，请稍后重试。");
                }
            }

            return View(viewModel);
        }

        // GET: Contact/Edit/5
        /// <summary>
        /// 编辑联系人页面
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var contact = await _contactService.GetByIdAsync(id.Value);
                if (contact == null)
                {
                    return NotFound();
                }

                var viewModel = new ContactViewModel
                {
                    Id = contact.Id,
                    Name = contact.Name,
                    Company = contact.Company,
                    Position = contact.Position,
                    Notes = contact.Notes,
                    IsFavorite = contact.IsFavorite,
                    ContactMethods = contact.ContactMethods.Select(cm => new ContactMethodViewModel
                    {
                        Id = cm.Id,
                        Type = cm.Type,
                        Value = cm.Value,
                        Label = cm.Label
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取编辑联系人页面时发生错误，ID: {ContactId}", id);
                TempData["Error"] = "获取联系人信息失败，请稍后重试。";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contact/Edit/5
        /// <summary>
        /// 编辑联系人 - 处理表单提交
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 创建新的 Contact 对象，不从数据库加载
                    var contact = new Contact
                    {
                        Id = id,
                        Name = viewModel.Name,
                        Company = viewModel.Company,
                        Position = viewModel.Position,
                        Notes = viewModel.Notes,
                        IsFavorite = viewModel.IsFavorite,
                        ContactMethods = viewModel.ContactMethods.Select(cm => new ContactMethod
                        {
                            Id = 0,  // 重置ID，让Service层处理
                            Type = cm.Type,
                            Value = cm.Value,
                            Label = cm.Label,
                            IsPrimary = cm.IsPrimary
                        }).ToList()
                    };

                    await _contactService.UpdateAsync(contact);
                    TempData["Success"] = "联系人更新成功！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "更新联系人时发生错误，ID: {ContactId}", id);
                    ModelState.AddModelError("", "更新联系人失败，请稍后重试。");
                }
            }

            return View(viewModel);
        }

        // GET: Contact/Delete/5
        /// <summary>
        /// 删除联系人确认页面
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var contact = await _contactService.GetByIdAsync(id.Value);
                if (contact == null)
                {
                    return NotFound();
                }

                return View(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取删除确认页面时发生错误，ID: {ContactId}", id);
                TempData["Error"] = "获取联系人信息失败，请稍后重试。";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contact/Delete/5
        /// <summary>
        /// 删除联系人 - 处理删除操作
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _contactService.DeleteAsync(id);
                TempData["Success"] = "联系人删除成功！";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除联系人时发生错误，ID: {ContactId}", id);
                TempData["Error"] = "删除联系人失败，请稍后重试。";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contact/ToggleFavorite/5
        /// <summary>
        /// 切换收藏状态 - AJAX请求
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            try
            {
                var contact = await _contactService.ToggleFavoriteAsync(id);
                return Json(new { success = true, isFavorite = contact.IsFavorite });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切换收藏状态时发生错误，ID: {ContactId}", id);
                return Json(new { success = false, message = "操作失败，请稍后重试。" });
            }
        }

        // GET: Contact/Favorites
        /// <summary>
        /// 收藏的联系人列表页面
        /// </summary>
        public async Task<IActionResult> Favorites()
        {
            try
            {
                var favorites = await _contactService.GetFavoritesAsync();
                return View("Index", favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取收藏联系人列表时发生错误");
                TempData["Error"] = "获取收藏列表失败，请稍后重试。";
                return View("Index", new List<Contact>());
            }
        }
    }
}