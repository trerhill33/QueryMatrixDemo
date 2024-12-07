using Microsoft.EntityFrameworkCore;
using QueryMatrixDemo.Client.Server.Context;
using QueryMatrixDemo.Core.Core.Builders;
using QueryMatrixDemo.Core.Core.Caching;
using QueryMatrixDemo.Core.Core.Operators.Providers;
using QueryMatrixDemo.Core.Core.Operators.Strategies;
using QueryMatrixDemo.Core.Core.Operators.Strategies.Interfaces;
using QueryMatrixDemo.Core.Core.Services;
using QueryMatrixDemo.Core.Core.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions
        .MigrationsAssembly("QueryMatrixDemo.Server"))
        .EnableSensitiveDataLogging()
        .LogTo(log =>
        {
            if (log.Contains("Executing DbCommand"))
            {
                var sql = log[log.IndexOf("SELECT")..]; // Extract SQL query
                File.AppendAllText("sql-log.txt", sql + Environment.NewLine);
            }
        }); 
});

builder.Services.AddSingleton<IPropertyInfoCache, PropertyInfoCache>();

// Register Builders
builder.Services.AddTransient<IQueryMatrixFluentBuilder, QueryMatrixFluentBuilder>();

// Register Operator Strategies for Value-Based Operators
builder.Services.AddTransient<IValueOperatorStrategy, EqualsOperatorStrategy>();
builder.Services.AddTransient<IValueOperatorStrategy, GreaterThanOperatorStrategy>();
builder.Services.AddTransient<IValueOperatorStrategy, LessThanOperatorStrategy>();
builder.Services.AddTransient<IValueOperatorStrategy, NotEqualOperatorStrategy>();
builder.Services.AddTransient<IValueOperatorStrategy, InOperatorStrategy>();
builder.Services.AddTransient<IValueOperatorStrategy, NotInOperatorStrategy>();
// Register other value operator strategies as needed

// Register Operator Strategies for Column-Based Operators
builder.Services.AddTransient<IColumnOperatorStrategy, EqualsOperatorStrategy>();
builder.Services.AddTransient<IColumnOperatorStrategy, GreaterThanOperatorStrategy>();
builder.Services.AddTransient<IColumnOperatorStrategy, LessThanOperatorStrategy>();
builder.Services.AddTransient<IColumnOperatorStrategy, NotEqualOperatorStrategy>();


builder.Services.AddSingleton<IOperatorStrategyProvider, OperatorStrategyProvider>();

// Register Services
builder.Services.AddTransient<IQueryExpressionBuilder, QueryExpressionBuilder>();
builder.Services.AddTransient<IQueryMatrixApplier, QueryMatrixApplier>();
builder.Services.AddTransient<IValueConverter, ValueConverter>();
builder.Services.AddTransient<IQueryMatrixProcessor, QueryMatrixProcessor>();

builder.Services.AddLogging();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();