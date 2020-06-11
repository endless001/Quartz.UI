using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Api.Data;
using Quartz.Api.Infrastructure;
using Quartz.Api.Job;
using Quartz.Api.Services;
using Quartz.Impl;

namespace Quartz.Api
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
            services.AddDbContext<QuartzDbContext>(options =>
            {
                options.UseMySQL(Configuration["MysqlConnection"]);
            });




            services.AddSingleton<StandardHttpClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<StandardHttpClient>>();

                return new StandardHttpClient(logger);
            });
            services.AddScoped<IScheduleService, ScheduleService>();
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            scheduler.JobFactory = new JobFactory(services.BuildServiceProvider());
            services.AddSingleton(scheduler);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseRouting();

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
