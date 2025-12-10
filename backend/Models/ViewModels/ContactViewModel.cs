using System.ComponentModel.DataAnnotations;

namespace ContactBook.Web.Models.ViewModels
{
    /// <summary>
    /// 联系人视图模型 - 用于创建和编辑联系人
    /// </summary>
    public class ContactViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(100, ErrorMessage = "姓名长度不能超过100个字符")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "公司")]
        public string? Company { get; set; }

        [StringLength(100)]
        [Display(Name = "职位")]
        public string? Position { get; set; }

        [Display(Name = "备注")]
        public string? Notes { get; set; }

        [Display(Name = "收藏")]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// 联系方式列表
        /// </summary>
        public List<ContactMethodViewModel> ContactMethods { get; set; } = new List<ContactMethodViewModel>();
    }

    /// <summary>
    /// 联系方式视图模型
    /// </summary>
    public class ContactMethodViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "类型不能为空")]
        [Display(Name = "类型")]
        public ContactMethodType Type { get; set; }

        [Display(Name = "标签")]
        public string? Label { get; set; }

        [Required(ErrorMessage = "值不能为空")]
        [Display(Name = "值")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "主要")]
        public bool IsPrimary { get; set; }
    }

    /// <summary>
    /// 联系人列表视图模型
    /// </summary>
    public class ContactListViewModel
    {
        /// <summary>
        /// 联系人列表
        /// </summary>
        public List<Contact> Contacts { get; set; } = new List<Contact>();

        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// 是否只显示收藏
        /// </summary>
        public bool FavoritesOnly { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 导入结果视图模型
    /// </summary>
    public class ImportResultViewModel
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 成功导入数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 失败数量
        /// </summary>
        public int ErrorCount => TotalCount - SuccessCount;

        /// <summary>
        /// 错误信息列表
        /// </summary>
        public List<ImportError> Errors { get; set; } = new List<ImportError>();

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess => ErrorCount == 0;
    }

    /// <summary>
    /// 导入错误信息
    /// </summary>
    public class ImportError
    {
        public int RowNumber { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}