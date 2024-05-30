using SomeBasicEFApp.Web.ValueTypes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SomeBasicEFApp.Web.Entities;

///
public class Customer
{
    ///
    [JsonIgnore]
    public int Id { get; init; }
    ///<summary>Id column used by API</summary>
    ///<remarks>Cannot be used in LINQ</remarks>
    [JsonPropertyName("id")]
    public CustomerId CustomerId => new(Id);
    ///
    public string? Firstname { get; set; }
    ///
    public string? Lastname { get; set; }
    ///
    [JsonIgnore]
    public IList<Order> Orders { get; init; } = new List<Order>();
    ///
    [JsonIgnore]
    public int Version { get; init; }
}
