namespace pyro.Scripts.Utils
{
    public class Logger
    {
        private readonly RequestDelegate _request;

        public Logger(RequestDelegate requestDelegate)
        {
            _request = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _request(httpContext);

            var method = httpContext.Request.Method;
            var path = httpContext.Request.Path;
            var statusCode = httpContext.Response.StatusCode;

            var color = statusCode >= 400 ? Utils.redColor : Utils.greenColor;

            Console.WriteLine($"{Utils.purpleColor}[BACKEND]{Utils.resetColor} {method} {color}{path}{Utils.resetColor} | Code: {statusCode}");
        }
    }
}