using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityHistoryApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        person,
        asset,
        tenure
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Type
    {
        create,
        update,
        delete
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Title
    {
        Dr,
        Master,
        Miss,
        Mr,
        Mrs,
        Ms,
        Other,
        Rabbi,
        Reverend
    }
}
