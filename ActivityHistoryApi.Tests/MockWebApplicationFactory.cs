using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Hackney.Core.DynamoDb;
using Hackney.Core.Testing.DynamoDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ActivityHistoryApi.Tests
{
    public class MockWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly List<TableDef> _tables = new List<TableDef>
        {
            new TableDef
            {
                Name = "ActivityHistory",
                KeyName = "targetId",
                KeyType = ScalarAttributeType.S,
                RangeKeyName = "id",
                RangeKeyType = ScalarAttributeType.S,
                LocalSecondaryIndexes = new List<LocalSecondaryIndex>(new[]
                {
                    new LocalSecondaryIndex
                    {
                        IndexName = "ActivityHistoryByCreatedAt",
                        KeySchema = new List<KeySchemaElement>(new[]
                        {
                            new KeySchemaElement("targetId", KeyType.HASH),
                            new KeySchemaElement("createdAt", KeyType.RANGE)
                        }),
                        Projection = new Projection { ProjectionType = ProjectionType.ALL }
                    }
                })
            }
        };

        public IDynamoDbFixture DynamoDbFixture { get; private set; }
        public HttpClient Client { get; private set; }

        public MockWebApplicationFactory()
        {
            EnsureEnvVarConfigured("DynamoDb_LocalMode", "true");
            EnsureEnvVarConfigured("DynamoDb_LocalServiceUrl", "http://localhost:8000");

            Client = CreateClient();
        }

        private static void EnsureEnvVarConfigured(string name, string defaultValue)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                Environment.SetEnvironmentVariable(name, defaultValue);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(b => b.AddEnvironmentVariables())
                .UseStartup<Startup>();
            builder.ConfigureServices(services =>
            {
                services.ConfigureDynamoDB();
                services.ConfigureDynamoDbFixture();

                var serviceProvider = services.BuildServiceProvider();

                DynamoDbFixture = serviceProvider.GetRequiredService<IDynamoDbFixture>();
                DynamoDbFixture.EnsureTablesExist(_tables);
            });
        }
    }
}
