using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using PxGraf.Caching;
using PxGraf.PxWebInterface;
using PxGraf.PxWebInterface.Caching;
using PxGraf.Settings;
using PxGraf.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

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
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.AddResponseCaching();
            services.AddCors(options =>
            {
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
            });
            services.AddMemoryCache();

            services.AddControllers()
                .AddNewtonsoftJson(mvcNewtonsoftJsonOptions =>
                {
                    mvcNewtonsoftJsonOptions.AllowInputFormatterExceptionMessages = HostEnvironment.IsDevelopment();
                    mvcNewtonsoftJsonOptions.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                    //We have global error handler but for some reason serialization errors don't end up there
                    //...until we throw in error handler...
                    mvcNewtonsoftJsonOptions.SerializerSettings.Error += (sender, args) =>
                    {
                        throw new SerializationException("Serialization failed", args.ErrorContext.Error);
                    };
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
            services.AddSingleton<PxWebInterface.IPxWebApiInterface, PxWebInterface.PxWebV1ApiInterface>();
            services.AddSingleton<ISqFileInterface, SqFileInterface>();
            services.AddSingleton<ICachedPxWebConnection, CachedPxWebConnection>();
            services.AddSingleton<IVisualizationResponseCache, VisualizationResponseCache>();
            services.AddSingleton<IPxWebApiResponseCache, PxWebApiResponseCache>();
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
                c.SwaggerEndpoint($"../{swaggerDocName}/swagger/swagger.json", "PxGraf");
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