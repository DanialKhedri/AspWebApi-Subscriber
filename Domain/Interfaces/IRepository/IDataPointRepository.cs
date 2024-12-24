using Domain.Entities;

namespace Domain.Interfaces.IRepository;

public interface IDataPointRepository
{

    public Task AddDatapointAsync(DataPoint dataPoint);

    public Task SaveChangeAsync();





}
