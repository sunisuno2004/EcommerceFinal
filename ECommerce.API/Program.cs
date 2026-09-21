using ECommerce.API.Exceptions;
using ECommerce.API.Security;
using ECommerce.Business.Repositories;
using ECommerce.Business.Security;
using ECommerce.Business.Services;
using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Controllers
// --------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

// --------------------------------------------------
// Database
// --------------------------------------------------
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// --------------------------------------------------
// JWT Authentication
// --------------------------------------------------
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];
        var jwtAudience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey!))
            };
    });

// --------------------------------------------------
// Product
// --------------------------------------------------
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// --------------------------------------------------
// Inventory
// --------------------------------------------------
builder.Services.AddScoped<IInventoryService, InventoryService>();

// --------------------------------------------------
// Cart
// --------------------------------------------------
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

// --------------------------------------------------
// Order
// --------------------------------------------------
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// --------------------------------------------------
// Authentication / User
// --------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<JwtTokenService>();

// --------------------------------------------------
// Payment
// --------------------------------------------------
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductBatchRepository, ProductBatchRepository>();
builder.Services.AddScoped<IProductBatchService, ProductBatchService>();
// --------------------------------------------------
// Global Exception Handling
// --------------------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --------------------------------------------------
// Swagger / OpenAPI
// --------------------------------------------------
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// ==================================================
// BUILD APPLICATION
// ==================================================

var app = builder.Build();

// --------------------------------------------------
// Global Exception Handler
// --------------------------------------------------
app.UseExceptionHandler();

// --------------------------------------------------
// Swagger
// --------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("FrontendPolicy");
}

// --------------------------------------------------
// HTTPS
// --------------------------------------------------
app.UseHttpsRedirection();


// --------------------------------------------------
// Authentication
// IMPORTANT: Authentication must come before
// Authorization.
// --------------------------------------------------
app.UseAuthentication();

// --------------------------------------------------
// Authorization
// --------------------------------------------------
app.UseAuthorization();

// --------------------------------------------------
// Controllers
// --------------------------------------------------
app.MapControllers();

// --------------------------------------------------
// Run Application
// --------------------------------------------------
app.Run();