namespace WebStore.Infrastructure.Middleware;

public class TestMiddleware
{
    private readonly RequestDelegate _next;

    public TestMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // ReSharper disable once UnusedVariable
        var controllerName = context.Request.RouteValues["controller"];

        // ReSharper disable once UnusedVariable
        var actionName = context.Request.RouteValues["action"];

        var processingTask = _next(context);

        await processingTask;
    }
}