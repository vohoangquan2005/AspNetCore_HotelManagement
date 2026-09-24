using HotelManagement.Data;
using HotelManagement.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnectionString")
    )
);

// Controllers
builder.Services.AddControllers();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Services
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<RoomTypeService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<RoomImageService>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<ServicePackageService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<ApplyPromotionService>();
builder.Services.AddScoped<CancellationPolicyService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<BookingDetailService>();
builder.Services.AddScoped<GuestService>();
builder.Services.AddScoped<BookingServiceService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<CancellationService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<FavoriteRoomService>();
builder.Services.AddScoped<AuditLogService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
// Middleware

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Hotel Management API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seeder
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

app.Run();
