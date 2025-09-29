using System.Collections.Generic;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.IntegrationTests
{
    public class RabbitMqConnectionFactoryTests : RabbitMqTestBase
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnection(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "anotherHost",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithConnectionName(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostName = "anotherHost",
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithTcpEndpoints(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                TcpEndpoints = new List<RabbitMqTcpEndpoint>
                {
                    new()
                    {
                        HostName = "anotherHost"
                    }
                },
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithHostNames(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "anotherHost" },
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void ShouldProperlyRetryCreatingInitialConnectionWithHostNamesAndNamedConnection(int retries)
        {
            var connectionOptions = new RabbitMqServiceOptions
            {
                HostNames = new List<string> { "anotherHost" },
                ClientProvidedName = "connectionName",
                InitialConnectionRetries = retries,
                InitialConnectionRetryTimeoutMilliseconds = 20
            };
            ExecuteUnsuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnection()
        {
            RabbitMqServiceOptions connectionOptions = base.CreateOptions();
            connectionOptions.InitialConnectionRetries = 1;
            connectionOptions.InitialConnectionRetryTimeoutMilliseconds = 20;
            
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithConnectionName()
        {
            RabbitMqServiceOptions connectionOptions = base.CreateOptions();
            connectionOptions.ClientProvidedName = "connectionName";
            connectionOptions.InitialConnectionRetries = 3;
            connectionOptions.InitialConnectionRetryTimeoutMilliseconds = 20;
            
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithTcpEndpoints()
        {
            RabbitMqServiceOptions connectionOptions = base.CreateOptions();
            connectionOptions.TcpEndpoints = new List<RabbitMqTcpEndpoint>
            {
                new()
                {
                    HostName = connectionOptions.HostName,
                    Port = connectionOptions.Port
                }
            };
            connectionOptions.HostName = null!;
            connectionOptions.HostNames.Clear();
            connectionOptions.InitialConnectionRetries = 3;
            connectionOptions.InitialConnectionRetryTimeoutMilliseconds = 20;
            
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithHostNames()
        {
            RabbitMqServiceOptions connectionOptions = base.CreateOptions();
            connectionOptions.HostNames = new List<string> { connectionOptions.HostName };
            connectionOptions.HostName = null!;
            connectionOptions.InitialConnectionRetries = 3;
            connectionOptions.InitialConnectionRetryTimeoutMilliseconds = 20;
            
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        [Fact]
        public void ShouldProperlyCreateInitialConnectionWithHostNamesAndNamedConnection()
        {
            RabbitMqServiceOptions connectionOptions = base.CreateOptions();
            connectionOptions.HostNames = new List<string> { connectionOptions.HostName };
            connectionOptions.ClientProvidedName = "connectionName";
            connectionOptions.InitialConnectionRetries = 3;
            connectionOptions.InitialConnectionRetryTimeoutMilliseconds = 20;
            
            ExecuteSuccessfulConnectionCreationAndAssertResults(connectionOptions);
        }

        private static void ExecuteUnsuccessfulConnectionCreationAndAssertResults(RabbitMqServiceOptions connectionOptions)
        {
            var connectionFactory = new RabbitMqConnectionFactory();
            var exception = Assert.Throws<InitialConnectionException>(() => connectionFactory.CreateRabbitMqConnection(connectionOptions));
            Assert.Equal(connectionOptions.InitialConnectionRetries, exception.NumberOfRetries);
        }

        private static void ExecuteSuccessfulConnectionCreationAndAssertResults(RabbitMqServiceOptions connectionOptions)
        {
            var connectionFactory = new RabbitMqConnectionFactory();
            using var connection = connectionFactory.CreateRabbitMqConnection(connectionOptions);
            Assert.True(connection!.IsOpen);
        }
    }
}