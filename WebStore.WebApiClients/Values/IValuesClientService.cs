namespace WebStore.WebApiClients.Values;

public interface IValuesClientService
{
    Task<string[]?> GetValuesAsync();

    Task<int> CountAsync();

    Task<string?> GetByIdAsync(int id);

    Task AddAsync(string value);

    Task EditAsync(int id, string value);

    Task<bool> DeleteAsync(int id);
}