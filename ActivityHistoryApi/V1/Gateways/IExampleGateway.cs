using System.Collections.Generic;
using ActivityHistoryApi.V1.Domain;

namespace ActivityHistoryApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);
    }
}
