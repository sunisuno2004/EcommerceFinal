var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// CORS
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "http://127.0.0.1:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// --------------------------------------------------
// YARP Reverse Proxy
// --------------------------------------------------
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration
            .GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.MapGet("/", () =>
    Results.Ok(new
    {
        service = "ECommerce API Gateway",
        status = "Running"
    }));

app.MapReverseProxy();

app.Run();