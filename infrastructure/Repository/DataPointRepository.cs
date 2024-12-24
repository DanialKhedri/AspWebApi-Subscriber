using Domain.Entities;
using Domain.Interfaces.IRepository;
using infrastructure.Data;

namespace infrastructure.Repository;

public class DataPointRepository : IDataPointRepository
{


    #region Ctor
    private readonly DataContext _datacontext;

    public DataPointRepository(DataContext datacontext)
    {
        _datacontext = datacontext;
    }

    public Task AddDatapointAsync(DataRecord dataPoint)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangeAsync()
    {
        throw new NotImplementedException();
    }



    #endregion








}
