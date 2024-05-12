using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class ProductWarehouse
{
    //[EmailAddress]
    //[PhoneNumber]
    [Required]
    public int IdProduct { get; set; }
    [Required]
    
    public int IdWarehouse { get; set; }
    [Range(0,10000)]
    public int Amount { get; set; }
    [StringLength(50,MinimumLength = 3)]
    [DataType(DataType.DateTime)]
    public string CreatedAt { get; set; }
}