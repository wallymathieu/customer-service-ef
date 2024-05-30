namespace SomeBasicEFApp.Web.Entities;

///
public class ProductOrder
{
    ///
    public int Id { get; init; }
    ///
    public Order? Order { get; init; }
    ///
    public Product? Product { get; init; }
}
