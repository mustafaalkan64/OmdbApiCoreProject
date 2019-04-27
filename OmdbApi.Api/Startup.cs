using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OmdbApi.DAL.Uow;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using OmdbApi.DAL.Models;
using OmdbApi.Domain.IServices;
using OmdbApi.Business.Services;
using OmdbApi.DAL.Consts;
using OmdbApi.DAL.EFDbContext;

namespace OmdbApi.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();

            //services.AddDbContext<OmdApiDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<OmdApiDbContext>(opts => opts.UseSqlServer(AppSettingsParameters.ConnectionString));

            var secret = AppSettingsParameters.Secret;
            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddMvcCore()
                .AddAuthorization() // Note - this is on the IMvcBuilder, not the service collection
                .AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MovieResponse, Movie>();
                cfg.CreateMap<Movie, MovieResponse>()
                    .ForMember(x => x.Error, opt => opt.Ignore())
                    .ForMember(x => x.Response, opt => opt.Ignore());
                cfg.CreateMap<UserDto, User>()
                    .ForMember(x => x.Hash, opt => opt.Ignore())
                    .ForMember(x => x.Salt, opt => opt.Ignore());
                cfg.CreateMap<User, UserDto>()
                    .ForMember(x => x.Password, opt => opt.Ignore());
                cfg.CreateMap<MovieDto, Movie>()
                    .ForMember(x => x.Id, opt => opt.Ignore());
                cfg.CreateMap<Movie, MovieDto>();
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHangfire(_ => _.UseSqlServerStorage(Configuration.GetValue<string>("HangfireDbConn")));

            // configure DI for application services

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICacheManagementService, CacheManagementService>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();

            // logging
            loggerFactory.AddSerilog();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            // HangFire Middleware
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            //// Update All Movies Per 10 Minutes..
            RecurringJob.AddOrUpdate<IMovieService>("movieService",
                movieService => movieService.UpdateAllMovies(), "*/10 * * * *");

            //app.UseHttpsRedirection();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
