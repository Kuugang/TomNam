// namespace Tomnam.Middlewares{
//   // KarenderyaProtectedStaticFilesOptions
//   public class KarenderyaProtectedStaticFilesOptions : ProtectedStaticFilesOptions
//   {
//       public string BusinessId { get; set; }
//       public IBusinessService BusinessService { get; set; }  // Service to check business ownership
//   }
//   
//   public class KarenderyaProtectedFilesMiddleware
//   {
//       private readonly RequestDelegate _next;
//       private readonly StaticFileOptions _options;
//       private readonly string _businessId;
//       private readonly IBusinessService _businessService;
//   
//       public BusinessProtectedStaticFilesMiddleware(
//           RequestDelegate next,
//           BusinessProtectedStaticFilesOptions options)
//       {
//           _next = next;
//           _businessId = options.BusinessId;
//           _businessService = options.BusinessService;
//   
//           _options = new StaticFileOptions
//           {
//               FileProvider = new PhysicalFileProvider(options.PhysicalPath),
//               RequestPath = options.RequestPath,
//               OnPrepareResponse = async ctx =>
//               {
//                   if (!await IsAuthorizedAsync(ctx.Context.User))
//                   {
//                       ctx.Context.Response.StatusCode = 403; // Forbidden
//                       ctx.Context.Response.ContentLength = 0;
//                       ctx.Context.Response.Body = Stream.Null;
//                   }
//               }
//           };
//       }
//   
//       private async Task<bool> IsAuthorizedAsync(ClaimsPrincipal user)
//       {
//           // Check if user is admin
//           if (user.IsInRole("Admin"))
//               return true;
//   
//           // Check if user is authenticated
//           if (!user.Identity.IsAuthenticated)
//               return false;
//   
//           // Get user ID from claims
//           var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//           if (string.IsNullOrEmpty(userId))
//               return false;
//   
//           // Check if user owns the business
//           return await _businessService.IsBusinessOwnerAsync(userId, _businessId);
//       }
//   
//       public async Task InvokeAsync(HttpContext context)
//       {
//           var staticFileMiddleware = new StaticFileMiddleware(
//               _next,
//               Microsoft.Extensions.Options.Options.Create(_options),
//               context.RequestServices.GetRequiredService<IWebHostEnvironment>(),
//               context.RequestServices.GetRequiredService<ILoggerFactory>());
//   
//           await staticFileMiddleware.Invoke(context);
//       }
//   }
//   
//   // Extension method
//   public static class BusinessProtectedStaticFilesMiddlewareExtensions
//   {
//       public static IApplicationBuilder UseBusinessProtectedStaticFiles(
//           this IApplicationBuilder app,
//           Action<BusinessProtectedStaticFilesOptions> configureOptions)
//       {
//           var options = new BusinessProtectedStaticFilesOptions();
//           configureOptions(options);
//           return app.UseMiddleware<BusinessProtectedStaticFilesMiddleware>(options);
//       }
//   }
//   }
// }
