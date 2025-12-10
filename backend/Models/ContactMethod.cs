using System.ComponentModel.DataAnnotations;

namespace ContactBook.Web.Models
{
    /// <summary>
    /// 联系方式实体类
    /// </summary>
    public class ContactMethod
    {
        /// <summary>
        /// 联系方式ID（主键）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 联系人ID（外键）
        /// </summary>
        [Required]
        public int ContactId { get; set; }

        /// <summary>
        /// 联系方式类型（Phone, Email, WeChat, QQ, Address等）
        /// </summary>
        [Required(ErrorMessage = "联系方式类型不能为空")]
        [Display(Name = "类型")]
        public ContactMethodType Type { get; set; }

        /// <summary>
        /// 标签（Mobile, Work, Home等）
        /// </summary>
        [StringLength(50)]
        [Display(Name = "标签")]
        public string? Label { get; set; }

        /// <summary>
        /// 联系方式的值（必填）
        /// </summary>
        [Required(ErrorMessage = "联系方式值不能为空")]
        [StringLength(500)]
        [Display(Name = "值")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 是否为主要联系方式
        /// </summary>
        [Display(Name = "主要联系方式")]
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 导航属性：所属联系人
        /// </summary>
        public virtual Contact? Contact { get; set; }
    }

    /// <summary>
    /// 联系方式类型枚举
    /// </summary>
    public enum ContactMethodType
    {
        [Display(Name = "手机")]
        Phone = 0,

        [Display(Name = "电话")]
        Telephone = 1,

        [Display(Name = "邮箱")]
        Email = 2,

        [Display(Name = "微信")]
        WeChat = 3,

        [Display(Name = "QQ")]
        QQ = 4,

        [Display(Name = "地址")]
        Address = 5,

        [Display(Name = "其他")]
        Other = 6
    }

    /// <summary>
    /// 联系方式标签枚举
    /// </summary>
    public enum ContactMethodLabel
    {
        [Display(Name = "手机")]
        Mobile,

        [Display(Name = "工作")]
        Work,

        [Display(Name = "家庭")]
        Home,

        [Display(Name = "传真")]
        Fax,

        [Display(Name = "个人")]
        Personal,

        [Display(Name = "其他")]
        Other
    }
}