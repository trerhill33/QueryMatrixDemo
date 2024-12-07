using NUnit.Framework;
using Core.Operators;
using Core.Operators.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryMatrixDemo.Core.Core.Operators.Strategies;

namespace QueryMatrixDemo.Tests.Operators.Strategies
{
    [TestFixture]
    public class InOperatorStrategyTests
    {
        private InOperatorStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _strategy = new InOperatorStrategy();
        }

        [Test]
        [TestCase("_in", true)]
        [TestCase("_nin", false)]
        [TestCase("_eq", false)]
        [TestCase("_gt", false)]
        public void CanHandle_ShouldReturnExpectedResult(string operatorValue, bool expected)
        {
            // Arrange
            var op = QueryOperatorExtensions.FromString(operatorValue);

            // Act
            var canHandle = _strategy.CanHandle(op);

            // Assert
            Assert.That(canHandle, Is.EqualTo(expected), $"CanHandle failed for operator '{operatorValue}'. Expected: {expected}, Actual: {canHandle}");
        }

        [Test]
        public void BuildValueExpression_WithValidCollection_ShouldReturnContainsExpression()
        {
            // Arrange
            var parameter = Expression.Parameter(typeof(string), "x");
            var property = Expression.Property(parameter, "Category");
            var categories = new List<object> { "Electronics", "Books", "Home Appliances" };
            var constant = Expression.Constant(categories, typeof(IEnumerable<object>));

            // Act
            var expression = _strategy.BuildValueExpression(property, constant);

            // Assert
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(string));

            var expected = Expression.Call(containsMethod, Expression.Constant(categories), property);
            Assert.That(expression.ToString(), Is.EqualTo(expected.ToString()), $"BuildValueExpression failed. Expected: {expected}, Actual: {expression}");
        }

        [Test]
        public void BuildValueExpression_WithInvalidValue_ShouldThrowArgumentException()
        {
            // Arrange
            var parameter = Expression.Parameter(typeof(int), "x");
            var property = Expression.Property(parameter, "Age");
            var invalidValue = Expression.Constant(25, typeof(int)); // Should be IEnumerable<int>

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => _strategy.BuildValueExpression(property, invalidValue));
            Assert.That(ex.Message, Does.Contain("Value for 'In' operator must be an enumerable collection."), "Exception message mismatch.");
        }

        [Test]
        public void BuildColumnExpression_ShouldThrowNotSupportedException()
        {
            // Arrange
            var parameter1 = Expression.Parameter(typeof(int), "x");
            var property1 = Expression.Property(parameter1, "Age");
            var parameter2 = Expression.Parameter(typeof(int), "y");
            var property2 = Expression.Property(parameter2, "MinimumAge");

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => _strategy.BuildValueExpression(property1, property2));
            Assert.That(ex.Message, Does.Contain("not supported"), "Exception message mismatch.");
        }
    }
}
