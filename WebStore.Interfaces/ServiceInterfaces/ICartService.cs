using WebStore.Domain.ViewModels;

namespace WebStore.Interfaces.ServiceInterfaces;

public interface ICartService
{
    void Add(int id);

    void Decrement(int id);

    void Remove(int id);

    void Clear();

    CartViewModel GetViewModel();
}