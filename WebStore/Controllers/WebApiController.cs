using Microsoft.AspNetCore.Mvc;
using WebStore.WebApiClients.Values;

namespace WebStore.Controllers;

public class WebApiController : Controller
{
    private readonly IValuesClientService _valuesService;

    public WebApiController(IValuesClientService valuesService)
    {
        _valuesService = valuesService;
    }

    public async Task<IActionResult> Index()
    {
        var values = await _valuesService.GetValuesAsync();

        return View(values);
    }
}