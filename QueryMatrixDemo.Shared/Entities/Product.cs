using System;
using System.Collections.Generic;

namespace QueryMatrixDemo.Shared.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int InventoryQuantity { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
