using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces.IRepository;

public interface IDataRepository
{


    public  Task<DataRecord?> GetDataByIdAsync(int id);


    public Task<IEnumerable<DataRecord>> GetPagedDataAsync(int pageNumber, int pageSize);


    public Task<IEnumerable<DataRecord>> GetDataByDateRangeAsync(DateTime startDate, DateTime endDate);


    //Get Date By Date
    public Task<IEnumerable<DataRecord>> GetDataBySpecificDateAsync(int year, int month, int day);


    // GET DataRecords By Specific Date, Hour, and Minute
    public Task<IEnumerable<DataRecord>> GetDataBySpecificDateAndTimeAsync(DateTime specificDate);

  

}
