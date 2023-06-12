using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDDCqrsEs.Application.Queries.ActivityQueries;
using DDDCqrsEs.Infrastructure.Bootstrap;
using DDDCqrsEs.Persistance.Bootstrap;
using Joonasw.AspNetCore.SecurityHeaders;
using MediatR;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using DDDCqrsEs.Persistance;
using DDDCqrsEs.Application.Bootstrap;
using Microsoft.Extensions.Options;
using FluentValidation;
using DDDCqrsEs.Application.Commands.ToDoCommands;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.Antiforgery;
using DDDCqrsEs.Domain.Entities;
using Quotation.Domain.Entities.Authentication;
using System.Collections.Generic;
using DDDCqrsEs.Infrastructure.Services;
using DDDCqrsEs.Application.Services;
using DDDCqrsEs.Application.Models.AuthenticationModels;

namespace DDDCqrsEs.WebUI
{
    public class Startup
    {
        private const string ReportViolationsTo = "/csp-report";
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddDistributedMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Task.CompletedTask;
                    };
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.MaxAge = TimeSpan.FromDays(365);
            });

            var connectionString = Configuration.GetConnectionString("ToDoDatabase");
            services.AddDbContext<ToDoDbContext>(options => options.UseSqlServer(connectionString));
            var tableString = Configuration.GetConnectionString("TableConnectionString");
            var serviceBusString = Configuration.GetConnectionString("ServiceBusString");


            services.AddTransient<ITableStorageConnection, TableStorageConnection>(service => new TableStorageConnection(tableString));
            services.AddTransient<IBaseEventPublisher, ServiceBusPublisher>(service => new ServiceBusPublisher(serviceBusString, "stockevents"));

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-GB")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en-GB", uiCulture: "en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                var defaultRequestProvider =
                    options.RequestCultureProviders.FirstOrDefault(rcp =>
                        rcp.GetType() == typeof(AcceptLanguageHeaderRequestCultureProvider));
                if (defaultRequestProvider != null)
                    options.RequestCultureProviders.Remove(defaultRequestProvider);

                options.RequestCultureProviders.Insert(0,
                    new AcceptLanguageHeaderRequestCultureProvider()
                    {
                        Options = options
                    });
            });
            services.AddMediatR(typeof(ToDoQueryHandler).GetTypeInfo().Assembly);
            services.AddValidatorsFromAssembly(typeof(AddToDoCommandValidator).Assembly);
            services.RegisterInfrastructureComponents();
            services.RegisterApplicationServices();
            services.RegisterRepositories();
            services.RegisterWebAPIServices();
            services.AddCsp(nonceByteAmount: 32);

            services.AddIdentity<User, ApplicationRole>()
                  .AddEntityFrameworkStores<ToDoDbContext>()
                  .AddDefaultTokenProviders();

            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                      .RequireAuthenticatedUser()
                      .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            //services.AddOpenApiDocument();
            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";
            //});

            //services.AddAppEvents(opt =>
            //{
            //    opt.Handlers = new List<AppEventHandlerDescriptor>
            //    {
            //        new AppEventHandlerDescriptor(typeof(UserActionLogEventHandler))
            //    };
            //});

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IAntiforgery antiforgery)//, IAsyncQueryService asyncQueryService, IQueryHelperService queryHelperService, AppConfigurations appConfigurations)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "My API V1");
            });

            var supportedCultures = new[] { "en-GB" };
            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
                SupportedCultures = supportedCultures.Select(n => CultureInfo.GetCultureInfo(n)).ToList(),
                SupportedUICultures = supportedCultures.Select(n => CultureInfo.GetCultureInfo(n)).ToList(),
            };
            app.UseRequestLocalization(requestLocalizationOptions);
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.Use(next => context =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = true, Path = "/" });
                //setting HttpOnly=true,Secure=true will make browser cookie secure 
                return next(context);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            //TODO: See if this really needs to be removed
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Xss-Protection", "1;mode=block");
                context.Response.Headers["Cache-control"] = "private,no-cache,no-store, must-revalidate ,max-age=0";
                await next();
            });

            // Content Security Policy
            app.UseCsp(csp =>
            {
                csp.ByDefaultAllow
                    .FromSelf();

                csp.AllowFormActions
                .ToSelf();

                // Allow JavaScript from:
                csp.AllowScripts
                    .FromSelf() //This domain
                    .AddNonce()
                    .AllowUnsafeInline();

                // CSS allowed from:
                csp.AllowStyles
                    .FromSelf()
                  .AllowUnsafeInline();

                csp.AllowImages
                    .FromSelf();

                // HTML5 audio and video elemented sources can be from:
                csp.AllowAudioAndVideo
                    .FromNowhere();

                // Contained iframes can be sourced from:
                csp.AllowFrames
                    .FromNowhere(); //Nowhere, no iframes allowed

                // Allow AJAX, WebSocket and EventSource connections to:
                csp.AllowConnections
                    .ToSelf();

                // Allow fonts to be downloaded from:
                csp.AllowFonts
                   .FromSelf();

                // Allow object, embed, and applet sources from:
                csp.AllowPlugins
                    .FromNowhere();

                // Allow other sites to put this in an iframe?
                csp.AllowFraming
                    .FromNowhere(); // Block framing on other sites, equivalent to X-Frame-Options: DENY

                // Do not block violations, only report
                // This is a good idea while testing your CSP
                // Remove it when you know everything will work
                csp.SetReportOnly();
                // Where should the violation reports be sent to?
                csp.ReportViolationsTo(ReportViolationsTo);

                // Do not include the CSP header for requests to the /api endpoints
                csp.OnSendingHeader = context =>
                {
                    context.ShouldNotSend = context.HttpContext.Request.Path.StartsWithSegments("/api");
                    return Task.CompletedTask;
                };
            });
            CreateStartupUsers(serviceProvider);
        }

        private void CreateStartupUsers(IServiceProvider serviceProvider)
        {
            var userService = serviceProvider.GetRequiredService<IUserService>();

            if (userService.IsUsernameUnique("admin"))
            {
                userService.Register(new RegisterUserModel
                {
                    Username = "admin",
                    Password = "P@ssw0rd",
                    Email = "admin@webdotnet.com",
                    Roles = new List<RoleModel>
                    {
                        new RoleModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            RoleName = RoleTypeEnum.Admin.ToString(),
                            RoleType = RoleTypeEnum.Admin
                        }
                    }
                });
            }
        }

    }
}
