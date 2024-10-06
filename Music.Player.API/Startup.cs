using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Music.Player.API.Middleware;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music.Player.API
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
            services.AddScoped<ISongRepository, SongRepository>();
            services.AddScoped<IAlbumRepository, AlbumRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ISongService, SongService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddAWSService<IAmazonS3>(new AWSOptions
            {
                Region = Amazon.RegionEndpoint.GetBySystemName(Configuration["AWS:Region"])
            });
            services.AddSingleton<IAwsConfigurationService, AwsConfigurationService>();

            services.AddDbContext<Context>(options => options.UseNpgsql(Configuration.GetConnectionString("DBLocalConnection")));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //Configure Options for multi-part length and avoid issue uploading large files
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set 
                                                                 //default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;

            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set 
                                                                  //default value is: 30 MB
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddAuthorization();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

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
