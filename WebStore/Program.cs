using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Infrastructure.Conventions;
using WebStore.Infrastructure.Middleware;
using WebStore.Interfaces.ServiceInterfaces;
using WebStore.Services.Services;
using WebStore.Services.Services.InCookies;
using WebStore.Services.Services.InSQL;
using WebStore.WebApiClients.Common.RequestSender;
using WebStore.WebApiClients.Values;

var builder = WebApplication.CreateBuilder(args);

#region Настройка построителя приложения - определение содержимого

var services = builder.Services;

services.AddControllersWithViews(opt =>
{
    opt.Conventions.Add(new TestConvention());
});

var databaseType = builder.Configuration["Database"];

switch (databaseType)
{
    default: 
        throw new InvalidOperationException($"Тип БД {databaseType} не поддерживается");

    case "SqlServer":
        services.AddDbContext<WebStoreDb>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
        break;

    case "Sqlite":
        services.AddDbContext<WebStoreDb>(opt => 
            opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"),
                o => o.MigrationsAssembly("WebStore.DAL.Sqlite")));
        break;
}

services.AddTransient<IDbInitializer, DbInitializer>();
services.AddTransient<IRequestSender, RequestSender>();
services.AddTransient<IValuesClientService, ValuesClientService>();

services.AddIdentity<User, Role>()
   .AddEntityFrameworkStores<WebStoreDb>()
   .AddDefaultTokenProviders();

services.Configure<IdentityOptions>(opt =>
{
#if DEBUG
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 3;
    opt.Password.RequiredUniqueChars = 3;
#endif
    opt.User.RequireUniqueEmail = false;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ1234567890";
    opt.Lockout.AllowedForNewUsers = false;
    opt.Lockout.MaxFailedAccessAttempts = 10;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
});

services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.Name = "WebStore.GB";
    opt.Cookie.HttpOnly = true;
    opt.ExpireTimeSpan = TimeSpan.FromDays(10);
    opt.LoginPath = "/Account/Login";
    opt.LogoutPath = "/Account/Logout";
    opt.AccessDeniedPath = "/Account/AccessDenied";
    opt.SlidingExpiration = true;
});

services.AddScoped<IEmployeesData, SqlEmployeesData>();
services.AddScoped<IProductData, SqlProductData>();
services.AddScoped<IOrderService, SqlOrderService>();
services.AddScoped<ICartService, InCookiesCartService>();

#endregion

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeAsync(removeBefore: false).ConfigureAwait(true);
}

#region Конфигурирование конвейера обработки входящих соединения

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TestMiddleware>();
app.UseWelcomePage("/welcome");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

#endregion

app.Run();
