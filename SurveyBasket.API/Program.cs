var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependenciesServices();

var app = builder.Build();



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
