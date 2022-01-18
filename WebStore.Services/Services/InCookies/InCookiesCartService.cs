using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebStore.Domain.Entities;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.ServiceInterfaces;
using WebStore.Services.Mapping;

namespace WebStore.Services.Services.InCookies;

public class InCookiesCartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductData _productData;
    private readonly string _cartName;

    private Cart Cart
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            var cookies = context!.Response.Cookies;
            var cartCookie = context.Request.Cookies[_cartName];

            if (cartCookie is null)
            {
                var cart = new Cart();
                cookies.Append(_cartName, JsonConvert.SerializeObject(cart));

                return cart;
            }

            ReplaceCart(cookies, cartCookie);

            return JsonConvert.DeserializeObject<Cart>(cartCookie)!;
        }
        set
        {
            ReplaceCart(_httpContextAccessor.HttpContext!.Response.Cookies, JsonConvert.SerializeObject(value));
        }
    }

    private void ReplaceCart(IResponseCookies cookies, string cart)
    {
        cookies.Delete(_cartName);
        cookies.Append(_cartName, cart);
    }

    public InCookiesCartService(IHttpContextAccessor httpContextAccessor, IProductData productData)
    {
        _httpContextAccessor = httpContextAccessor;
        _productData = productData;

        var user = httpContextAccessor.HttpContext!.User;
        var userName = user.Identity!.IsAuthenticated ? $"-{user.Identity.Name}" : null;

        _cartName = $"WebStore.GB.Cart{userName}";
    }

    public void Add(int id)
    {
        var cart = Cart;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

        if (item is null)
        {
            cart.Items.Add(new CartItem { ProductId = id, Quantity = 1 });
        }
        else
        {
            item.Quantity++;
        }

        Cart = cart;
    }

    public void Decrement(int id)
    {
        var cart = Cart;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

        if(item is null)
        {
            return;
        }

        if (item.Quantity > 0)
        {
            item.Quantity--;
        }

        if (item.Quantity == 0)
        {
            cart.Items.Remove(item);
        }

        Cart = cart;
    }

    public void Remove(int id)
    {
        var cart = Cart;
        var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

        if (item is null)
        {
            return;
        }

        cart.Items.Remove(item);

        Cart = cart;
    }

    public void Clear()
    {
        var cart = Cart;
        cart.Items.Clear();

        Cart = cart;
    }

    public CartViewModel GetViewModel()
    {
        var cart = Cart;
        var products = _productData.GetProducts(new()
        {
            Ids = cart.Items.Select(i => i.ProductId).ToArray()
        });

        var productsViews = products.ToView().ToDictionary(p => p!.Id);
        
        return new CartViewModel
        {
            Items = cart.Items
               .Where(item => productsViews.ContainsKey(item.ProductId))
               .Select(item => (productsViews[item.ProductId], item.Quantity))!
        };
    }
}