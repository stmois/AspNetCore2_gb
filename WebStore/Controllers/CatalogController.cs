using Microsoft.AspNetCore.Mvc;
using WebStore.Domain;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;
using WebStore.Services.Mapping;

namespace WebStore.Controllers;

public class CatalogController : Controller
{
    private readonly IProductData _productData;

    public CatalogController(IProductData productData)
    {
        _productData = productData;
    }

    public IActionResult Index(int? brandId, int? sectionId)
    {
        var filter = new ProductFilter
        {
            BrandId = brandId,
            SectionId = sectionId,
        };

        var products = _productData.GetProducts(filter);

        var catalogModel = new CatalogViewModel
        {
            BrandId = brandId,
            SectionId = sectionId,
            Products = products.OrderBy(p => p.Order).ToView(),
        };

        return View(catalogModel);
    }

    public IActionResult Details(int id)
    {
        var product = _productData.GetProductById(id);

        return product is null 
            ? NotFound() 
            : View(product.ToView());
    }
}