using ContactBook.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactBook.Web.Controllers
{
    /// <summary>
    /// 导入导出控制器 - 处理Excel文件的导入和导出
    /// </summary>
    public class ImportExportController : Controller
    {
        private readonly IImportExportService _importExportService;
        private readonly IContactService _contactService;
        private readonly ILogger<ImportExportController> _logger;

        public ImportExportController(
            IImportExportService importExportService,
            IContactService contactService,
            ILogger<ImportExportController> logger)
        {
            _importExportService = importExportService;
            _contactService = contactService;
            _logger = logger;
        }

        // GET: ImportExport/Import
        /// <summary>
        /// 导入页面
        /// </summary>
        public IActionResult Import()
        {
            return View();
        }

        // POST: ImportExport/Import
        /// <summary>
        /// 处理Excel文件导入
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "请选择要导入的Excel文件。");
                return View();
            }

            // 验证文件扩展名
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".xlsx" && extension != ".xls")
            {
                ModelState.AddModelError("", "只支持Excel文件格式（.xlsx 或 .xls）。");
                return View();
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var result = await _importExportService.ImportFromExcelAsync(stream);

                    if (result.SuccessCount > 0)
                    {
                        TempData["Success"] = $"成功导入 {result.SuccessCount} 条联系人记录。";
                    }

                    if (result.ErrorCount > 0)
                    {
                        TempData["Warning"] = $"有 {result.ErrorCount} 条记录导入失败。";
                        return View("ImportResult", result);
                    }

                    return RedirectToAction("Index", "Contact");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导入Excel文件时发生错误");
                ModelState.AddModelError("", "导入失败，请检查文件格式是否正确。");
                return View();
            }
        }

        // GET: ImportExport/Export
        /// <summary>
        /// 导出所有联系人到Excel
        /// </summary>
        public async Task<IActionResult> Export()
        {
            try
            {
                var contacts = await _contactService.GetAllAsync();
                
                if (!contacts.Any())
                {
                    TempData["Warning"] = "没有可导出的联系人数据。";
                    return RedirectToAction("Index", "Contact");
                }

                var stream = await _importExportService.ExportToExcelAsync(contacts);
                
                var fileName = $"联系人列表_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导出Excel文件时发生错误");
                TempData["Error"] = "导出失败，请稍后重试。";
                return RedirectToAction("Index", "Contact");
            }
        }

        // GET: ImportExport/ExportFavorites
        /// <summary>
        /// 导出收藏的联系人到Excel
        /// </summary>
        public async Task<IActionResult> ExportFavorites()
        {
            try
            {
                var favorites = await _contactService.GetFavoritesAsync();
                
                if (!favorites.Any())
                {
                    TempData["Warning"] = "没有收藏的联系人可导出。";
                    return RedirectToAction("Favorites", "Contact");
                }

                var stream = await _importExportService.ExportToExcelAsync(favorites);
                
                var fileName = $"收藏联系人_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导出收藏联系人时发生错误");
                TempData["Error"] = "导出失败，请稍后重试。";
                return RedirectToAction("Favorites", "Contact");
            }
        }

        // GET: ImportExport/DownloadTemplate
        /// <summary>
        /// 下载Excel导入模板
        /// </summary>
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                // 创建一个空的联系人列表作为模板
                var templateContacts = new List<ContactBook.Web.Models.Contact>
                {
                    new ContactBook.Web.Models.Contact
                    {
                        Name = "张三",
                        Company = "示例公司",
                        Position = "经理",
                        Notes = "这是一个示例联系人",
                        IsFavorite = true,
                        ContactMethods = new List<ContactBook.Web.Models.ContactMethod>
                        {
                            new ContactBook.Web.Models.ContactMethod
                            {
                                Type = ContactBook.Web.Models.ContactMethodType.Phone,
                                Value = "13800138000",
                                Label = "手机"
                            },
                            new ContactBook.Web.Models.ContactMethod
                            {
                                Type = ContactBook.Web.Models.ContactMethodType.Email,
                                Value = "zhangsan@example.com",
                                Label = "工作邮箱"
                            }
                        }
                    }
                };

                var stream = await _importExportService.ExportToExcelAsync(templateContacts);
                
                var fileName = "联系人导入模板.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "下载模板时发生错误");
                TempData["Error"] = "下载模板失败，请稍后重试。";
                return RedirectToAction("Import");
            }
        }
    }
}