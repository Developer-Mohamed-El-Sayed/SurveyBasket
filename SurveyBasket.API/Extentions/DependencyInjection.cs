namespace SurveyBasket.API.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependenciesServices(this IServiceCollection services)
    {
        services.AddControllerConfig()
            .AddMapsterConfig();
        return services;
    }
    private static IServiceCollection AddControllerConfig(this IServiceCollection services)
    {
         services.AddControllers();
        return services;
    }
    private static IServiceCollection  AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
        mappingConfiguration.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));
        return services;
    }
}
