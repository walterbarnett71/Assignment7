using System.Collections.Generic;

namespace DBLibrary
{
    public interface IDBConnector
    {
        bool Ping();
        void InsertData(List<string> values);
        string ReadData(int index);
    }
}
