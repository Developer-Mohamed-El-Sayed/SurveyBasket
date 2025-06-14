namespace SurveyBasket.API.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependenciesServices(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddControllerConfig()
            .AddMapsterConfig()
            .AddValidationConfig()
            .AddRegistrationConfig()
            .AddConnectionConfig(configuration);
        return services;
    }
    private static IServiceCollection AddControllerConfig(this IServiceCollection services) 
    {
         services.AddControllers();
        return services;
    }
    private static IServiceCollection  AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));
        return services;
    }
    private static IServiceCollection AddValidationConfig(this IServiceCollection services) =>
        services.AddFluentValidationAutoValidation()
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    private static IServiceCollection AddConnectionConfig(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Please check your appsettings.json.");
        services.AddDbContext<SurveyBasketDbContext>(option =>
            option.UseSqlServer(connectionString));
        return services;
    }
    private static IServiceCollection AddRegistrationConfig(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        return services;
    }

}
