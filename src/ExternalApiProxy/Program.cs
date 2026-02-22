using ExternalApiProxy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<IJsonPlaceholderService, JsonPlaceholderService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApis:JsonPlaceholder:BaseUrl"]!);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
