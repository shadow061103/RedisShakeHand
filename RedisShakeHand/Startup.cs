using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RedisShakeHand.Extensions;
using RedisShakeHand.Services;
using RedisShakeHand.Services.Interfaces;

namespace RedisShakeHand
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Redis Shake Hand",
                    Description = "Redis���ի��O"
                });

                var basePath = AppContext.BaseDirectory;
                var xmlFiles = Directory.EnumerateFiles(basePath, $"*.xml", SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in xmlFiles)
                {
                    c.IncludeXmlComments(xmlFile, true);
                }
            });

            //redis
            services.AddRedisCache();

            services.AddScoped<IWeatherForecastService, WeatherForecastService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IRedisHashService, RedisHashService>();
            services.AddScoped<ISetService, SetService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: �ݰt�X SwaggerDoc �� name�C "/swagger/{SwaggerDoc name}/swagger.json"
                    url: "./v1/swagger.json",
                    // description: �Ω� Swagger UI �k�W����ܤ��P������ SwaggerDocument ��ܦW�٨ϥΡC
                    name: ""
                );
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}