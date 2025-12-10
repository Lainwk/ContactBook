using ContactBook.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactBook.Web.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 联系人数据集
        /// </summary>
        public DbSet<Contact> Contacts { get; set; } = null!;

        /// <summary>
        /// 联系方式数据集
        /// </summary>
        public DbSet<ContactMethod> ContactMethods { get; set; } = null!;

        /// <summary>
        /// 配置实体模型
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置Contact实体
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Company)
                    .HasMaxLength(200);

                entity.Property(e => e.Position)
                    .HasMaxLength(100);

                entity.Property(e => e.PhotoPath)
                    .HasMaxLength(500);

                entity.Property(e => e.IsFavorite)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("datetime('now')");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("datetime('now')");

                // 创建索引以提升查询性能
                entity.HasIndex(e => e.IsFavorite)
                    .HasDatabaseName("IX_Contacts_IsFavorite");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Contacts_Name");

                entity.HasIndex(e => new { e.IsFavorite, e.Name })
                    .HasDatabaseName("IX_Contacts_IsFavorite_Name");
            });

            // 配置ContactMethod实体
            modelBuilder.Entity<ContactMethod>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.Label)
                    .HasMaxLength(50);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.IsPrimary)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("datetime('now')");

                // 配置外键关系 - 级联删除
                entity.HasOne(e => e.Contact)
                    .WithMany(c => c.ContactMethods)
                    .HasForeignKey(e => e.ContactId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 创建索引
                entity.HasIndex(e => e.ContactId)
                    .HasDatabaseName("IX_ContactMethods_ContactId");

                entity.HasIndex(e => e.Type)
                    .HasDatabaseName("IX_ContactMethods_Type");

                entity.HasIndex(e => new { e.ContactId, e.Type })
                    .HasDatabaseName("IX_ContactMethods_ContactId_Type");
            });
        }
    }
}