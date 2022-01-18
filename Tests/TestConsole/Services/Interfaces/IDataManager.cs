using TestConsole.Data;

namespace TestConsole.Services.Interfaces
{
    public interface IDataManager
    {
        void ProcessData(IEnumerable<DataValue> values);
    }


    public interface IDataProcessor
    {
        void Process(DataValue value);
    }
}
