using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroceryStoreManagementSystem.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; }
    public string Role { get; set; }
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class LoginDTO
{
    public string CustomerName { get; set; }
    public string Password { get; set; }
}
