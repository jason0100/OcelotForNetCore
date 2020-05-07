using APIGateway.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Middleware;
using Ocelot.Logging;

namespace APIGateway.Middleware
{
    //public class MyMiddleware : OcelotMiddleware
    public class CheckTokenMiddleware
    {
        //private readonly DataContext _db;
        //private readonly OcelotRequestDelegate _next;
        private readonly RequestDelegate _next;

        //private readonly IHttpContextAccessor _contextAccessor;
        public CheckTokenMiddleware(
            //DataContext db
            //, IHttpContextAccessor contextAccessor
                 RequestDelegate next
               )
        {
            //_db = db;
            //_contextAccessor = contextAccessor;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataContext db)
        {

            //string newPath = null;
            //var url = context.Request.Path;
            //if (url == "/dan")
            //{
            //    newPath = "/customer";
            //    context.Request.Path = newPath;
            //    context.Request.Method = "GET";
            //    context.Request.ContentType = "application/json";

            //}
            //await _next.Invoke(context);

            var token = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                //await context.Response.WriteAsync($"token={token}");
                var existUser = db.Users.FirstOrDefault(a => a.token == token);
                //var existUser = "";
                if (existUser == null)
                {
                    await context.Response.WriteAsync($"Invalid request. \r\n");
                }
                else
                {
                    string newPath = null;
                    var url = context.Request.Path;
                    if (url == "/dan")
                    {
                        newPath = "/customer";
                        context.Request.Path = newPath;
                        context.Request.Method = "GET";
                        context.Request.ContentType = "application/json";

                    }
                    await _next.Invoke(context); ;
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
