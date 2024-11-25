using System;
using System.Collections.Generic;

namespace QueryMatrixDemo.Shared.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Payment? Payment { get; set; }
}
