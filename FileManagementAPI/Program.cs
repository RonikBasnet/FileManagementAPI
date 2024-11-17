using FileManagementAPI.DataAccess;
using FileManagementAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register DapperContext
builder.Services.AddSingleton<DapperContext>();

// Register FileService
builder.Services.AddScoped<FileService>();

builder.Services.AddControllers();

//cors ko configratiion
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

//file upload support ko lagi swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "File Management API",
        Version = "v1"
    });

    c.OperationFilter<FileUploadOperation>();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Management API V1");
    });
}

// Use CORS
app.UseCors("AllowAll");

app.UseStaticFiles(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();