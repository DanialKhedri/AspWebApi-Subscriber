using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.DataContext;

public class DataContext : DbContext
{

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }


    public DbSet<DataPoint> DataPoints { get; set; }


}
