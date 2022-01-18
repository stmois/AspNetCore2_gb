using WebStore.Domain.Entities.Orders;
using WebStore.Domain.ViewModels;

namespace WebStore.Interfaces.ServiceInterfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userName, CancellationToken cancel = default);

    Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancel = default);

    Task<Order> CreateOrderAsync(
        string userName, 
        CartViewModel cart, 
        OrderViewModel orderModel, 
        CancellationToken cancel = default);
}