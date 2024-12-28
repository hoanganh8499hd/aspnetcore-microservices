using Basket.API;
using Basket.API.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddAppConfigurations();
//builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

// Add services to the container.
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

builder.Services.ConfigureServices();
//Mass Transit
builder.Services.ConfigureMassTransit();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore-swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();