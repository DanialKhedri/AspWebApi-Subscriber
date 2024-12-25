using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces.IRepository;

namespace Application.Services.Implements;

public class DataService : IDataService
{

    #region Ctor
    private readonly IDataRepository _datarepository;

    public DataService(IDataRepository datarepository)
    {
        _datarepository = datarepository;
    }

    #endregion


    public async Task<DataRecord?> GetDataByIdAsync(int id)
    {

        return await _datarepository.GetDataByIdAsync(id);

    }


    public async Task<IEnumerable<DataRecord>> GetPagedDataAsync(int pageNumber, int pageSize)
    {

        return await _datarepository.GetPagedDataAsync(pageNumber, pageSize);


    }


    public async Task<IEnumerable<DataRecord>> GetDataByDateRangeAsync(DateTime startDate, DateTime endDate)
    {


        return await _datarepository.GetDataByDateRangeAsync(startDate, endDate);

    }


    //Get Date By Date
    public async Task<IEnumerable<DataRecord>> GetDataBySpecificDateAsync(int year, int month, int day)
    {


        return await _datarepository.GetDataBySpecificDateAsync(year, month, day);

    }



    public async Task<IEnumerable<DataRecord>> GetDataBySpecificDateAndTimeAsync(DateTime specificDate)
    {


        return await _datarepository.GetDataBySpecificDateAndTimeAsync(specificDate);

    }


}
