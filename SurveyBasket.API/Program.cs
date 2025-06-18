var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependenciesServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
