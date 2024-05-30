using System.Collections.Generic;
using System.Text.Json.Serialization;
using SomeBasicEFApp.Web.ValueTypes;

namespace SomeBasicEFApp.Web.Entities;

///
public class Product
{
    ///
    public float Cost { get; init; }
    ///
    public string? Name { get; init; }
    ///
    [JsonIgnore]
    public IList<Order> Orders { get; init; } = new List<Order>();
    ///
    [JsonIgnore]
    public int Id { get; init; }
    ///<summary>Id column used by API</summary>
    ///<remarks>Cannot be used in LINQ</remarks>
    [JsonPropertyName("id")]
    public ProductId ProductId => new(Id);
    ///
    [JsonIgnore]
    public int Version { get; init; }
    ///
    public ProductType Type { get; init; } = new ProductType(null);
}
