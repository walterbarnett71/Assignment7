namespace DBConnector;

public interface IDBConnector
{
    public Task<bool> ping();
}

