using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueryMatrixDemo.Client.Server.Context;
using QueryMatrixDemo.Core.Interfaces;
using QueryMatrixDemo.Core.Models;
using QueryMatrixDemo.Shared.Entities;

namespace QueryMatrixDemo.Client.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueryController(ApplicationDbContext context, IQueryMatrixService queryMatrixService)
    : ControllerBase
{
    [HttpPost("{entityType}")]
    public async Task<IActionResult> ExecuteQuery(string entityType, [FromBody] QueryMatrix matrix)
    {
        object? results;
        try
        {
            results = entityType switch
            {
                "Product" => await ExecuteTypedQuery<Product>(matrix),
                "Category" => await ExecuteTypedQuery<Category>(matrix),
                "Order" => await ExecuteTypedQuery<Order>(matrix),
                "Customer" => await ExecuteTypedQuery<Customer>(matrix),
                "Warehouse" => await ExecuteTypedQuery<Warehouse>(matrix),
                "InventoryItem" => await ExecuteTypedQuery<InventoryItem>(matrix),
                _ => throw new ArgumentException($"Unknown entity type: {entityType}"),
            };
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest($"Query execution failed: {ex.Message}");
        }
    }

    private async Task<IEnumerable<T>> ExecuteTypedQuery<T>(QueryMatrix matrix) where T : class
    {
        var query = context.Set<T>().AsQueryable();
        var filteredQuery = queryMatrixService.ApplyMatrix(query, matrix);
        return await filteredQuery.ToListAsync();
    }
}