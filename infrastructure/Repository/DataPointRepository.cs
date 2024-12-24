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



    #endregion

    public async Task AddDatapointAsync(DataPoint dataPoint)
    {

        try
        {
            await _datacontext.DataPoints.AddAsync(dataPoint);
        }
        catch
        {


        }

    }

    public async Task SaveChangeAsync()
    {
        await _datacontext.SaveChangesAsync();
    }






}
