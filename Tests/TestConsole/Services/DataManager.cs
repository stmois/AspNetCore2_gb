using TestConsole.Data;
using TestConsole.Services.Interfaces;

namespace TestConsole.Services
{
    public class DataManager : IDataManager, IDisposable
    {
        private readonly IDataProcessor _processor;

        public DataManager(IDataProcessor processor)
        {
            _processor = processor;
        }

        public void ProcessData(IEnumerable<DataValue> values)
        {
            foreach (var value in values)
            {
                _processor.Process(value);
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Менеджер обработки данных уничтожен");
        }
    }
}
