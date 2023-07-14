using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal_Chat_Application;
using Minimal_Chat_Application.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var connectionString = builder.Configuration.GetConnectionString("MySqlDb");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
        //builder.Services.AddAuthentication(x =>
        //{
        //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //}).AddJwtBearer(x =>
        //{
        //    x.RequireHttpsMetadata = false;
        //    x.SaveToken = true;
        //    x.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("veryverysecret.....")),
        //        //Validite the receipient of the token is authorize to receive
        //        ValidateAudience = false,
        //        //Validite the Issuer that Validiate the token 
        //        ValidateIssuer = false,
        //        //the minimum session time is 5 min but these will help for exact time.
        //        ClockSkew = TimeSpan.Zero
        //    };
        //});


        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        builder.Host.UseSerilog((hostingContext, LoggerConfig) =>
        {
            LoggerConfig.ReadFrom.Configuration(hostingContext.Configuration);
        });
        //builder.Services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(
        //    policy =>
        //    {
        //        policy.AllowAnyHeader().WithOrigins("https://localhost:4200", "http://localhost:4200").AllowAnyMethod();
        //    });
        //});

        //builder.Services.AddCors(option =>
        //{
        //    option.AddPolicy("MyPolicy", builder =>
        //    {
        //        builder.AllowAnyOrigin()
        //        .AllowAnyMethod()
        //        .AllowAnyHeader();
        //    });
        //});

        builder.Services.AddCors(
            options => { 
                options.AddPolicy("AllowAll", 
                    policy => { 
                        policy.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); 
                    }); 
            });




        var app = builder.Build();
       
        
        
        //app.UseRequestLoggingMiddleware();
        //app.UseHttpLogging();



        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseCors("AllowAll");
        app.UseRequestLoggingMiddleware();

        app.MapControllers();

        app.Run();
    }
   

}