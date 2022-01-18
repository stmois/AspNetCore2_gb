using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebStore.DAL.Context;
using WebStore.Domain;
using WebStore.Domain.Entities;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Services.Services.InSQL;

public class SqlProductData : IProductData
{
    private readonly WebStoreDb _db;

    public SqlProductData(WebStoreDb db)
    {
        _db = db;
    }

    public IEnumerable<Section> GetSections()
    {
        return _db.Sections;
    }

    public Section GetSectionById(int id)
    {
        return _db.Sections
            .Include(s => s.Products)
            .FirstOrDefault(s => s.Id == id);
    }

    public IEnumerable<Brand> GetBrands()
    {
        return _db.Brands;
    }

    public Brand GetBrandById(int id)
    {
        return _db.Brands
            .Include(b => b.Products)
            .FirstOrDefault(b => b.Id == id);
    }

    public IEnumerable<Product> GetProducts(ProductFilter filter = null)
    {
        IQueryable<Product> query = _db.Products
           .Include(p => p.Brand)
           .Include(p => p.Section);

        if (filter?.Ids?.Length > 0)
        {
            query = query.Where(product => filter.Ids.Contains(product.Id));
        }
        else
        {
            if (filter?.SectionId is { } sectionId)
                query = query.Where(p => p.SectionId == sectionId);

            if (filter?.BrandId is { } brandId)
                query = query.Where(p => p.BrandId == brandId);
        }

        return query;
    }

    public Product GetProductById(int id)
    {
        return _db.Products
            .Include(p => p.Brand)
            .Include(p => p.Section)
            .FirstOrDefault(p => p.Id == id);
    }

    public Product CreateProduct(
        string name, 
        int order, 
        decimal price, 
        string imageUrl,
        string section, 
        string brand = null)
    {
        var sectionSelected = _db.Sections.FirstOrDefault(s => s.Name == section)
                              ?? new Section { Name = section };

        var brandSelected = brand is { Length: > 0 }
            ? _db.Brands.FirstOrDefault(b => b.Name == brand) ?? new Brand { Name = brand }
            : null;

        var product = new Product
        {
            Name = name,
            Price = price,
            Order = order,
            ImageUrl = imageUrl,
            Section = sectionSelected,
            Brand = brandSelected,
        };

        _db.Products.Add(product);
        _db.SaveChanges();

        return product;
    }
}