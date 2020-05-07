using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIGateway.Middleware;
using Microsoft.AspNetCore.Builder;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;

namespace APIGateway.Extensions
{
    public static class CustomMiddlewareExtension
    {

      

        public static IApplicationBuilder UseCheckTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckTokenMiddleware>();
        }

        

    }
}
