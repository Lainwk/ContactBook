using ContactBook.Web.Models;
using ContactBook.Web.Models.ViewModels;

namespace ContactBook.Web.Services
{
    /// <summary>
    /// 导入导出服务接口
    /// </summary>
    public interface IImportExportService
    {
        /// <summary>
        /// 导出联系人到Excel
        /// </summary>
        /// <param name="contacts">要导出的联系人列表</param>
        /// <returns>Excel文件流</returns>
        Task<MemoryStream> ExportToExcelAsync(IEnumerable<Contact> contacts);

        /// <summary>
        /// 从Excel导入联系人
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <returns>导入结果</returns>
        Task<ImportResultViewModel> ImportFromExcelAsync(Stream stream);
    }
}