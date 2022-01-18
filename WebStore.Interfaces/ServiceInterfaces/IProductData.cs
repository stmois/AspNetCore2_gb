using WebStore.Domain;
using WebStore.Domain.Entities;

namespace WebStore.Interfaces.ServiceInterfaces;

public interface IProductData
{
    IEnumerable<Section> GetSections();

    Section? GetSectionById(int id);

    IEnumerable<Brand> GetBrands();

    Brand? GetBrandById(int id);

    IEnumerable<Product> GetProducts(ProductFilter? filter = null);

    Product? GetProductById(int id);

    Product CreateProduct(
        string name, 
        int order, 
        decimal price, 
        string imageUrl, 
        string section, 
        string? brand = null);
}