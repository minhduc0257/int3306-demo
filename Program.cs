using System.Net;
using System.Reflection;
using int3306;
using Microsoft.EntityFrameworkCore;

const string baseApiPath = "api";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (Environment.GetEnvironmentVariable("PORT") != null)
{
    builder.WebHost.ConfigureKestrel(k =>
    {
        k.Listen(
            IPAddress.Parse("0.0.0.0"), 
            int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port) ? port : 0
        );
    });
}

builder.Services.AddCors(cors =>
{
    cors.AddDefaultPolicy(
        policyBuilder => policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
    );
});
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContextPool<DataDbContext>(opt =>
{
    opt.UseSqlite("Data Source=./int3306.sqlite");
});
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    options.DocumentFilter<SwaggerPrefixDocumentFilter>(baseApiPath);
}).AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    o.RoutePrefix = string.Empty;
});

app.Map($"/{baseApiPath}", appBuilder =>
{
    appBuilder.UseRouting();
    appBuilder.UseAuthorization();
    appBuilder.UseCors();
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",    
            pattern: "{controller}/{action=Index}/{id?}");
    });
});

app.Run();
