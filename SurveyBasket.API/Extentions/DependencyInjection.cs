namespace SurveyBasket.API.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependenciesServices(this IServiceCollection services)
    {
        services.AddControllerConfig()
            .AddMapsterConfig()
            .AddValidationConfig();
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
    private static IServiceCollection AddValidationConfig(this IServiceCollection services) =>
        services.AddFluentValidationAutoValidation()
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        
}
