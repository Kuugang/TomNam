// using System.Security.Claims;
// using Microsoft.Extensions.FileProviders;
//
// namespace TomNam.MiddleWares{
//   public class ProtectedStaticFilesOptions
//   {
//       public string PhysicalPath { get; set; }
//       public string RequestPath { get; set; }
//       public Func<ClaimsPrincipal, bool> AuthorizationPredicate { get; set; }
//   }
//   
//   public class ProtectedStaticFilesMiddleware
//   {
//       private readonly RequestDelegate _next;
//       private readonly StaticFileOptions _options;
//       private readonly Func<ClaimsPrincipal, bool> _authorizationPredicate;
//   
//       public ProtectedStaticFilesMiddleware(
//           RequestDelegate next,
//           ProtectedStaticFilesOptions options)
//       {
//           _next = next;
//           _authorizationPredicate = options.AuthorizationPredicate ?? 
//               (user => user.Identity.IsAuthenticated);
//   
//           _options = new StaticFileOptions
//           {
//               FileProvider = new PhysicalFileProvider(options.PhysicalPath),
//               RequestPath = options.RequestPath,
//               OnPrepareResponse = ctx =>
//               {
//                   if (!_authorizationPredicate(ctx.Context.User))
//                   {
//                       ctx.Context.Response.StatusCode = 401;
//                       ctx.Context.Response.ContentLength = 0;
//                       ctx.Context.Response.Body = Stream.Null;
//                   }
//               }
//           };
//       }
//
//
//       public async Task InvokeAsync(HttpContext context)
//       {
//         // Create a new instance of StaticFileMiddleware
//         var staticFileMiddleware = new StaticFileMiddleware(
//             _next,
//             Microsoft.Extensions.Options.Options.Create(_options),
//             context.RequestServices.GetRequiredService<IWebHostEnvironment>(),
//             context.RequestServices.GetRequiredService<ILoggerFactory>());
//
//         // Process the request
//         await staticFileMiddleware.Invoke(context);
//       }
//   }
//   
//   // Updated extension method
//   public static class ProtectedStaticFilesMiddlewareExtensions
//   {
//       public static IApplicationBuilder UseProtectedStaticFiles(
//           this IApplicationBuilder app,
//           Action<ProtectedStaticFilesOptions> configureOptions)
//       {
//           var options = new ProtectedStaticFilesOptions();
//           configureOptions(options);
//           return app.UseMiddleware<ProtectedStaticFilesMiddleware>(options);
//       }
//   }
// }
