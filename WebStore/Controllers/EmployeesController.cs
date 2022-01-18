using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.Entities;
using WebStore.Domain.Entities.Identity;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesData _employeesData;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeesData employeesData, ILogger<EmployeesController> logger)
        {
            _employeesData = employeesData;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var result = _employeesData.GetAll();
            return View(result);
        }

        //[Route("~/employees/info-{id}")]
        public IActionResult Details(int id)
        {
            ViewData["TestValue"] = 123;

            var employee = _employeesData.GetById(id);

            if (employee is null)
            {
                return NotFound();
            }

            ViewBag.SelectedEmployee = employee;

            return View(employee);
        }

        public IActionResult Create()
        {
            return View("Edit", new EmployeeViewModel());
        }

        [Authorize(Roles = Role.ADMINISTRATORS)]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return View(new EmployeeViewModel());
            }

            var employee = _employeesData.GetById((int)id);

            if (employee is null)
            {
                _logger.LogWarning("При редактировании сотрудника с id:{0} он не был найден", id);
                return NotFound();
            }

            var model = new EmployeeViewModel
            {
                Id = employee.Id,
                LastName = employee.LastName,
                Name = employee.FirstName,
                Patronymic = employee.Patronymic,
                Age = employee.Age,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMINISTRATORS)]
        public IActionResult Edit(EmployeeViewModel model)
        {
            if (model.LastName == "Асама" && model.Name == "Бин" && model.Patronymic == "Ладен")
            {
                ModelState.AddModelError("", "Террористов на работу не берём!");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = new Employee
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.Name,
                Patronymic = model.Patronymic,
                Age = model.Age,
            };

            if (model.Id == 0)
            {
                _employeesData.Add(employee);
                _logger.LogInformation("Создан новый сотрудник {0}", employee);
            }
            else if (!_employeesData.Edit(employee))
            {
                _logger.LogInformation("Информация о сотруднике {0} изменена", employee);
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = Role.ADMINISTRATORS)]
        public IActionResult Delete(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var employee = _employeesData.GetById(id);

            if (employee is null)
            {
                return NotFound();
            }

            var model = new EmployeeViewModel
            {
                Id = employee.Id,
                LastName = employee.LastName,
                Name = employee.FirstName,
                Patronymic = employee.Patronymic,
                Age = employee.Age,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Role.ADMINISTRATORS)]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_employeesData.Delete(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Сотрудник с id:{0} удалён", id);

            return RedirectToAction("Index");
        }
    }
}
