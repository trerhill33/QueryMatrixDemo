using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using QueryMatrixDemo.Client.Server.Context;
using QueryMatrixDemo.Core.Core.Models;
using QueryMatrixDemo.Core.Core.Operators;
using QueryMatrixDemo.Core.Core.Services;
using QueryMatrixDemo.Core.Core.Caching;
using QueryMatrixDemo.Core.Core.Operators.Providers;
using QueryMatrixDemo.Core.Core.Operators.Strategies;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;
using QueryMatrixDemo.Core.Core.Services.Interfaces;
using QueryMatrixDemo.Core.Core.Builders;

namespace QueryMatrixDemo.Tests.Integration;

[TestFixture]
public class QueryMatrixIntegrationTests
{
    private ApplicationDbContext _context;
    private IQueryExpressionBuilder _expressionBuilder;
    private IQueryMatrixApplier _queryMatrixApplier;
    private DbContextOptions<ApplicationDbContext> _options;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestQueryMatrixDb")
            .Options;

        // Initialize services with actual strategy implementations
        var propertyInfoCache = new PropertyInfoCache();
        var valueConverter = new ValueConverter();
        var operatorStrategyProvider = new OperatorStrategyProvider(
            new IValueOperatorStrategy[]
            {
                new EqualsOperatorStrategy(),
                new NotEqualOperatorStrategy(),
                new GreaterThanOperatorStrategy(),
                new LessThanOperatorStrategy(),
                new InOperatorStrategy(),
                new NotInOperatorStrategy()
            },
            new IColumnOperatorStrategy[]
            {
                new EqualsOperatorStrategy(),
                new NotEqualOperatorStrategy(),
                new GreaterThanOperatorStrategy(),
                new LessThanOperatorStrategy()
            }
        );

        _expressionBuilder = new QueryExpressionBuilder(
            operatorStrategyProvider,
            valueConverter,
            propertyInfoCache);

        _queryMatrixApplier = new QueryMatrixApplier(_expressionBuilder);
    }

    [SetUp]
    public void Setup()
    {
        _context = new ApplicationDbContext(_options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated(); // This will create the DB with your seed data
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public void Find_Products_By_Price_Range()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .WithLogicalOperator(QueryOperator.And)
            .AddCondition("Price", QueryOperator.GreaterThan, 20.0m)
            .AddCondition("Price", QueryOperator.LessThan, 1000.0m)
            .Build();

        // Act
        var results = _queryMatrixApplier.ApplyMatrix(_context.Products, matrix).ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.All(p => p.Price > 20.0m && p.Price < 1000.0m), Is.True);
    }

    [Test]
    public void Find_Products_By_Category_With_Min_Inventory()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .WithLogicalOperator(QueryOperator.And)
            .AddCondition("CategoryId", QueryOperator.Equal, 1) // Electronics
            .AddCondition("InventoryQuantity", QueryOperator.GreaterThanOrEqual, 50)
            .Build();

        // Act
        var results = _queryMatrixApplier
            .ApplyMatrix(_context.Products, matrix)
            .Include(p => p.Category)
            .ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().Category.Name, Is.EqualTo("Electronics"));
        Assert.That(results.First().InventoryQuantity, Is.GreaterThanOrEqual(50));
    }

    [Test]
    public void Find_Warehouses_With_Specific_Product_Inventory()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .AddCondition("Location", QueryOperator.Equal, "New York")
            .Build();

        // Act
        var results = _queryMatrixApplier
            .ApplyMatrix(_context.Warehouses, matrix)
            .Include(w => w.InventoryItems)
            .ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().Location, Is.EqualTo("New York"));
        Assert.That(results.First().InventoryItems.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Find_Customers_With_Orders()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .AddCondition("Name", QueryOperator.Equal, "John Doe")
            .Build();

        // Act
        var results = _queryMatrixApplier
            .ApplyMatrix(_context.Customers, matrix)
            .Include(c => c.Orders)
            .ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results.First().Name, Is.EqualTo("John Doe"));
    }

    [Test]
    public void Multiple_Or_Conditions_For_Products()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .WithLogicalOperator(QueryOperator.Or)
            .AddCondition("Price", QueryOperator.GreaterThan, 500m)
            .AddCondition("CategoryId", QueryOperator.Equal, 2)
            .Build();

        // Act
        var results = _queryMatrixApplier.ApplyMatrix(_context.Products, matrix).ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2)); // Should find expensive products OR clothing category
    }

    [Test]
    public void Invalid_Property_Should_Throw_Exception()
    {
        // Arrange
        var matrix = new QueryMatrixFluentBuilder()
            .AddCondition("NonExistentProperty", QueryOperator.Equal, "Value")
            .Build();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _queryMatrixApplier.ApplyMatrix(_context.Products, matrix).ToList();
        });
    }
}