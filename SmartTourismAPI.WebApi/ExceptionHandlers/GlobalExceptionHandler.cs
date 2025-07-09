﻿using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartTourismAPI.WebApi.ExceptionHandlers;

public sealed class GlobalExceptionHandler() : IExceptionHandler {
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {

        (int status, string detail, IDictionary<string, object?> extensions) = exception switch {
            ValidationException validationException =>
                (StatusCodes.Status400BadRequest,
                "Dữ liệu không hợp lệ.",
                new Dictionary<string, object?> {
                    ["errors"] = validationException.Errors.Select(x => new {
                        Property = x.PropertyName,
                        Error = x.ErrorMessage
                    }).Distinct()
                }),
            BadHttpRequestException badRequestException =>
                (StatusCodes.Status400BadRequest,
                badRequestException.Message,
                default!),
            _ =>
                (StatusCodes.Status500InternalServerError,
                exception.Message,
                default!)
        };

        var problemDetails = new ProblemDetails {
            Title = exception.GetType().Name,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Status = status,
            Detail = detail,
            Extensions = extensions
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
