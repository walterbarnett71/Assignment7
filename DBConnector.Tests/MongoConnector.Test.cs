using Testcontainers.MongoDb;

namespace DBConnector.Tests;

public class MongoConnector : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;

    public MongoConnector()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync();
    }

    [Fact]
    public async Task should_ping_db_successfully()
    {
        // Given
        IDBConnector connector = new DBConnector.MongoConnector(_mongoDbContainer.GetConnectionString());

        // When
        bool ping_result = await connector.ping();

        // Then
        Assert.True(ping_result);
    }

    [Fact]
    public async Task should_fail_missing_db()
    {
        // Given
        var connector = new DBConnector.MongoConnector("");

        // When
        bool ping_result = await connector.ping();

        // Then
        Assert.False(ping_result);
    }
}
