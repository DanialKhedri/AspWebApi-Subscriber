using Domain.Entities;

namespace Domain.Interfaces.IRepository;

public interface IDataPointRepository
{

    public Task AddDatapointAsync(DataRecord dataPoint);

    public Task SaveChangeAsync();





}
