using Azure.Storage.Samples.API.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<AzureOptions>(builder.Configuration.GetSection(AzureOptions.Azure));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(optisons =>
    {
        optisons.Theme = ScalarTheme.BluePlanet;
    });

    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

