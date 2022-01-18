using System.Collections.Generic;
using System.Linq;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;

namespace WebStore.Services.Mapping;

public static class ProductMapper
{
    public static ProductViewModel ToView(this Product product)
    {
        return product is null
            ? null
            : new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Section = product.Section.Name,
                Brand = product.Brand?.Name,
            };
    }

    private static Product FromView(this ProductViewModel product)
    {
        return product is null
            ? null
            : new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
            };
    }

    public static IEnumerable<ProductViewModel> ToView(this IEnumerable<Product> products)
    {
        return products.Select(ToView);
    }

    public static IEnumerable<Product> FromView(this IEnumerable<ProductViewModel> products)
    {
        return products.Select(FromView);
    }
}