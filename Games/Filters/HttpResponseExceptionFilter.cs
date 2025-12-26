using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Games.Filters;

public class HttpResponseExceptionFilter : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is HttpResponseException ex)
        {
            context.Result = ex.Body switch
            {

                null => new StatusCodeResult((int)ex.HttpStatusCode),
                { } body => new ObjectResult(body)
                {
                    StatusCode = (int)ex.HttpStatusCode,
                },
            };
        }
    }
}

public class HttpResponseException : Exception
{
    public HttpStatusCode HttpStatusCode { get; }
    public object? Body { get; }

    public HttpResponseException(HttpStatusCode httpStatusCode, object? body, Exception? innerException = null)
        : base($"The server will respond to this request with {httpStatusCode}", innerException)
    {
        HttpStatusCode = httpStatusCode;
        Body = body;
    }
}
