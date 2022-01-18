using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Services.Services.InSQL;

public class SqlOrderService : IOrderService
{
    private readonly WebStoreDb _db;
    private readonly UserManager<User> _userManager;

    public SqlOrderService(WebStoreDb db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userName, CancellationToken cancel = default)
    {
        var orders = await _db.Orders
           .Include(o => o.User)
           .Include(o => o.Items)
           .ThenInclude(item => item.Product)
           .Where(o => o.User.UserName == userName)
           .ToArrayAsync(cancel)
           .ConfigureAwait(false);

        return orders;
    }

    public async Task<Order> GetOrderByIdAsync(int id, CancellationToken cancel = default)
    {
        var order = await _db.Orders
           .Include(o => o.User)
           .Include(o => o.Items)
           .ThenInclude(item => item.Product)
           .FirstOrDefaultAsync(o => o.Id == id, cancel)
           .ConfigureAwait(false);

        return order;
    }

    public async Task<Order> CreateOrderAsync(
        string userName, 
        CartViewModel cart, 
        OrderViewModel orderModel, 
        CancellationToken cancel = default)
    {
        var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

        if (user is null)
            throw new InvalidOperationException($"Пользователь с именем {userName} не найден в БД");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancel).ConfigureAwait(false);

        var order = new Order
        {
            User = user,
            Address = orderModel.Address,
            Phone = orderModel.Phone,
            Description = orderModel.Description,
        };

        var productsIds = cart.Items.Select(item => item.Product.Id).ToArray();

        var cartProducts = await _db.Products
           .Where(p => productsIds.Contains(p.Id))
           .ToArrayAsync(cancel)
           .ConfigureAwait(false);

        order.Items = cart.Items.Join(
            cartProducts,
            cartItem => cartItem.Product.Id,
            cartProduct => cartProduct.Id,
            (cartItem, cartProduct) => new OrderItem
            {
                Order = order,
                Product = cartProduct,
                Price = cartProduct.Price, // Здесь может быть применена скидка к стоимости товара
                Quantity = cartItem.Quantity,
            }).ToArray();

        await _db.Orders.AddAsync(order, cancel).ConfigureAwait(false);
        await _db.SaveChangesAsync(cancel).ConfigureAwait(false);
        await transaction.CommitAsync(cancel).ConfigureAwait(false);

        return order;
    }
}