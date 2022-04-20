using System;
using ContosoUniversity.Data;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackifyExample4.Data;
using StackifyLib;
using StackifyMiddleware;

namespace StackifyExample4
{
   public class Startup
   {
      private LoggerFactory loggerFactory;

      public Startup(IHostingEnvironment env)
      {
         loggerFactory = new LoggerFactory();
         var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)

                .AddJsonFile("Stackify.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
         Configuration = builder.Build();
      }

      public IConfigurationRoot Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddDbContext<SchoolContext>(options =>
         {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure());
         });
         services.AddTransient<StudentRecruiter>();
         services.AddSingleton<DapperContext>();
         services.AddScoped<DepartmentRepository>();
         services.AddScoped<StudentRepository>();

         // Add Hangfire services.
         services.AddHangfire(configuration => configuration
             .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
             {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
             }));

         // Add the processing server as IHostedService
         services.AddHangfireServer();

         services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

         services.AddSingleton(sp =>
         {
            return loggerFactory.Create();
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseHsts();
         }

         app.UseHttpsRedirection();
         app.UseMiddleware<RequestTracerMiddleware>();
         app.ConfigureStackifyLogging(this.Configuration);
         loggerFactory.Init();
         loggerFactory.Create().ForContext<Startup>().Information("Configuration: {@Configuration}", Configuration);
         app.UseHangfireDashboard(options: new DashboardOptions { IsReadOnlyFunc = (context) => true });
         app.UseMvc();
      }
   }
}
