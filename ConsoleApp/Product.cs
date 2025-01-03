#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    public int ID { get; set; }
    public string Name { get; set; }

    [Column("Amount")]
    public int? RestValue { get; set; }

    public override string ToString()
    {
        return $"Product Name: {Name} - Product ID: {ID} - Rest value: {RestValue}";
    }
}
