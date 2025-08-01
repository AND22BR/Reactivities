using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(FluentValidation.ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; // Internal Server Error

            var response = env.IsDevelopment()
                ? new AppException
                (
                    context.Response.StatusCode,
                    ex.Message,
                    ex.StackTrace
                ) : new AppException
                (
                    context.Response.StatusCode,
                    ex.Message,
                    null
                );

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private async Task HandleValidationException(HttpContext context, FluentValidation.ValidationException ex)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (ex.Errors is not null)
            {
                foreach (var error in ex.Errors)
                {
                    if (!validationErrors.ContainsKey(error.PropertyName))
                    {
                        validationErrors[error.PropertyName] = new string[] { error.ErrorMessage };
                    }
                    else
                    {
                        var existingErrors = validationErrors[error.PropertyName].ToList();
                        existingErrors.Add(error.ErrorMessage);
                        validationErrors[error.PropertyName] = existingErrors.ToArray();
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest; // Bad Request

            var validationProblemDetails = new ValidationProblemDetails(validationErrors)
            {
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Type = "ValidationFailure",
            };

            await context.Response.WriteAsJsonAsync(validationProblemDetails);
        }
    }
}