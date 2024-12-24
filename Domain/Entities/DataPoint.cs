using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class DataPoint
{
   
    public int Id { get; set; }

    public string Name { get; set; }

    public int Value { get; set; }

    public DateTime Time { get; set; }

}
