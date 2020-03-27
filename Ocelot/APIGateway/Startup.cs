using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIGateway.Data;
using APIGateway.Middleware;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.RateLimit;
using APIGateway.Extensions;
//using Ocelot.Middleware.Pipeline;

using Ocelot;

namespace APIGateway
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
            services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddTransient<FactoryActivatedMiddleware>();
            services.AddControllers();
            services.AddOcelot();

            //=============start ipRatelimit============
            // 將速限計數器資料儲存在 Memory 中
            services.AddMemoryCache();

            // 從 appsettings.json 讀取 IpRateLimiting 設定 
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            // 從 appsettings.json 讀取 Ip Rule 設定
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // 注入 counter and IP Rules 
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Rate Limit configuration 設定
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddSingleton<IRateLimitCounterHandler, MemoryCacheRateLimitCounterHandler>();
            //=============end ipRatelimit============
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIpRateLimiting();

            app.UseCheckTokenMiddleware();

            //app.Use(async (context, next) =>
            //{

            //    string newPath = null;
            //    var url = context.Request.Path;
            //    if (url == "/dan")
            //    {
            //        newPath = "/customer";
            //        context.Request.Path = newPath;
            //        context.Request.Method = "GET";
            //        context.Request.ContentType = "application/json";

            //    }

            //    await next.Invoke();

            //});
            app.UseOcelot().Wait();

            //var configuration = new OcelotPipelineConfiguration
            //{
            //    PreAuthenticationMiddleware = async (ctx, next) =>
            //    {
            //        if (xxxxxx)
            //        {
            //            ctx.Errors.Add(new UnauthenticatedError("Your message"));

            //            return;
            //        }

            //        await next.Invoke();
            //    }
            //};
            //app.UseOcelot(configuration).Wait();
            //var builder = new OcelotPipelineBuilder(app.ApplicationServices);
            //var configuration = builder.BuildCustomeOcelotPipeline(new OcelotPipelineBuilderConfiguration()
            //{
            //    PreAuthenticationMiddlewareTypes = new[]
            //        {
            //                typeof(MyCustomMiddleware)
            //            }
            //}
            //);

            //app.UseOcelot(configuration).Wait();

        }
    }
}
