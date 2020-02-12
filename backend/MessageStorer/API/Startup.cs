using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Persistance;
using API.Persistance.Repository;
using API.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                options.UseNpgsql(Configuration["ConnectionStrings:MessagesStore"]);
            }, ServiceLifetime.Singleton);

            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IHttpMetadataService, HttpMetadataService>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IWriterTypeRepository, WriterTypeRepository>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ILastSyncService, LastSyncService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IAliasRepository, AliasRepository>();
            services.AddScoped<IAliasService, AliasService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
