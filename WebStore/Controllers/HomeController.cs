using Microsoft.AspNetCore.Mvc;
using WebStore.Interfaces.ServiceInterfaces;
using WebStore.Services.Mapping;

namespace WebStore.Controllers;

public class HomeController : Controller
{
    public IActionResult Index([FromServices] IProductData productData)
    {
        var products = productData.GetProducts().OrderBy(p => p.Order).Take(6).ToView();
        ViewBag.Products = products;

        return View();
    }

    public string ConfiguredAction(string id, string value1)
    {
        return $"Hello World! {id} - {value1}";
    }

    public void Throw(string message)
    {
        throw new ApplicationException(message);
    }

    public IActionResult Error404() => View();
}