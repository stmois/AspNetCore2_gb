using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.ServiceInterfaces;
using WebStore.Services.Data;

namespace WebStore.Services.Services;

public class DbInitializer : IDbInitializer
{
    private readonly WebStoreDb _db;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(
        WebStoreDb db,
        UserManager<User> userManager, 
        RoleManager<Role> roleManager, 
        ILogger<DbInitializer> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<bool> RemoveAsync(CancellationToken cancel = default)
    {
        _logger.LogInformation("Удаление БД...");

        var result = await _db.Database.EnsureDeletedAsync(cancel).ConfigureAwait(false);

        _logger.LogInformation(result 
            ? "Удаление БД выполнено успешно" 
            : "Удаление БД не требуется (отсутствует)");

        return result;
    }

    public async Task InitializeAsync(bool removeBefore = false, CancellationToken cancel = default)
    {
        _logger.LogInformation("Инициализация БД...");

        if (removeBefore)
        {
            await RemoveAsync(cancel).ConfigureAwait(false);
        }

        var pendingMigrations = await _db.Database.GetPendingMigrationsAsync(cancel);

        if (pendingMigrations.Any())
        {
            _logger.LogInformation("Выполнение миграции БД...");

            await _db.Database.MigrateAsync(cancel).ConfigureAwait(false);

            _logger.LogInformation("Выполнение миграции БД выполнено успешно");
        }

        await InitializeProductsAsync(cancel).ConfigureAwait(false);
        await InitializeEmployeesAsync(cancel).ConfigureAwait(false);
        await InitializeIdentityAsync().ConfigureAwait(false);

        _logger.LogInformation("Инициализация БД выполнена успешно");
    }

    private async Task InitializeProductsAsync(CancellationToken cancel)
    {
        if (_db.Sections.Any())
        {
            _logger.LogInformation("Инициализация тестовых данных БД не требуется");
            return;
        }

        _logger.LogInformation("Инициализация тестовых данных БД ...");

        var sectionsPool = TestData.Sections.ToDictionary(s => s.Id);
        var brandsPool = TestData.Brands.ToDictionary(b => b.Id);

        foreach (var childSection in TestData.Sections.Where(s => s.ParentId is not null))
        {
            childSection.Parent = sectionsPool[(int)childSection.ParentId!];
        }

        foreach (var product in TestData.Products)
        {
            product.Section = sectionsPool[product.SectionId];

            if (product.BrandId is { } brandId)
            {
                product.Brand = brandsPool[brandId];
            }

            product.Id = 0;
            product.SectionId = 0;
            product.BrandId = null;
        }

        foreach (var section in TestData.Sections)
        {
            section.Id = 0;
            section.ParentId = null;
        }

        foreach (var brand in TestData.Brands)
        {
            brand.Id = 0;
        }

        await using (await _db.Database.BeginTransactionAsync(cancel))
        {
            await _db.Sections.AddRangeAsync(TestData.Sections, cancel);
            await _db.Brands.AddRangeAsync(TestData.Brands, cancel);
            await _db.Products.AddRangeAsync(TestData.Products, cancel);
            await _db.SaveChangesAsync(cancel);
            await _db.Database.CommitTransactionAsync(cancel);
        }

        _logger.LogInformation("Инициализация тестовых данных БД выполнена успешно");
    }

    private async Task InitializeEmployeesAsync(CancellationToken cancel)
    {
        if (await _db.Employees.AnyAsync(cancel))
        {
            _logger.LogInformation("Инициализация сотрудников не требуется");
            return;
        }

        _logger.LogInformation("Инициализация сотрудников...");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancel);

        TestData.Employees.ForEach(employee => employee.Id = 0);

        await _db.Employees.AddRangeAsync(TestData.Employees, cancel);
        await _db.SaveChangesAsync(cancel);

        await transaction.CommitAsync(cancel);
        _logger.LogInformation("Инициализация сотрудников выполнена успешно");
    }

    private async Task InitializeIdentityAsync()
    {
        _logger.LogInformation("Инициализация данных системы Identity");

        var timer = Stopwatch.StartNew();

        async Task CheckRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogInformation("Роль {0} существует в БД. {1} c", roleName, timer.Elapsed.TotalSeconds);
            }
            else
            {
                _logger.LogInformation("Роль {0} не существует в БД. {1} c", roleName, timer.Elapsed.TotalSeconds);

                await _roleManager.CreateAsync(new Role { Name = roleName });

                _logger.LogInformation("Роль {0} создана. {1} c", roleName, timer.Elapsed.TotalSeconds);
            }
        }

        await CheckRole(Role.ADMINISTRATORS);
        await CheckRole(Role.USERS);

        if (await _userManager.FindByNameAsync(User.ADMINISTRATOR) is null)
        {
            _logger.LogInformation("Пользователь {0} отсутствует в БД. Создаю... {1} c", User.ADMINISTRATOR, timer.Elapsed.TotalSeconds);

            var admin = new User
            {
                UserName = User.ADMINISTRATOR,
            };

            var creationResult = await _userManager.CreateAsync(admin, User.DEFAULT_ADMIN_PASSWORD);

            if (creationResult.Succeeded)
            {
                _logger.LogInformation("Пользователь {0} создан успешно. Наделяю его правами администратора... {1} c", User.ADMINISTRATOR, timer.Elapsed.TotalSeconds);

                await _userManager.AddToRoleAsync(admin, Role.ADMINISTRATORS);

                _logger.LogInformation("Пользователь {0} наделён правами администратора. {1} c", User.ADMINISTRATOR, timer.Elapsed.TotalSeconds);
            }
            else
            {
                var errors = creationResult.Errors.Select(err => err.Description);

                _logger.LogError("Учётная запись администратора не создана. Ошибки:{0}", string.Join(", ", errors));

                throw new InvalidOperationException($"Невозможно создать пользователя {User.ADMINISTRATOR} по причине: {string.Join(", ", errors)}");
            }
        }

        _logger.LogInformation("Данные системы Identity успешно добавлены в БД за {0} c", timer.Elapsed.TotalSeconds);
    }
}