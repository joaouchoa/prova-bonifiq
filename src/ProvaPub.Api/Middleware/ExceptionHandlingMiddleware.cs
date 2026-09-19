using System.Net;
using System.Text.Json;
using FluentValidation;

namespace ProvaPub.Api.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro não tratado em {Method} {Path}", context.Request.Method, context.Request.Path);
				await WriteErrorResponseAsync(context, ex);
			}
		}

		private static Task WriteErrorResponseAsync(HttpContext context, Exception exception)
		{
			var (statusCode, title) = MapException(exception);
			var isServerError = statusCode == (int)HttpStatusCode.InternalServerError;

			context.Response.ContentType = "application/problem+json";
			context.Response.StatusCode = statusCode;

			var problem = new
			{
				status = statusCode,
				title,
				detail = isServerError ? "Ocorreu um erro interno inesperado." : GetDetail(exception)
			};

			return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
		}

		private static string GetDetail(Exception exception) => exception is ValidationException validationException
			? string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage))
			: exception.Message;

		private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
		{
			ValidationException => ((int)HttpStatusCode.BadRequest, "Requisição inválida"),
			ArgumentException => ((int)HttpStatusCode.BadRequest, "Requisição inválida"),
			InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Regra de negócio violada"),
			_ => ((int)HttpStatusCode.InternalServerError, "Erro interno")
		};
	}

	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
			=> app.UseMiddleware<ExceptionHandlingMiddleware>();
	}
}
