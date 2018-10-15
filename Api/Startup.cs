using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Auctus.Util;
using Microsoft.Extensions.PlatformAbstractions;
using Auctus.Business;
using Swashbuckle.AspNetCore.Swagger;
using Api.Hubs;

namespace Api
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static IHostingEnvironment Enviroment { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Enviroment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            var urlConfiguration = Configuration.GetSection("Url");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidAudience = urlConfiguration.GetSection("Web").Get<List<string>>().First(),
                    ValidateIssuer = true,
                    ValidIssuer = urlConfiguration.GetValue<string>("Api"),
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new X509SecurityKey(new X509Certificate2(Path.Combine(env.ContentRootPath, "auctus.pfx"), "")),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Auth").GetValue<string>("Secret"))),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(3),
                    RequireExpirationTime = true
                };
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
            });

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Default", builder =>
                    builder.WithOrigins(urlConfiguration.GetSection("Web").Get<List<string>>().ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
                options.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFF'Z'";
            });
            services.AddSingleton<Cache>();

            DataAccessDependencyResolver.RegisterDataAccess(services, Configuration, Enviroment.IsDevelopment());

            if (Enviroment.IsDevelopment())
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "Auctus Platform API",
                        Description = "Auctus Platform Web API"
                    });
                });
            }

            services.AddApplicationInsightsTelemetry(Configuration.GetSection("ApplicationInsights:InstrumentationKey").Get<string>());

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors("Default");
            app.UseMvcWithDefaultRoute();
            
            app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<AuctusHub>("/api/auctusHub");
            });

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
        }
    }
}
