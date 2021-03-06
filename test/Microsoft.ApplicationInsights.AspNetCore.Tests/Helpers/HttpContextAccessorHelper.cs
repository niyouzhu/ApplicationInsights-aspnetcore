﻿namespace Microsoft.ApplicationInsights.AspNetCore.Tests.Helpers
{
    using System;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    
    public static class HttpContextAccessorHelper
    {
        public static HttpContextAccessor CreateHttpContextAccessor(RequestTelemetry requestTelemetry = null, ActionContext actionContext = null)
        {
            var services = new ServiceCollection();

            var request = new DefaultHttpContext().Request;
            request.Method = "GET";
            request.Path = new PathString("/Test");
            var contextAccessor = new HttpContextAccessor { HttpContext = request.HttpContext };

            services.AddSingleton<IHttpContextAccessor>(contextAccessor);

            if (actionContext != null)
            {
                var si = new ActionContextAccessor();
                si.ActionContext = actionContext;
                services.AddSingleton<IActionContextAccessor>(si);
            }

            if (requestTelemetry != null)
            {
                services.AddSingleton<RequestTelemetry>(requestTelemetry);
            }

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            contextAccessor.HttpContext.RequestServices = serviceProvider;

            return contextAccessor;
        }

        public static HttpContextAccessor CreateHttpContextAccessorWithoutRequest(HttpContextStub httpContextStub, RequestTelemetry requestTelemetry = null)
        {
            var services = new ServiceCollection();

            var contextAccessor = new HttpContextAccessor { HttpContext = httpContextStub };

            services.AddSingleton<IHttpContextAccessor>(contextAccessor);

            if (requestTelemetry != null)
            {
                services.AddSingleton<RequestTelemetry>(requestTelemetry);
            }

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            contextAccessor.HttpContext.RequestServices = serviceProvider;

            return contextAccessor;
        }
    }
}
