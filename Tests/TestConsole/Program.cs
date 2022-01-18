using Microsoft.Extensions.DependencyInjection;
using TestConsole.Data;
using TestConsole.Services;
using TestConsole.Services.Interfaces;

var serviceCollection = new ServiceCollection();

serviceCollection.AddScoped<IDataManager, DataManager>();
serviceCollection.AddScoped<IDataProcessor, ConsolePrintProcessor>();

var provider = serviceCollection.BuildServiceProvider();
var service = provider.GetRequiredService<IDataManager>();

using (var scope = provider.CreateScope())
{
    var scopeProvider = scope.ServiceProvider;
    var service2 = scopeProvider.GetRequiredService<IDataManager>();

    // ReSharper disable once UnusedVariable
    var isEquals = ReferenceEquals(service, service2);
}

var data = Enumerable.Range(1, 100).Select(i => new DataValue
{
    Id = i,
    Value = $"Value-{i}",
    Time = DateTime.Now.AddHours(-i * 10),
});

service.ProcessData(data);

Console.ReadLine();

provider.GetRequiredService<IDataManager>();
