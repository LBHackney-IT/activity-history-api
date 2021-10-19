using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Hackney.Core.Logging;
using System.Threading.Tasks;
using Hackney.Core.DynamoDb;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;
using Hackney.Shared.ActivityHistory.Domain;
using Hackney.Shared.ActivityHistory.Infrastructure;
using Hackney.Shared.ActivityHistory.Boundary.Request;
using Hackney.Shared.ActivityHistory.Factories;

namespace ActivityHistoryApi.V1.Gateways
{
    public class ActivityHistoryDbGateway : IActivityHistoryGateway
    {
        private const int MAX_RESULTS = 10;
        private const string GETACTIVITIESBYTARGETIDINDEX = "ActivityHistoryByCreatedAt";
        private const string TARGETID = "targetId";

        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<ActivityHistoryDbGateway> _logger;

        public ActivityHistoryDbGateway(IDynamoDBContext dynamoDbContext, ILogger<ActivityHistoryDbGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }
        [LogCall]
        public async Task<PagedResult<ActivityHistoryEntity>> GetByTargetIdAsync(GetActivityHistoryByTargetIdQuery query)
        {
            int pageSize = query.PageSize.HasValue ? query.PageSize.Value : MAX_RESULTS;
            var activityHistoryDB = new List<ActivityHistoryDB>();
            var table = _dynamoDbContext.GetTargetTable<ActivityHistoryDB>();

            var queryConfig = new QueryOperationConfig
            {
                IndexName = GETACTIVITIESBYTARGETIDINDEX,
                BackwardSearch = true,
                ConsistentRead = true,
                Limit = pageSize,
                PaginationToken = PaginationDetails.DecodeToken(query.PaginationToken),
                Filter = new QueryFilter(TARGETID, QueryOperator.Equal, query.TargetId)
            };
            var search = table.Query(queryConfig);

            _logger.LogDebug($"Querying {queryConfig.IndexName} index for targetId {query.TargetId}");
            var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);

            var paginationToken = search.PaginationToken;
            if (resultsSet.Any())
            {
                activityHistoryDB.AddRange(_dynamoDbContext.FromDocuments<ActivityHistoryDB>(resultsSet));

                // Look ahead for any more, but only if we have a token
                if (!string.IsNullOrEmpty(PaginationDetails.EncodeToken(paginationToken)))
                {
                    queryConfig.PaginationToken = paginationToken;
                    queryConfig.Limit = 1;
                    search = table.Query(queryConfig);
                    resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);
                    if (!resultsSet.Any())
                        paginationToken = null;
                }
            }

            return new PagedResult<ActivityHistoryEntity>(activityHistoryDB.Select(x => x.ToDomain()), new PaginationDetails(paginationToken));
        }
    }
}
