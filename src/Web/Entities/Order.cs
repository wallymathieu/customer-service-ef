using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using SomeBasicEFApp.Web.ValueTypes;

namespace SomeBasicEFApp.Web.Entities;

///
public class Order
{
    ///
    [JsonIgnore]
    public Customer? Customer { get; set; }
    ///
    public DateTime OrderDate { get; init; }
    ///
    [JsonIgnore]
    public int Id { get; init; }
    ///<summary>Id column used by API</summary>
    ///<remarks>Cannot be used in LINQ</remarks>
    [JsonPropertyName("id")]
    public OrderId OrderId => new(Id);

    ///
    [JsonIgnore]
    public int Version { get; init; }
    ///
    public IList<Product> Products { get; init; } = new List<Product>();

}
