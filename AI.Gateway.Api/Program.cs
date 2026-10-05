var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add ProblemDetails services and register typed exception handler
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AI.Gateway.Api.GlobalExceptionHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Register OpenAI client and prompt analyzer
// OpenAI API key must be provided via environment variable OPENAI_API_KEY or user secrets (not committed)
var openAiApiKey = builder.Configuration["OPENAI_API_KEY"];
if (string.IsNullOrWhiteSpace(openAiApiKey))
{
    // Do not silently continue without API key; fail fast so configuration is explicit for runtime.
    throw new InvalidOperationException("OPENAI_API_KEY configuration value is required to run with OpenAIPromptAnalyzer.");
}

// Register concrete ResponsesClient from the OpenAI SDK as a singleton (long-lived, thread-safe)
builder.Services.AddSingleton(sp =>
{
    // Construct ResponsesClient directly with the API key
    // Suppress OPENAI001 warnings for usage of SDK evaluation types
    #pragma warning disable OPENAI001
    return new OpenAI.Responses.ResponsesClient(openAiApiKey);
    #pragma warning restore OPENAI001
});
// Register adapter that wraps the SDK ResponsesClient
builder.Services.AddSingleton<AI.Gateway.Api.Services.IOpenAIResponsesClient, AI.Gateway.Api.Services.OpenAIResponsesClient>();

// Register OpenAIPromptAnalyzer as the active IPromptAnalyzer implementation
builder.Services.AddSingleton<AI.Gateway.Api.Services.IPromptAnalyzer, AI.Gateway.Api.Services.OpenAIPromptAnalyzer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable the framework exception handler which will resolve the typed handler
app.UseExceptionHandler(new ExceptionHandlerOptions
{
    SuppressDiagnosticsCallback = _ => false
});

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
