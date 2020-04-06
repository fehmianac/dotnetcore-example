
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace DotnetCore.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException validationException)
            {
                throw validationException;
            }
        }
    }
}