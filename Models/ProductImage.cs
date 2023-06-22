namespace ETIT.Models;

public class ProductImage
{
    public int Id { get; set; }
    public string Path { get; set; }
    public bool IsMain { get; set; }
    public virtual Product? Product{ get; set; }
}
