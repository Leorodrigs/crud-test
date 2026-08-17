using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

const string localApiBaseUrl = "http://localhost:5099";
const string mintlifyPreviewUrl = "http://localhost:3000";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MintlifyLocal", policy =>
        policy
            .WithOrigins(mintlifyPreviewUrl)
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Documentation Demo API",
            Description = "Simple API created to compare OpenAPI documentation interfaces.",
            Version = "v1"
        };
        document.Servers =
        [
            new OpenApiServer
            {
                Url = localApiBaseUrl,
                Description = "Local ASP.NET Core API"
            }
        ];

        return Task.CompletedTask;
    });
});

var app = builder.Build();

const string openApiDocumentUrl = "/openapi/v1.json";

app.UseCors("MintlifyLocal");

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "Documentation Demo API - Swagger UI";
    options.SwaggerEndpoint(openApiDocumentUrl, "Documentation Demo API v1");
});

app.MapScalarApiReference(options =>
    options
        .WithTitle("Documentation Demo API - Scalar")
        .WithOpenApiRoutePattern("/openapi/{documentName}.json"))
    .ExcludeFromDescription();

app.MapControllers();

app.MapGet("/mintlify", () => Results.Redirect(mintlifyPreviewUrl))
    .ExcludeFromDescription();

var documentationPages = new Dictionary<string, string>
{
    ["/rapidoc"] = "rapidoc.html",
    ["/stoplight"] = "stoplight.html",
    ["/redoc"] = "redoc.html",
    ["/openapi-explorer"] = "openapi-explorer.html"
};

foreach (var (route, fileName) in documentationPages)
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "docs", fileName);

    app.MapGet(route, () => Results.File(filePath, "text/html; charset=utf-8"))
        .ExcludeFromDescription();
}

app.Run();
