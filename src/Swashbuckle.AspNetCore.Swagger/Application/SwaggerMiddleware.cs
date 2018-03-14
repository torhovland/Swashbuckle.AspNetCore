﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Swashbuckle.AspNetCore.Swagger
{
    public class SwaggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISwaggerProvider _swaggerProvider;
        private readonly JsonSerializer _swaggerSerializer;
        private readonly SwaggerOptions _options;
        private readonly TemplateMatcher _requestMatcher;
        private readonly TemplateMatcher _openApiRequestMatcher;

        public SwaggerMiddleware(
            RequestDelegate next,
            ISwaggerProvider swaggerProvider,
            IOptions<MvcJsonOptions> mvcJsonOptions,
            SwaggerOptions options)
        {
            _next = next;
            _swaggerProvider = swaggerProvider;
            _swaggerSerializer = SwaggerSerializerFactory.Create(mvcJsonOptions);
            _options = options;
            _requestMatcher = new TemplateMatcher(TemplateParser.Parse(options.RouteTemplate), new RouteValueDictionary());
            _openApiRequestMatcher = new TemplateMatcher(TemplateParser.Parse(options.OpenApiRouteTemplate), new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string documentName;
            if (!RequestingSwaggerDocument(httpContext.Request, out documentName))
            {
                await _next(httpContext);
                return;
            }

            var basePath = string.IsNullOrEmpty(httpContext.Request.PathBase)
                ? null
                : httpContext.Request.PathBase.ToString();

            var path = httpContext.Request.Path;

            Document swagger = path.HasValue && path.Value.EndsWith("/openapi.json")
                ? (Document)_swaggerProvider.GetOpenApi(documentName, null, basePath)
                : (Document)_swaggerProvider.GetSwagger(documentName, null, basePath);

            // One last opportunity to modify the Swagger Document - this time with request context
            foreach (var filter in _options.PreSerializeFilters)
            {
                filter(swagger, httpContext.Request);
            }

            await RespondWithSwaggerJson(httpContext.Response, swagger);
        }

        private bool RequestingSwaggerDocument(HttpRequest request, out string documentName)
        {
            documentName = null;
            if (request.Method != "GET") return false;

			var routeValues = new RouteValueDictionary();
            if (!_requestMatcher.TryMatch(request.Path, routeValues) && !_openApiRequestMatcher.TryMatch(request.Path, routeValues)) return false;
            if (!routeValues.ContainsKey("documentName")) return false;

            documentName = routeValues["documentName"].ToString();
            return true;
        }

        private async Task RespondWithSwaggerJson(HttpResponse response, Document swagger)
        {
            response.StatusCode = 200;
            response.ContentType = "application/json";

            var jsonBuilder = new StringBuilder();
            using (var writer = new StringWriter(jsonBuilder))
            {
                _swaggerSerializer.Serialize(writer, swagger);
                await response.WriteAsync(jsonBuilder.ToString(), new UTF8Encoding(false));
            }
        }
    }
}
