using API.Config;
using Common.Exceptions;
using API.Persistance;
using API.Persistance.Repository;
using API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using Common.Service;
using API.Middleware;
using System;
using API.Infrastructure;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<MessagesStoreContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("MessagesStore"));
            });

            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message Storer API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Authorization token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Security:Key"]))
                    };
                });

            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IHttpMetadataService, HttpMetadataService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IWriterTypeRepository, WriterTypeRepository>();
            services.AddScoped<IImportRepository, ImportRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ISyncDateTimeService, SyncDateTimeService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IAliasRepository, AliasRepository>();
            services.AddScoped<IMessengerIntegrationClient, MessengerIntegrationClient>();
            services.AddScoped<IAliasService, AliasService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IImportService, ImportService>();

            services.AddSingleton<ISecurityConfig, SecurityConfig>();
            services.AddSingleton<IAttachmentConfig, AttachmentConfig>();
            services.AddSingleton<IApkConfig, ApkConfig>();

            services.AddControllers(options =>
                options.Filters.Add(new ExceptionHandler()));

            services.AddHttpClient("messengerIntegrationClient", configure =>
            {
                configure.BaseAddress = new Uri(Configuration["MessengerIntegration:Url"]);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<AddTokenFromQueryMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<RefreshTokenMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message Storer API v1");
            });
        }
    }
}
