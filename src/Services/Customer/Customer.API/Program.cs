using Customer.API;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services;
using Customer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information($"Start {builder.Environment.ApplicationName} up");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<CustomerContext>(
           options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<ICustomerService, CustomerService>();
var app = builder.Build();

app.MapGet("/", () => "Hello");
app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomersAsync());
app.MapGet("/api/customer/{username}", async (string username, ICustomerService customerService) => await customerService.GetCustomerByUsernameAsync(username));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.SeedCustomerData();

app.Run();
