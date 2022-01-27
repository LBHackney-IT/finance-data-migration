using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FinanceDataMigrationApi.V1.Gateways;
using FinanceDataMigrationApi.V1.Infrastructure;
using FinanceDataMigrationApi.V1.UseCase;
using FinanceDataMigrationApi.V1.UseCase.Interfaces;
using FinanceDataMigrationApi.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using FinanceDataMigrationApi.V1.Common;
using Microsoft.Extensions.Options;
using FinanceDataMigrationApi.V1.Gateways.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Interfaces;
using FinanceDataMigrationApi.V1.Infrastructure.Accounts;
using Hackney.Core.DynamoDb;
using FinanceDataMigrationApi.V1.UseCase.Accounts;
using FinanceDataMigrationApi.V1.UseCase.Interfaces.Accounts;
using Hackney.Core.ElasticSearch.Interfaces;
using Hackney.Core.ElasticSearch;

namespace FinanceDataMigrationApi
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static List<ApiVersionDescription> _apiVersions { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup configuration
            IConfigurationSection settingsSection = Configuration.GetSection("Settings");
            IConfigurationSection apiOptionsConfigSection = settingsSection.GetSection(nameof(ApiOptions));
            services.Configure<ApiOptions>(apiOptionsConfigSection);
            ApiOptions apiOptions = apiOptionsConfigSection.Get<ApiOptions>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            
            services.AddApiVersioning(o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.AssumeDefaultVersionWhenUnspecified = true; // assume that the caller wants the default version if they don't specify
                o.ApiVersionReader = new UrlSegmentApiVersionReader(); // read the version number from the url segment header)
            });

            services.AddSingleton<IApiVersionDescriptionProvider, DefaultApiVersionDescriptionProvider>();

            services.AddSwaggerGen(swaggerSetup =>
            {
                swaggerSetup.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Your Hackney Token. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                swaggerSetup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });

                //Looks at the APIVersionAttribute [ApiVersion("x")] on controllers and decides whether or not
                //to include it in that version of the swagger document
                //Controllers must have this [ApiVersion("x")] to be included in swagger documentation!!
                swaggerSetup.DocInclusionPredicate((docName, apiDesc) =>
                {
                    apiDesc.TryGetMethodInfo(out var methodInfo);

                    var versions = methodInfo?
                        .DeclaringType?.GetCustomAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions).ToList();

                    return versions?.Any(v => $"{v.GetFormattedApiVersion()}" == docName) ?? false;
                });

                //Get every ApiVersion attribute specified and create swagger docs for them
                foreach (var version in _apiVersions.Select(apiVersion => $"v{apiVersion.ApiVersion}"))
                {
                    swaggerSetup.SwaggerDoc(version, new OpenApiInfo
                    {
                        Title = $"{apiOptions.ApiName}-api {version}",
                        Version = version,
                        Description = $"{apiOptions.ApiName} version {version}. Please check older versions for depreciated endpoints."
                    });
                }

                swaggerSetup.CustomSchemaIds(x => x.FullName);
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                    swaggerSetup.IncludeXmlComments(xmlPath);
            });

            ConfigureDbContext(services);
            services.ConfigureDynamoDB();
            services.ConfigureElasticSearch(Configuration);

            RegisterGateways(services);
            RegisterUseCases(services);
            #region ExtraSerives

            services.AddScoped<ICustomeHttpClient, CustomeHttpClient>();
            services.AddScoped<IGetEnvironmentVariables, GetEnvironmentVariables>();
            services.AddScoped(typeof(IQueryBuilder<>), typeof(QueryBuilder<>));
            services.AddScoped<IWildCardAppenderAndPrepender, WildCardAppenderAndPrepender>();

            #endregion
        }

        private static void ConfigureDbContext(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<DbTransactionsContext>(opt => opt.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(360);
            }));

            services.AddDbContext<DbAccountsContext>(opt => opt.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(360);
            }));
        }

        private static void RegisterGateways(IServiceCollection services)
        {
            services.AddTransient<LoggingDelegatingHandler>();

            services.AddScoped<IDMTransactionEntityGateway, DMTransactionEntityGateway>();
            services.AddScoped<IDMAccountEntityGateway, DMAccountEntityGateway>();
            services.AddScoped<ITransactionAPIGateway, TransactionAPIGateway>();
            services.AddScoped<IDMRunLogGateway, DMRunLogGateway>();
            services.AddScoped<ITenureAPIGateway, TenureAPIGateway>();
            services.AddScoped<IPersonGateway, PersonGateway>();
            services.AddScoped<IEsGateway, EsGateway>();
            services.AddScoped<ITenureDynamoDbGateway, TenureDynamoDbGateway>();
            services.AddScoped<ITransactionsDynamoDbGateway, TransactionsDynamoDbGateway>();
            services.AddScoped<IAccountsDynamoDbGateway, AccountsDynamoDbGateway>();

            var transactionApiUrl = Environment.GetEnvironmentVariable("FINANCIAL_TRANSACTION_API_URL");
            var transactionApiToken = Environment.GetEnvironmentVariable("FINANCIAL_TRANSACTION_API_TOKEN");

            services.AddHttpClient<ITransactionAPIGateway, TransactionAPIGateway>(c =>
                {
                    c.BaseAddress = new Uri(transactionApiUrl);
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(transactionApiToken);
                })
                .AddHttpMessageHandler<LoggingDelegatingHandler>();

            var searchApiUrl = Environment.GetEnvironmentVariable("SEARCH_API_URL");
            var searchApiToken = Environment.GetEnvironmentVariable("SEARCH_API_TOKEN");

            services.AddHttpClient<ITenureAPIGateway, TenureAPIGateway>(c =>
                {
                    c.BaseAddress = new Uri(searchApiUrl);
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(searchApiToken);
                })
                .AddHttpMessageHandler<LoggingDelegatingHandler>();

            var personApiUrl = Environment.GetEnvironmentVariable("PERSON_API_URL");
            var personApiToken = Environment.GetEnvironmentVariable("PERSON_API_TOKEN");

            services.AddHttpClient<IPersonGateway, PersonGateway>(c =>
                {
                    c.BaseAddress = new Uri(personApiUrl);
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personApiToken);
                })
                .AddHttpMessageHandler<LoggingDelegatingHandler>();
        }

        private static void RegisterUseCases(IServiceCollection services)
        {
            services.AddScoped<IExtractTransactionEntityUseCase, ExtractTransactionEntityUseCase>();
            services.AddScoped<ITransformTransactionEntityUseCase, TransformTransactionEntityUseCase>();
            services.AddScoped<IIndexTransactionEntityUseCase, IndexTransactionEntityUseCase>();
            services.AddScoped<ILoadTransactionEntityUseCase, LoadTransactionEntityUseCase>();

            services.AddScoped<IGetTenureByPrnUseCase, GetTenureByPrnUseCase>();
            services.AddScoped<IGetPersonByIdUseCase, GetPersonByIdUseCase>();

            services.AddScoped<IExtractAccountEntityUseCase, ExtractAccountEntityUseCase>();
            services.AddScoped<ITransformAccountsUseCase, TransformAccountsUseCase>();
            services.AddScoped<ILoadAccountsUseCase, LoadAccountsUseCase>();
            services.AddScoped<IIndexAccountEntityUseCase, IndexAccountEntityUseCase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ApiOptions> apiOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var origins = Environment.GetEnvironmentVariable("ACCEPTED_ORIGINS").Split(",");
            app.UseCors(options => options.WithOrigins(origins)
                .AllowAnyMethod()
                .AllowAnyHeader());

            //app.UseCors(builder => builder
            //      .AllowAnyOrigin()
            //      .AllowAnyHeader()
            //      .AllowAnyMethod()
            //      .WithExposedHeaders("x-correlation-id"));

            //app.UseCorrelationId();
            //app.UseLoggingScope();
            //app.UseCustomExceptionHandler(logger);


            //app.UseXRay("finance-data-migration-api");


            //Get All ApiVersions,
            var api = app.ApplicationServices.GetService<IApiVersionDescriptionProvider>();
            _apiVersions = api.ApiVersionDescriptions.ToList();

            //Swagger ui to view the swagger.json file
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionDescription in _apiVersions)
                {
                    //Create a swagger endpoint for each swagger version
                    c.SwaggerEndpoint($"{apiVersionDescription.GetFormattedApiVersion()}/swagger.json",
                        $"{apiOptions.Value.ApiName}-api {apiVersionDescription.GetFormattedApiVersion()}");
                }
            });
            app.UseSwagger();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // SwaggerGen won't find controllers that are routed via this technique.
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapHealthChecks("/api/v1/healthcheck/ping", new HealthCheckOptions()
                //{
                //    ResponseWriter = HealthCheckResponseWriter.WriteResponse
                //});
            });

            // app.UseLogCall();
        }
    }
}
