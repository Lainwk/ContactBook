using ContactBook.Web.Models;
using ContactBook.Web.Models.ViewModels;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ContactBook.Web.Services
{
    /// <summary>
    /// 导入导出服务实现类
    /// </summary>
    public class ImportExportService : IImportExportService
    {
        private readonly ILogger<ImportExportService> _logger;

        public ImportExportService(ILogger<ImportExportService> logger)
        {
            _logger = logger;
            // 设置EPPlus许可证上下文为非商业用途
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// 导出联系人到Excel
        /// </summary>
        public async Task<MemoryStream> ExportToExcelAsync(IEnumerable<Contact> contacts)
        {
            try
            {
                var stream = new MemoryStream();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("联系人列表");

                    // 设置表头
                    worksheet.Cells[1, 1].Value = "姓名";
                    worksheet.Cells[1, 2].Value = "公司";
                    worksheet.Cells[1, 3].Value = "职位";
                    worksheet.Cells[1, 4].Value = "备注";
                    worksheet.Cells[1, 5].Value = "是否收藏";
                    worksheet.Cells[1, 6].Value = "联系方式类型";
                    worksheet.Cells[1, 7].Value = "联系方式标签";
                    worksheet.Cells[1, 8].Value = "联系方式值";

                    // 设置表头样式
                    using (var range = worksheet.Cells[1, 1, 1, 8])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // 填充数据
                    int row = 2;
                    foreach (var contact in contacts)
                    {
                        if (contact.ContactMethods != null && contact.ContactMethods.Any())
                        {
                            foreach (var method in contact.ContactMethods)
                            {
                                worksheet.Cells[row, 1].Value = contact.Name;
                                worksheet.Cells[row, 2].Value = contact.Company;
                                worksheet.Cells[row, 3].Value = contact.Position;
                                worksheet.Cells[row, 4].Value = contact.Notes;
                                worksheet.Cells[row, 5].Value = contact.IsFavorite ? "是" : "否";
                                worksheet.Cells[row, 6].Value = GetContactMethodTypeName(method.Type);
                                worksheet.Cells[row, 7].Value = method.Label;
                                worksheet.Cells[row, 8].Value = method.Value;
                                row++;
                            }
                        }
                        else
                        {
                            // 没有联系方式的联系人也要导出
                            worksheet.Cells[row, 1].Value = contact.Name;
                            worksheet.Cells[row, 2].Value = contact.Company;
                            worksheet.Cells[row, 3].Value = contact.Position;
                            worksheet.Cells[row, 4].Value = contact.Notes;
                            worksheet.Cells[row, 5].Value = contact.IsFavorite ? "是" : "否";
                            row++;
                        }
                    }

                    // 自动调整列宽
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    await package.SaveAsync();
                }

                stream.Position = 0;
                _logger.LogInformation("成功导出 {Count} 个联系人到Excel", contacts.Count());
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导出Excel时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 从Excel导入联系人
        /// </summary>
        public async Task<ImportResultViewModel> ImportFromExcelAsync(Stream stream)
        {
            var result = new ImportResultViewModel();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = 0,
                            ContactName = "",
                            ErrorMessage = "Excel文件中没有工作表"
                        });
                        return result;
                    }

                    var rowCount = worksheet.Dimension?.Rows ?? 0;
                    if (rowCount <= 1)
                    {
                        result.Errors.Add(new ImportError
                        {
                            RowNumber = 0,
                            ContactName = "",
                            ErrorMessage = "Excel文件中没有数据"
                        });
                        return result;
                    }

                    var contacts = new Dictionary<string, Contact>();

                    // 从第2行开始读取数据（第1行是表头）
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                result.Errors.Add(new ImportError
                                {
                                    RowNumber = row,
                                    ContactName = "",
                                    ErrorMessage = "姓名不能为空"
                                });
                                continue;
                            }

                            // 如果联系人不存在，创建新的
                            if (!contacts.ContainsKey(name))
                            {
                                var contact = new Contact
                                {
                                    Name = name,
                                    Company = worksheet.Cells[row, 2].Value?.ToString()?.Trim(),
                                    Position = worksheet.Cells[row, 3].Value?.ToString()?.Trim(),
                                    Notes = worksheet.Cells[row, 4].Value?.ToString()?.Trim(),
                                    IsFavorite = worksheet.Cells[row, 5].Value?.ToString()?.Trim() == "是",
                                    ContactMethods = new List<ContactMethod>()
                                };
                                contacts[name] = contact;
                            }

                            // 添加联系方式
                            var methodType = worksheet.Cells[row, 6].Value?.ToString()?.Trim();
                            var methodLabel = worksheet.Cells[row, 7].Value?.ToString()?.Trim();
                            var methodValue = worksheet.Cells[row, 8].Value?.ToString()?.Trim();

                            if (!string.IsNullOrWhiteSpace(methodValue))
                            {
                                var type = ParseContactMethodType(methodType);
                                contacts[name].ContactMethods.Add(new ContactMethod
                                {
                                    Type = type,
                                    Label = methodLabel,
                                    Value = methodValue
                                });
                            }

                            result.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            result.Errors.Add(new ImportError
                            {
                                RowNumber = row,
                                ContactName = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                                ErrorMessage = ex.Message
                            });
                        }
                    }

                    result.TotalCount = rowCount - 1; // 减去表头行
                }

                _logger.LogInformation("Excel导入完成，成功: {SuccessCount}, 失败: {ErrorCount}", 
                    result.SuccessCount, result.ErrorCount);

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "导入Excel时发生错误");
                result.Errors.Add(new ImportError
                {
                    RowNumber = 0,
                    ContactName = "",
                    ErrorMessage = $"导入失败: {ex.Message}"
                });
                return result;
            }
        }

        /// <summary>
        /// 获取联系方式类型的中文名称
        /// </summary>
        private string GetContactMethodTypeName(ContactMethodType type)
        {
            return type switch
            {
                ContactMethodType.Phone => "手机",
                ContactMethodType.Telephone => "电话",
                ContactMethodType.Email => "邮箱",
                ContactMethodType.WeChat => "微信",
                ContactMethodType.QQ => "QQ",
                ContactMethodType.Address => "地址",
                ContactMethodType.Other => "其他",
                _ => "其他"
            };
        }

        /// <summary>
        /// 解析联系方式类型
        /// </summary>
        private ContactMethodType ParseContactMethodType(string? typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                return ContactMethodType.Other;

            return typeName.Trim() switch
            {
                "手机" => ContactMethodType.Phone,
                "电话" => ContactMethodType.Telephone,
                "邮箱" => ContactMethodType.Email,
                "微信" => ContactMethodType.WeChat,
                "QQ" => ContactMethodType.QQ,
                "地址" => ContactMethodType.Address,
                _ => ContactMethodType.Other
            };
        }
    }
}