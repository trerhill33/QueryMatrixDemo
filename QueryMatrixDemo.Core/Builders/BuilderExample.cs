//using QueryMatrixDemo.Core.Interfaces;
//using QueryMatrixDemo.Core.Operators;
//using QueryMatrixDemo.Shared.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace QueryMatrixDemo.Core.Builders
//{

//    // Example usage showing the full flow
//    public class BuilderExample
//    {
//        private readonly IQueryMatrixService _queryMatrixService;
//        private readonly ApplicationDbContext _context;

//        public async Task<IEnumerable<>> GetExpensiveElectronicsProducts()
//        {
//            // 1. Build the query structure using QueryMatrixBuilder
//            var matrix = new QueryMatrixBuilder()
//                .WithLogicalOperator(QueryOperator.And)
//                .AddCondition("Category", QueryOperator.Equal, "Electronics")
//                .AddCondition("Price", QueryOperator.GreaterThan, 1000)
//                .AddCondition("IsActive", QueryOperator.Equal, true)
//                .Build();

//            // 2. Use the service to apply this matrix to a query
//            var query = _context.Products.AsQueryable();
//            var filteredQuery = _queryMatrixService.ApplyMatrix(query, matrix);

//            // 3. Execute the query
//            return await filteredQuery.ToListAsync();
//        }

//        // More complex example with nested conditions
//        public async Task<IEnumerable<Product>> GetComplexProductQuery()
//        {
//            // Build a nested matrix for the OR conditions
//            var priceMatrix = new QueryMatrixBuilder()
//                .WithLogicalOperator(QueryOperator.Or)
//                .AddCondition("Price", QueryOperator.GreaterThan, 1000)
//                .AddCondition("DiscountPrice", QueryOperator.LessThan, 500)
//                .Build();

//            // Main matrix combining multiple conditions
//            var matrix = new QueryMatrixBuilder()
//                .WithLogicalOperator(QueryOperator.And)
//                .AddCondition("Category", QueryOperator.Equal, "Electronics")
//                .AddCondition("IsActive", QueryOperator.Equal, true)
//                .AddNestedMatrix(priceMatrix)  // Add the nested OR conditions
//                .Build();

//            // Apply and execute
//            var query = _context.Products.AsQueryable();
//            var filteredQuery = _queryMatrixService.ApplyMatrix(query, matrix);
//            return await filteredQuery.ToListAsync();
//        }

//        // Example showing column comparison
//        public async Task<IEnumerable<Product>> GetDiscountedProducts()
//        {
//            var matrix = new QueryMatrixBuilder()
//                .WithLogicalOperator(QueryOperator.And)
//                .AddColumnComparison("DiscountPrice", QueryOperator.ColumnLessThan, "Price")
//                .AddCondition("IsActive", QueryOperator.Equal, true)
//                .Build();

//            var query = _context.Products.AsQueryable();
//            var filteredQuery = _queryMatrixService.ApplyMatrix(query, matrix);
//            return await filteredQuery.ToListAsync();
//        }
//    }
//}
