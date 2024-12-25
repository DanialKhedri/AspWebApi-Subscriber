using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IDataService
{

    public Task<DataRecord?> GetDataByIdAsync(int id);

    public Task<IEnumerable<DataRecord>> GetPagedDataAsync(int pageNumber, int pageSize);

    public Task<IEnumerable<DataRecord>> GetDataByDateRangeAsync(DateTime startDate, DateTime endDate);

    //Get Date By Date
    public Task<IEnumerable<DataRecord>> GetDataBySpecificDateAsync(int year, int month, int day);

    public Task<IEnumerable<DataRecord>> GetDataBySpecificDateAndTimeAsync(DateTime specificDate);
 

}
