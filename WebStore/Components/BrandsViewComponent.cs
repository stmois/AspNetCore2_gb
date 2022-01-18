using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Components;

public class BrandsViewComponent : ViewComponent
{
    private readonly IProductData _productData;

    public BrandsViewComponent(IProductData productData)
    {
        _productData = productData;
    }

    public IViewComponentResult Invoke()
    {
        return View(GetBrands());
    }

    private IEnumerable<BrandViewModel> GetBrands() =>
        _productData.GetBrands()
           .OrderBy(b => b.Order)
           .Select(b => new BrandViewModel
            {
                Id = b.Id,
                Name = b.Name,
            });
}