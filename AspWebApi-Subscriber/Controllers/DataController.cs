using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/data")]
[ApiController]
public class DataController : ControllerBase
{

    #region Ctor
    private readonly IDataService _dataService;

    // Constructor
    public DataController(IDataService dataService)
    {
        _dataService = dataService;
    }

    #endregion


    // GET api/data/GetById/1
    // GET api/data/GetById/{id}
    [Authorize]
    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> GetDataByIdAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { Message = "Invalid ID, it must be greater than zero" });
        }

        var data = await _dataService.GetDataByIdAsync(id);

        if (data == null)
        {
            return NotFound(new { Message = "Data record not found" });
        }

        return Ok(data);
    }



    // GET api/data/paged?pageNumber=1&pageSize=10
    // GET api/data/paged?pageNumber={pageNumber}&pageSize={pageSize}
    [Authorize]
    [HttpGet("paged")]
    public async Task<IActionResult> GetPagedDataAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest(new { Message = "Page number and page size must be greater than zero" });
        }

        var data = await _dataService.GetPagedDataAsync(pageNumber, pageSize);

        if (data == null || !data.Any())
        {
            return NotFound(new { Message = "No data found for the specified page" });
        }

        return Ok(data);
    }


    //GET  api/data/date-range? startDate = 2023 - 01 - 01T00:00:00&endDate=2023-12-31T23:59:59
    // GET api/data/date-range?startDate={startDate}&endDate={endDate}
    [Authorize]
    [HttpGet("date-range")]
    public async Task<IActionResult> GetDataByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { Message = "Start date cannot be greater than end date" });
        }

        var data = await _dataService.GetDataByDateRangeAsync(startDate, endDate);

        if (data == null || !data.Any())
        {
            return NotFound(new { Message = "No data found for the specified date range" });
        }
        return Ok(data);
    }


    // GET /api/data/specific-date?year=2024&month=12&day=26
    // GET api/data/specific-date?year={year}&month={month}&day={day}
    [Authorize]
    [HttpGet("specific-date")]
    public async Task<IActionResult> GetDataBySpecificDateAsync(int year, int month, int day)
    {
        if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31)
        {
            return BadRequest(new { Message = "Invalid date parameters" });
        }

        var data = await _dataService.GetDataBySpecificDateAsync(year, month, day);

        if (data == null || !data.Any())
        {
            return NotFound(new { Message = "No data found for the specified date" });
        }

        return Ok(data);
    }



    // GET /api/data/specific-date-time?specificDate=2024-12-26T04:41:42
    [Authorize]
    [HttpGet("specific-date-time")]
    public async Task<IActionResult> GetDataBySpecificDateAndTimeAsync(DateTime specificDate)
    {
        if (specificDate == default)
        {
            return BadRequest(new { Message = "Invalid date format" });
        }

        var data = await _dataService.GetDataBySpecificDateAndTimeAsync(specificDate);

        if (data == null || !data.Any())
        {
            return NotFound(new { Message = "No data found for the specified date and time" });
        }

        return Ok(data);
    }


}
