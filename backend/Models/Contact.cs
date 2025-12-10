using System.ComponentModel.DataAnnotations;

namespace ContactBook.Web.Models
{
    /// <summary>
    /// 联系人实体类
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// 联系人ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 联系人姓名（必填）
        /// </summary>
        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(100, ErrorMessage = "姓名长度不能超过100个字符")]
        [Display(Name = "姓名")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        [StringLength(200, ErrorMessage = "公司名称长度不能超过200个字符")]
        [Display(Name = "公司")]
        public string? Company { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(100, ErrorMessage = "职位长度不能超过100个字符")]
        [Display(Name = "职位")]
        public string? Position { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注")]
        public string? Notes { get; set; }

        /// <summary>
        /// 是否收藏（默认为false）
        /// </summary>
        [Display(Name = "收藏")]
        public bool IsFavorite { get; set; } = false;

        /// <summary>
        /// 头像图片路径
        /// </summary>
        [StringLength(500)]
        public string? PhotoPath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 导航属性：联系方式集合
        /// </summary>
        public virtual ICollection<ContactMethod> ContactMethods { get; set; } = new List<ContactMethod>();
    }
}