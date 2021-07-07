using ActivityHistoryApi.V1.Boundary.Response;
using ActivityHistoryApi.V1.Domain;
using ActivityHistoryApi.V1.Infrastructure;
using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityHistoryApi.Tests.V1.E2ETests.Fixtures
{
    public class ActivityHistoryFixture
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly IDynamoDBContext _dbContext;

        public List<ActivityHistoryDB> ActivityHistories { get; private set; } = new List<ActivityHistoryDB>();

        public Guid TargetId { get; private set; }

        public string InvalidTargetId { get; private set; }

        public const int MAXNOTES = 10;

        public ActivityHistoryResponseObject ActivityHistoryResponse { get; set; }

        public ActivityHistoryFixture(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (ActivityHistories.Any())
                    foreach (var activityHistory in ActivityHistories)
                        _dbContext.DeleteAsync(activityHistory).GetAwaiter().GetResult();

                _disposed = true;
            }
        }

        public void GivenActivityHistoryAlreadyExist()
        {
            GivenActivityHistoryAlreadyExist(MAXNOTES);
        }

        public void GivenActivityHistoryAlreadyExist(int count)
        {
            if (!ActivityHistories.Any())
            {
                var random = new Random();
                TargetId = Guid.NewGuid();
                Func<DateTime> funcDT = () => DateTime.UtcNow.AddDays(0 - random.Next(100));
                ActivityHistories.AddRange(_fixture.Build<ActivityHistoryDB>()
                                       .With(x => x.CreatedAt, funcDT)
                                       .With(x => x.TargetType, TargetType.person)
                                       .With(x => x.TargetId, TargetId)
                                       .CreateMany(count));
                foreach (var activityHistory in ActivityHistories)
                    _dbContext.SaveAsync(activityHistory).GetAwaiter().GetResult();
            }
        }

        public void GivenActivityHistoryWithMultiplePagesAlreadyExist()
        {
            GivenActivityHistoryAlreadyExist(35);
        }

        public void GivenATargetIdHasNoActivities()
        {
            TargetId = Guid.NewGuid();
        }

        public void GivenAnInvalidTargetId()
        {
            InvalidTargetId = "12345667890";
        }

    }
}
