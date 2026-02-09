using Microsoft.EntityFrameworkCore;
using TestPDFGenerator.Api.Data;
using TestPDFGenerator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add DbContext with In-Memory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("PdfTemplateSystemDb"));

// Add custom services
builder.Services.AddScoped<SchemaDiscoveryService>();
builder.Services.AddSingleton<PdfGenerationService>();
builder.Services.AddScoped<ICustomJoinValidator, CustomJoinValidator>();
builder.Services.AddScoped<IHybridContextDataFetcher, HybridContextDataFetcher>();

// Configure TemplateEngineService with HybridContextDataFetcher
builder.Services.AddScoped<TemplateEngineService>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    var cache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
    var logger = sp.GetRequiredService<ILogger<TemplateEngineService>>();
    var templateEngine = new TemplateEngineService(context, cache, logger);
    var dataFetcher = sp.GetRequiredService<IHybridContextDataFetcher>();
    templateEngine.SetDataFetcher(dataFetcher);
    return templateEngine;
});

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DataSeeder.SeedDataAsync(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Cleanup on shutdown
await PdfGenerationService.DisposeBrowserAsync();
