using APISecurityToken.Interfaces;
using APISecurityToken.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PINDomain.Interfaces;
using PINDomain.Security;
using PINDomain.Shared;
using System;

namespace APISecurityToken
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = Configuration.GetSection("Configuration:Title").Value,
                    Version = Configuration.GetSection("Configuration:Version").Value,
                    Description = Configuration.GetSection("Configuration:Description").Value,
                    TermsOfService = new Uri(Configuration.GetSection("Configuration:TermsOfService").Value),
                    Contact = new OpenApiContact
                    {
                        Name = Configuration.GetSection("Configuration:ContactName").Value,
                        Email = Configuration.GetSection("Configuration:ContactEMail").Value,
                        Url = new Uri(Configuration.GetSection("Configuration:Url").Value),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri(Configuration.GetSection("Configuration:UrlLicence").Value),
                    }
                });
            });
            // Dependency Injection
            services.AddScoped<ISpreadSheet, SpreadSheet>();
            services.AddScoped<IPINGenerator, PINGenerator>();
            services.AddScoped<ITokenService, TokenService>();
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
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            var swaggerEndPoint = string.Format(Configuration.GetSection("Configuration:SwaggerEndpoint").Value, Configuration.GetSection("Configuration:Version").Value);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(swaggerEndPoint, Configuration.GetSection("Configuration:Title").Value);
            });
        }
    }
}
