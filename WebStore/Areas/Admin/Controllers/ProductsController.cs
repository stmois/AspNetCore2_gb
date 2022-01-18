using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Areas.Admin.ViewModels;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = Role.ADMINISTRATORS)]
public class ProductsController : Controller
{
    private readonly IProductData _productData;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductData productData, ILogger<ProductsController> logger)
    {
        _productData = productData;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var products = _productData.GetProducts();
        return View(products);
    }

    public IActionResult Edit(int id)
    {
        var product = _productData.GetProductById(id);

        if (product is null)
        {
            return NotFound();
        }

        return View(new EditProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Order = product.Order,
            SectionId = product.SectionId,
            Section = product.Section.Name,
            Brand = product.Brand?.Name,
            BrandId = product.BrandId,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
        });
    }

    [HttpPost]
    public IActionResult Edit(EditProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = _productData.GetProductById(model.Id);

        return product is null ? NotFound() : RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var product = _productData.GetProductById(id);

        return product is null
            ? NotFound()
            : View(new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Order = product.Order,
                SectionId = product.SectionId,
                Section = product.Section.Name,
                Brand = product.Brand?.Name,
                BrandId = product.BrandId,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
            });
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _productData.GetProductById(id);

        return product is null 
            ? NotFound() 
            : RedirectToAction(nameof(Index));
    }
}