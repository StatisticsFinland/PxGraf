using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System;
using PxGraf.Datasource;
using PxGraf.Datasource.Cache;
using PxGraf.Models.Queries;
using PxGraf.Datasource.FileDatasource;
using PxGraf.Datasource.ApiDatasource;
using System.Text;

namespace PxGraf
{
    [ExcludeFromCodeCoverage] // Testing of these methods should be done with integration tests
    public class Startup
    {
        public const string PXWEBCLIENTNAME = "PxWebClient";

        readonly string PxGrafAllowSpecificOrigins = "_pxGrafAllowSpecificOrigins";
        private readonly ILogger logger;
        private const string swaggerDocName = "api";

        private IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            logger = new NLogLoggerProvider().CreateLogger(typeof(Startup).FullName);
            HostEnvironment = environment;

            Configuration.Load(configuration);
            try
            {
                Language.Localization.Load(Path.Combine(AppContext.BaseDirectory, "Pars\\translations.json"));
            }
            catch(IOException ex)
            {
                logger.LogCritical(ex, "A file system error occurred during startup");
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("An error occurred while parsing the translation file", ex);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "After evaluating the function and CORS permissions, we have deemed the warning to be unnecessary.")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.AddResponseCaching();
            services.AddCors(options =>
            {
#pragma warning disable S5122
                if (Configuration.Current.CorsOptions.AllowAnyOrigin)
                {
                    // It is not recommended that any origin is allowed but this is sometimes nessessary for debugging etc.
                    // Warning is disabled, because this is an optional block.
                    options.AddPolicy(name: PxGrafAllowSpecificOrigins,
                        builder =>
                        {
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                }
                else
                {
                    options.AddPolicy(name: PxGrafAllowSpecificOrigins,
                        builder =>
                        {
                            builder
                                .WithOrigins(Configuration.Current.CorsOptions.AllowedOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                }
#pragma warning restore S5122
            });
            services.AddMemoryCache();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.AllowInputFormatterExceptionMessages = HostEnvironment.IsDevelopment();
                options.JsonSerializerOptions.PropertyNamingPolicy = GlobalJsonConverterOptions.Default.PropertyNamingPolicy;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = GlobalJsonConverterOptions.Default.PropertyNameCaseInsensitive;
                options.JsonSerializerOptions.AllowTrailingCommas = GlobalJsonConverterOptions.Default.AllowTrailingCommas;
            });

            services.AddMvc(c =>
                c.Conventions.Add(new ApiExplorerConventions())
            );

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.ToString());

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.SwaggerDoc(swaggerDocName, new OpenApiInfo { Title = "PxGraf", Version = "v1" });
                try
                {
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PxGraf.xml"));
                }
                catch(IOException ex)
                {
                    logger.LogCritical(ex, "A file system error was encountered");
                }
            });

            services.AddHttpClient(PXWEBCLIENTNAME).ConfigurePrimaryHttpMessageHandler(() =>
                new HttpClientHandler
                {
                    UseDefaultCredentials = true
                });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ISqFileInterface, SqFileInterface>();
            services.AddSingleton<IMultiStateMemoryTaskCache>(provider => new MultiStateMemoryTaskCache(
                Configuration.Current.CacheOptions.Database.ItemAmountLimit,
                TimeSpan.FromSeconds(Configuration.Current.CacheOptions.CacheFreshnessCheckIntervalSeconds)));
            if (Configuration.Current.LocalFilesystemDatabaseConfig.Enabled)
            {
                services.AddSingleton<IFileDatasource>(provider => new LocalFilesystemDatabase(
                    Configuration.Current.LocalFilesystemDatabaseConfig ));
                services.AddSingleton<ICachedDatasource, CachedFileDatasource>();
            }
            else
            {
                services.AddSingleton<IPxWebConnection, PxWebConnection>();
                services.AddSingleton<IApiDatasource, PxWebV1ApiInterface>();
                services.AddSingleton<ICachedDatasource, CachedApiDatasource>();
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            /*
             Emulates old behavior with priority:
                1. /api/error
                2. developerExceptionPage
               (3. default exception handler)
             */
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Swagger requires this seemingly redundant setup to work properly with a custom api route prefix.
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "{documentName}/swagger/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger.json", "PxGraf");
                c.RoutePrefix = $"{swaggerDocName}/swagger";
            });

            app.UseExceptionHandler("/api/error");

            if (Configuration.Current.CreationAPI)
            {
                app.UseDefaultFiles().UseStaticFiles();
            }

            app.UseRouting();
            app.UseCors(PxGrafAllowSpecificOrigins);
            app.UseResponseCaching();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(PxGrafAllowSpecificOrigins);
                if (Configuration.Current.CreationAPI)
                {
                    endpoints.MapFallbackToFile("index.html");
                }
            });
        }
    }
}