using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Data;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }


    public DbSet<DataRecord> DataRecords { get; set; }


}
