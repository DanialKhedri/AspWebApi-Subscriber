using Domain.Entities;
using Domain.Interfaces.IRepository;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace infrastructure.Repository;

public class DataRepository : IDataRepository
{

    #region Ctor
    private readonly DataContext _context;
    private readonly ILogger<DataRepository> logger;
    public DataRepository(DataContext context, ILogger<DataRepository> logger)
    {
        _context = context;
        this.logger = logger;
    }

    #endregion


    public async Task<DataRecord?> GetDataByIdAsync(int id)
    {

        try
        {

            var datarecord = await _context.DataRecords
                     .FirstOrDefaultAsync(d => d.Id == id);

            return datarecord;

        }
        catch (Exception ex)
        {

            logger.LogError("Error Message :" + ex.Message);
            return null;

        }

    }


    public async Task<IEnumerable<DataRecord>> GetPagedDataAsync(int pageNumber, int pageSize)
    {

        try
        {
            return await _context.DataRecords
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        }
        catch (Exception ex)
        {

            logger.LogError("Error Message :" + ex.Message);
            return Enumerable.Empty<DataRecord>();
        }


    }


    public async Task<IEnumerable<DataRecord>> GetDataByDateRangeAsync(DateTime startDate, DateTime endDate)
    {


        try
        {
            return await _context.DataRecords
                       .Where(d => d.Time >= startDate && d.Time <= endDate)
                       .ToListAsync();
        }
        catch (Exception ex)
        {

            logger.LogError("Error Message :" + ex.Message);
            return Enumerable.Empty<DataRecord>();

        }

    }


    //Get Date By Date
    public async Task<IEnumerable<DataRecord>> GetDataBySpecificDateAsync(int year, int month, int day)
    {
        try
        {
            // تاریخ کامل را از سال، ماه و روز بسازید
            var specificDate = new DateTime(year, month, day);

            // جستجو برای رکوردهایی که تاریخ مشابه دارند
            return await _context.DataRecords
                        .Where(d => d.Time.Date == specificDate.Date) // مقایسه فقط قسمت تاریخ
                        .ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("Error Message :" + ex.Message);
            return Enumerable.Empty<DataRecord>();
        }
    }


    // GET DataRecords By Specific Date, Hour, and Minute
    public async Task<IEnumerable<DataRecord>> GetDataBySpecificDateAndTimeAsync(DateTime specificDate)
    {
        try
        {
            int targetHour = specificDate.Hour;
            int targetMinute = specificDate.Minute;

            return await _context.DataRecords
                .Where(d => d.Time.Year == specificDate.Year &&
                            d.Time.Month == specificDate.Month &&
                            d.Time.Day == specificDate.Day &&
                            d.Time.Hour == targetHour &&       
                            d.Time.Minute == targetMinute)    
                .ToListAsync();

        }
        catch (Exception ex)
        {

            logger.LogError($"Error Message: {ex.Message}");
            return Enumerable.Empty<DataRecord>();

        }
    }


}
