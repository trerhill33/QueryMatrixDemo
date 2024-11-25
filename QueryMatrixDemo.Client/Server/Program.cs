using Microsoft.EntityFrameworkCore;
using QueryMatrixDemo.Client.Server.Context;
using QueryMatrixDemo.Core.Interfaces;
using QueryMatrixDemo.Core.Services;

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

// Register services
builder.Services.AddScoped<IQueryMatrixService, QueryMatrixService>();

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