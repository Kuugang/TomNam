using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using TomNam.Data;
using TomNam.Models;
using TomNam.Middlewares;
using TomNam.Middlewares.Filters;
using TomNam.Interfaces;
using TomNam.Services;
using TomNam.Repository;


namespace TomNam
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var root = Directory.GetCurrentDirectory();
            var dotenvPath = Path.Combine(root, ".env");

            if (File.Exists(dotenvPath))
            {
                DotNetEnv.Env.Load(dotenvPath);
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            // Console.WriteLine("Environment Variable: " +
            //     Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION"));
            // Console.WriteLine("Environment Variable: " +
            //     Environment.GetEnvironmentVariable("JWT__SECRET"));
            // Console.WriteLine("Configuration Connection String: " +
            //     Configuration.GetConnectionString("DefaultConnection"));
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            });

            services.AddSingleton<JwtAuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IKarenderyaService, KarenderyaService>();
            services.AddScoped<IFoodService, FoodService>();
            services.AddScoped<ICartItemService, CartItemService>();

            services.AddScoped<IFileUploadService, FileUploadService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IKarenderyaRepository, KarenderyaRepository>();
            services.AddScoped<IFoodRepository, FoodRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();



            // Configure Entity Framework with PostgreSQL
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")
                ));

            // Configure Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<DataProtectorTokenProvider<User>>("MyApp");

            // Configure Authentication with JWT Bearer
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })


            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),
                    RoleClaimType = "Role"
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerPolicy", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Owner"));

                options.AddPolicy("CustomerPolicy", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Customer"));

                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
            });

            // Enable Swagger for API documentation
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TomNam v1"));
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
                RequestPath = "/Uploads"
            });

            // Register JwtAuthenticationService as middleware
            app.UseMiddleware<JwtAuthenticationService>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
