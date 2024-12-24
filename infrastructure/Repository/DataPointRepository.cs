using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace infrastructure.Repository;

public class DataPointRepository : IDataPointRepository
{
    public Task AddDatapointAsync(DataPoint dataPoint)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangeAsync()
    {
        throw new NotImplementedException();
    }
}
