namespace SurveyBasket.API.Extentions;

public static class DependencyInjection
{
    public static IServiceCollection AddDependenciesServices(this IServiceCollection services,IConfiguration configuration)
    {
        services
            .AddControllerConfig()
            .AddMapsterConfig()
            .AddHttpContextAccessor()
            .AddMailSettingConfig(configuration)
            .AddCORSConfig(configuration)
            .AddIdentityConfig()
            .AddValidationConfig()
            .AddHybridCacheConfig()
            .AddRegistrationConfig()
            .AddAuthenticationConfig(configuration)
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
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddTransient<IAuthorizationHandler,PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider,PermissionAuthorizationPolicyProvider>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
    private static IServiceCollection AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<SurveyBasketDbContext>();
        services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
        });
        return services;
    }
    private static IServiceCollection AddAuthenticationConfig(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<GoogleSettings>(configuration.GetSection(nameof(GoogleSettings))); // add options 
        var googleSettings = configuration.GetSection(nameof(GoogleSettings)).Get<GoogleSettings>();  // bind data 
        services.AddOptions<JwtOptions>()
            .BindConfiguration(((JwtOptions.SectionName)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtOptions!.Issuer,
                ValidAudience = jwtOptions!.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
            };
        })
        .AddGoogle(Options =>
        {
            Options.ClientId = googleSettings!.ClientId;
            Options.ClientSecret = googleSettings!.ClientSecret;
            Options.CallbackPath = "/signin-google";
        });
        return services;
    }
    private static IServiceCollection AddCORSConfig(this IServiceCollection services,IConfiguration configuration) =>
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(option => 
                option
                .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
        });
    private static IServiceCollection AddHybridCacheConfig(this IServiceCollection services)
    {
        services.AddHybridCache(); 
        return services;
    }
    private static IServiceCollection AddMailSettingConfig(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        return services;
    } 
}
