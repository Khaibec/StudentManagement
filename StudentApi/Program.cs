using Microsoft.EntityFrameworkCore;
using StudentApi.Models;
using StudentApi.Data;
using StudentApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<EnrollmentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
var app = builder.Build();

app.UseCors("AngularClient");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDBContext>();

    try
    {
        Console.WriteLine($"Database: {db.Database.GetDbConnection().Database}");
        Console.WriteLine($"Server: {db.Database.GetDbConnection().DataSource}");

        Console.WriteLine(
            db.Database.CanConnect()
                ? "DATABASE CONNECTED"
                : "DATABASE CONNECTION FAILED"
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "StudentApi dang chay");

app.MapControllers();

app.Run();