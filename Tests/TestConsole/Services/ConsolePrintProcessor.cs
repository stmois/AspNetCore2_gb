using TestConsole.Data;
using TestConsole.Services.Interfaces;

namespace TestConsole.Services
{
    public class ConsolePrintProcessor : IDataProcessor
    {
        public void Process(DataValue value)
        {
            Console.WriteLine("[{0}]({1}):{2}", value.Id, value.Time, value.Value);
        }
    }
}
