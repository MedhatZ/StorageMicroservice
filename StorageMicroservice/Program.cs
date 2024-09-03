// Program.cs (for .NET 6 and later)
using Microsoft.EntityFrameworkCore;
using StorageMicroservice.Data;
using StorageMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


