using ECommerce.OrderService.Repositories;

using ECommerce.OrderService.Clients;
using ECommerce.OrderService.Data;
using ECommerce.OrderService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// CONTROLLERS
// --------------------------------------------------
builder.Services.AddControllers();

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------
// DATABASE
// OrderService owns only OrderDb
// --------------------------------------------------
builder.Services.AddDbContext<OrderDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString("OrderDb")));

// --------------------------------------------------
// REPOSITORIES
// --------------------------------------------------
builder.Services.AddScoped<
    ICartRepository,
    CartRepository>();

builder.Services.AddScoped<
    IOrderRepository,
    OrderRepository>();

// --------------------------------------------------
// UNIT OF WORK
// Used for OrderDb transaction handling
// --------------------------------------------------
builder.Services.AddScoped<
    IUnitOfWork,
    UnitOfWork>();

// --------------------------------------------------
// SERVICES
// --------------------------------------------------
builder.Services.AddScoped<
    ICartService,
    CartService>();

builder.Services.AddScoped<
    IOrderService,
    OrderService>();

// --------------------------------------------------
// PRODUCT SERVICE HTTP CLIENT
//
// OrderService must NOT access ProductDb directly.
// It communicates with ProductService through HTTP.
// --------------------------------------------------
var productServiceBaseUrl =
    builder.Configuration[
        "ProductService:BaseUrl"];

if (string.IsNullOrWhiteSpace(
        productServiceBaseUrl))
{
    throw new InvalidOperationException(
        "ProductService BaseUrl is not configured.");
}

builder.Services
    .AddHttpClient<
        IProductServiceClient,
        ProductServiceClient>(
            client =>
            {
                client.BaseAddress =
                    new Uri(
                        productServiceBaseUrl);
            })
    .AddHttpMessageHandler<AuthHeaderHandler>();
        

// --------------------------------------------------
// JWT CONFIGURATION
// --------------------------------------------------
var jwtKey =
    builder.Configuration[
        "Jwt:Key"];

var jwtIssuer =
    builder.Configuration[
        "Jwt:Issuer"];

var jwtAudience =
    builder.Configuration[
        "Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is not configured.");
}

builder.Services
    .AddAuthentication(
        JwtBearerDefaults
            .AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey =
                        true,

                    ValidIssuer =
                        jwtIssuer,

                    ValidAudience =
                        jwtAudience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8
                                .GetBytes(
                                    jwtKey))
                };
        });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<AuthHeaderHandler>();
// --------------------------------------------------
// BUILD APPLICATION
// --------------------------------------------------
var app = builder.Build();

// --------------------------------------------------
// SWAGGER
// --------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --------------------------------------------------
// MIDDLEWARE
// --------------------------------------------------
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();