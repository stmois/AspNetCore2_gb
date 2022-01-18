using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebStore.DAL.Context;
using WebStore.Domain.Entities;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Services.Services.InSQL;

public class SqlEmployeesData : IEmployeesData
{
    private readonly WebStoreDb _db;
    private readonly ILogger<SqlEmployeesData> _logger;

    public SqlEmployeesData(WebStoreDb db, ILogger<SqlEmployeesData> logger)
    {
        _db = db;
        _logger = logger;
    }

    public IEnumerable<Employee> GetAll()
    {
        return _db.Employees.AsEnumerable();
    }

    public Employee GetById(int id)
    {
        return _db.Employees.Find(id);
    }

    public int Add(Employee employee)
    {
        _db.Entry(employee).State = EntityState.Added;
        _db.SaveChanges(); 

        return employee.Id;
    }

    public bool Edit(Employee employee)
    {
        _db.Employees.Update(employee);

        return _db.SaveChanges() != 0;
    }

    public bool Delete(int id)
    {
        var employee = _db.Employees
           .Select(e => new Employee { Id = e.Id, }) // Неполная проекция - для экономии памяти и времени на передачу данных
           .FirstOrDefault(e => e.Id == id);

        if (employee is null)
        {
            return false;
        }

        _db.Employees.Remove(employee);
        _db.SaveChanges();

        return true;
    }
}