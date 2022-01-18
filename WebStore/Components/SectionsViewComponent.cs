using Microsoft.AspNetCore.Mvc;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;

namespace WebStore.Components;


public class SectionsViewComponent : ViewComponent
{
    private readonly IProductData _productData;

    public SectionsViewComponent(IProductData productData)
    {
        _productData = productData;
    }

    public IViewComponentResult Invoke()
    {
        var sections = _productData.GetSections();
        var parentSections = sections.Where(s => s.ParentId is null);

        var parentSectionsViews = parentSections
           .Select(s => new SectionViewModel
            {
               Id = s.Id,
               Name = s.Name,
               Order = s.Order,
            })
           .ToList();

        foreach (var parentSection in parentSectionsViews)
        {
            var childs = sections.Where(s => s.ParentId == parentSection.Id);

            foreach (var childSection in childs)
            {
                parentSection.ChildSections.Add(new SectionViewModel
                {
                    Id = childSection.Id,
                    Name = childSection.Name,
                    Order = childSection.Order,
                    Parent = parentSection
                });
            }

            parentSection.ChildSections.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));
        }

        parentSectionsViews.Sort((a, b) => Comparer<int>.Default.Compare(a.Order, b.Order));

        return View(parentSectionsViews);
    }
}