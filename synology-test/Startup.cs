using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Synology;
using Synology.Api.Auth.Parameters;
using Synology.Interfaces;
using Synology.Settings;
using synology_test.Models;

namespace synology_test
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
            services.AddMvc();

            services.Configure<SynologyConfig>(Configuration.GetSection("Synology"));

            services.AddSynology(s =>
            {
                s.AddApi();
                s.AddFileStation();
            });

            services.AddScoped<ISynologyConnectionSettings, SynologyConnectionSettings>((ctx) =>
            {
                var config = ctx.GetService<IOptions<SynologyConfig>>();
                return ConfigureSynologySettings(config);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ISynologyConnection connection,
            ISynologyConnectionSettings synologyConnectionSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            connection.Api().Auth().Login(new LoginParameters
            {
                Username = synologyConnectionSettings.Username,
                Password = synologyConnectionSettings.Password
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private SynologyConnectionSettings ConfigureSynologySettings(IOptions<SynologyConfig> synologySettings)
        {
            var settings = new SynologyConnectionSettings();
            settings.BaseHost = synologySettings.Value.BaseHost;
            settings.Username = synologySettings.Value.Username;
            settings.Password = synologySettings.Value.Password;
            settings.Port = synologySettings.Value.Port;
            settings.Ssl = synologySettings.Value.Ssl;
            settings.SslPort = synologySettings.Value.SslPort;
            return settings;
        }
    }
}
