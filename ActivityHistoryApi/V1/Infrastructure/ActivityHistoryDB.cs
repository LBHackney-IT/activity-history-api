using ActivityHistoryApi.V1.Domain;
using Amazon.DynamoDBv2.DataModel;
using Hackney.Core.DynamoDb.Converters;
using System;
using Type = ActivityHistoryApi.V1.Domain.Type;

namespace ActivityHistoryApi.V1.Infrastructure
{
    [DynamoDBTable("ActivityHistory", LowerCamelCaseProperties = true)]
    public class ActivityHistoryDB
    {
        [DynamoDBRangeKey]
        public Guid Id { get; set; }

        [DynamoDBHashKey]
        public Guid TargetId { get; set; }


        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<Type>))]
        public Type Type { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbEnumConverter<TargetType>))]
        public TargetType TargetType { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        public int TimetoLiveForRecordInDays { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<OldData>))]
        public OldData OldData { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<NewData>))]
        public NewData NewData { get; set; }

        [DynamoDBProperty(Converter = typeof(DynamoDbObjectConverter<AuthorDetails>))]
        public AuthorDetails AuthorDetails { get; set; }

    }
}
