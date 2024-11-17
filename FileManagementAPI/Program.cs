//using Microsoft.OpenApi.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//// Configure Swagger/OpenAPI with file upload support
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "File Management API",
//        Version = "v1"
//    });

//    // Add support for file uploads in Swagger
//    c.OperationFilter<FileUploadOperation>();
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Management API V1");
//    });
//}

//app.UseStaticFiles(); // Important for serving uploaded files
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();
using FileManagementAPI.DataAccess;
using FileManagementAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Register DapperContext
builder.Services.AddSingleton<DapperContext>();

// Register FileService
builder.Services.AddScoped<FileService>();

builder.Services.AddControllers();

// Configure CORS if needed
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

// Configure Swagger/OpenAPI with file upload support
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

app.UseStaticFiles(); // Important for serving uploaded files
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();