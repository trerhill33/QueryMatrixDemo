using System;
using System.Collections.Generic;

namespace QueryMatrixDemo.Shared.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
