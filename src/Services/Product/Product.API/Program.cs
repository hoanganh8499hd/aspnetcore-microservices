
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);


Log.Information($"Start Product API up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();
    // Add services to the container.

    builder.Services.AddInfrastructure(builder.Configuration);
    //builder.Services.AddControllers();
    //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //builder.Services.AddEndpointsApiExplorer();
    //builder.Services.AddSwaggerGen();

    var app = builder.Build();
    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}
    app.UseInfrastructure();

    //app.UseHttpsRedirection();

    //app.UseAuthorization();

    //app.MapControllers();

    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    })
       .Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}

