using Grom.IntegrationTests.Models;
using Grom.Util.Exceptions;

namespace Grom.IntegrationTests.Tests.OrientDB.ConnectionTest;

[Collection("orientdb-collection")]
public class CreateConnectionTests
{
    [Fact]
    public async Task CreateConnectionWithBasicAuthShouldRun()
    {
        var node = new Person("Aegon", 90);
        await node.Persist();
    }
}

public class NoConnectionTests
{
    [Fact]
    public async Task CreateNeo4JConnectionBeforeInitializationShouldThrowException()
    {
        await Assert.ThrowsAsync<ConnectionNotInitializedException>(async () =>
        {
            var node = new Person("Aegon", 90);
            await node.Persist();
        });

    }
}