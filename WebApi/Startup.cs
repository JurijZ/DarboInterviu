using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using WebApi.Services;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using NLog.Extensions.Logging;
using NLog.Web;
using System;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("TestDb"));
            services.AddDbContext<DataContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
            });

            services.AddAutoMapper();

            // Populate strongly typed config objects with the Environment variables and add to DI
            services.Configure<AppSettings>(Configuration);
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Employer", policy => policy.RequireClaim("Role", "Employer"));
                options.AddPolicy("Support", policy => policy.RequireClaim("Role", "Support"));                
                options.AddPolicy("Candidate", policy => policy.RequireClaim("Role", "Candidate"));                
            });            

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var adminService = context.HttpContext.RequestServices.GetRequiredService<IAdminService>();
                        var applicationService = context.HttpContext.RequestServices.GetRequiredService<IApplicationService>();
                        //var userId = int.Parse(context.Principal.Identity.Name); 
                        
                        var t = context.Principal.Claims.ToArray();

                        //var logger = NLog.LogManager.GetCurrentClassLogger();
                        //logger.Info("Role claim - " + t[1].Type + " " + t[1].Value);

                        if (context.Principal.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value == "Support") //Support Authentication
                        {
                            var accountGuid = context.Principal.Identity.Name;

                            var admin = adminService.GetById(accountGuid);

                            if (admin == null)
                            {
                                // return unauthorized admin
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        }
                        else if (context.Principal.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value == "Employer") //Employer Authentication
                        {
                            var accountGuid = context.Principal.Identity.Name;

                            var user = userService.GetById(accountGuid);

                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }
                            return Task.CompletedTask;
                        }
                        else if (context.Principal.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value == "Candidate") //Candidate Authentication
                        {
                            // Add a check her for the Candidate token extra verification
                            return Task.CompletedTask;
                        }
                        else
                        {
                            context.Fail("Unauthorized");
                            return Task.CompletedTask;
                        }                        
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("JWTSECRET"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<ISupportService, SupportService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            env.ConfigureNLog("NLog.Config");            

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Darbo Interviu API V1");
            });

            app.UseMvc();
        }
    }
}
