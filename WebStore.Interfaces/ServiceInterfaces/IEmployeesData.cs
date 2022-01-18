using WebStore.Domain.Entities;

namespace WebStore.Interfaces.ServiceInterfaces;

public interface IEmployeesData
{
    IEnumerable<Employee> GetAll();

    Employee? GetById(int id);

    int Add(Employee employee);

    bool Edit(Employee employee);

    bool Delete(int id);
}