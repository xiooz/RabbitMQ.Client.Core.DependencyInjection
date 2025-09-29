using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using Testcontainers.RabbitMq;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests;

public class RabbitMqTestBase : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer;

    protected RabbitMqTestBase()
    {
        _rabbitMqContainer = new RabbitMqBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqContainer.DisposeAsync();
    }

    protected RabbitMqServiceOptions CreateOptions()
    {
        return new RabbitMqServiceOptions
        {
            HostName = _rabbitMqContainer.Hostname,
            Port = _rabbitMqContainer.GetMappedPublicPort(5672),
            UserName = RabbitMqBuilder.DefaultUsername,
            Password = RabbitMqBuilder.DefaultPassword,
            VirtualHost = "/"
        };
    }
}