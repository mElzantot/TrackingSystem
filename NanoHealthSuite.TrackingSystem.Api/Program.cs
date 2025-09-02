using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using NanoHealthSuite.TrackingSystem;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Workflow Tracking API",
        Version = "v1",
        Description = "API for managing workflows and process tracking"
    });
    c.UseInlineDefinitionsForEnums();
});
    
builder.Services.AddWorkflowServices(builder.Configuration);

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "workflow/swagger/{documentname}/swagger.json";
    });
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/workflow/swagger/v1/swagger.json", "workflow docs V1");
        options.RoutePrefix = "workflow/swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
