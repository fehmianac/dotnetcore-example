using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreBase.API.Behaviours;
using DotnetCore.Api.Behaviours;
using DotnetCore.Api.Config;
using DotnetCore.Api.HealthCheck;
using DotnetCore.Api.Middleware;
using DotnetCore.Api.Swagger;
using DotnetCore.Data;
using DotnetCore.Data.Example;
using DotnetCore.Service.Example;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DotnetCore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private AppConfig _appSettings;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddLocalization();
            services.AddHealthChecks();
            services.AddHealthChecks()
                .AddCheck<DbHealthCheck>("DbHealthCheck");
            var section = Configuration.GetSection("AppSettings");
            services.Configure<AppConfig>(section);
            _appSettings = section.Get<AppConfig>();
            services.AddSingleton<AppConfig>(_appSettings);
            
            if (!string.IsNullOrEmpty(_appSettings.CacheSettings.RedisConfiguration))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = _appSettings.CacheSettings.RedisConfiguration;
                    options.InstanceName = _appSettings.CacheSettings.RedisInstanceName;
                });    
            }

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
              services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.AuthorizationSettings.JwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AuthorizePolicy", policy => policy.RequireClaim(ClaimTypes.Authentication));
                options.AddPolicy("LoginPolicy", policy => policy.RequireClaim(ClaimTypes.Authentication, "Login"));
            });

            services.AddSwaggerGen(o =>
            {
                o.OperationFilter<RequiredHeaderParameterOperationFilter>();
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = _appSettings.SwaggerSettings.DocInfoTitle+"("+_appSettings.Environment+")",
                    Version = _appSettings.SwaggerSettings.DocInfoVersion
                });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            Scheme = SecuritySchemeType.ApiKey.ToString(),
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services.AddSingleton<ConnectionHelper>(new ConnectionHelper(_appSettings.ConnectionStrings.UserDB));
            services.AddMediatR(typeof(GetUserListRequest).Assembly);
            services.AddMediatR(typeof(GetUserIdDataRequest).Assembly);
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheBehaviour<,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHealthChecks("/health");

            app.UseRouting();
            app.UseRouting();
            app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", _appSettings.SwaggerSettings.DocName); });
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}