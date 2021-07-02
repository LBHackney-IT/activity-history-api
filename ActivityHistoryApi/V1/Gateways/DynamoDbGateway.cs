using Amazon.DynamoDBv2.DataModel;
using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Factories;
using ActivityHistoryApi.V1.Infrastructure;

namespace ActivityHistoryApi.V1.Gateways
{
    public class DynamoDbGateway : IExampleGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;

        public DynamoDbGateway(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public ActivityHistoryEntity GetEntityById(int id)
        {
            var result = _dynamoDbContext.LoadAsync<ActivityHistoryDB>(id).GetAwaiter().GetResult();
            return result?.ToDomain();
        }
    }
}
