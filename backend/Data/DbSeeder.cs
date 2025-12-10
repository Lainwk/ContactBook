using ContactBook.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Web.Data
{
    /// <summary>
    /// 数据库种子数据类
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// 添加示例数据
        /// </summary>
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // 检查是否已有数据
            if (await context.Contacts.AnyAsync())
            {
                Console.WriteLine("数据库已有数据，跳过种子数据添加。");
                return;
            }

            Console.WriteLine("正在添加示例数据...");

            // 联系人1：张三 - 软件工程师
            var contact1 = new Contact
            {
                Name = "张三",
                Company = "ABC科技有限公司",
                Position = "软件工程师",
                Notes = "负责后端开发，熟悉.NET和Python",
                IsFavorite = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod
                    {
                        Type = ContactMethodType.Phone,
                        Label = "工作手机",
                        Value = "13800138000",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Email,
                        Label = "工作邮箱",
                        Value = "zhangsan@abc-tech.com",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.WeChat,
                        Label = "微信",
                        Value = "zhangsan_dev",
                        IsPrimary = false,
                        CreatedAt = DateTime.Now
                    }
                }
            };

            // 联系人2：李四 - 产品经理
            var contact2 = new Contact
            {
                Name = "李四",
                Company = "XYZ网络公司",
                Position = "产品经理",
                Notes = "擅长用户体验设计和产品规划",
                IsFavorite = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod
                    {
                        Type = ContactMethodType.Phone,
                        Label = "个人手机",
                        Value = "13900139000",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Email,
                        Label = "个人邮箱",
                        Value = "lisi@example.com",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.QQ,
                        Label = "QQ",
                        Value = "123456789",
                        IsPrimary = false,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Address,
                        Label = "公司地址",
                        Value = "上海市浦东新区世纪大道100号",
                        IsPrimary = false,
                        CreatedAt = DateTime.Now
                    }
                }
            };

            // 联系人3：王五 - 项目经理
            var contact3 = new Contact
            {
                Name = "王五",
                Company = "DEF咨询公司",
                Position = "项目经理",
                Notes = "10年项目管理经验，PMP认证",
                IsFavorite = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ContactMethods = new List<ContactMethod>
                {
                    new ContactMethod
                    {
                        Type = ContactMethodType.Telephone,
                        Label = "办公电话",
                        Value = "010-12345678",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Phone,
                        Label = "手机",
                        Value = "13700137000",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Email,
                        Label = "邮箱",
                        Value = "wangwu@def-consulting.com",
                        IsPrimary = true,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.Address,
                        Label = "办公地址",
                        Value = "北京市朝阳区建国路88号SOHO现代城",
                        IsPrimary = false,
                        CreatedAt = DateTime.Now
                    },
                    new ContactMethod
                    {
                        Type = ContactMethodType.WeChat,
                        Label = "微信",
                        Value = "wangwu_pm",
                        IsPrimary = false,
                        CreatedAt = DateTime.Now
                    }
                }
            };

            // 添加到数据库
            await context.Contacts.AddRangeAsync(contact1, contact2, contact3);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ 成功添加3个示例联系人！");
            Console.WriteLine($"  - {contact1.Name} ({contact1.Company})");
            Console.WriteLine($"  - {contact2.Name} ({contact2.Company})");
            Console.WriteLine($"  - {contact3.Name} ({contact3.Company})");
        }
    }
}