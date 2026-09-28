var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Register prompt analyzer
builder.Services.AddSingleton<AI.Gateway.Api.Services.IPromptAnalyzer, AI.Gateway.Api.Services.FakePromptAnalyzer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // expose built-in OpenAPI document
    app.MapOpenApi();

    // Serve Swagger UI (development only), pointing to MapOpenApi's document
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AI.Gateway API v1");
        options.RoutePrefix = "swagger"; // serve UI at /swagger
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
