using ActivityHistoryApi.V1.Boundary.Response;

namespace ActivityHistoryApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ActivityHistoryResponseObject Execute(int id);
    }
}
