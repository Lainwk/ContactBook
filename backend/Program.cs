using ContactBook.Web.Data;
using ContactBook.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加数据库上下文 - 使用SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册服务 - 依赖注入
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();

// 添加MVC控制器和视图
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 初始化数据库并添加示例数据
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // 确保数据库已创建
        await context.Database.EnsureCreatedAsync();
        
        // 添加示例数据
        await DbSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "初始化数据库时发生错误");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 配置默认路由 - 默认到联系人列表页面
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contact}/{action=Index}/{id?}");

app.Run();
