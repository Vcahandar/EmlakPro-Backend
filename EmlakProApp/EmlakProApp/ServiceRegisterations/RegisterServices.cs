using EmlakProApp.Data;
using EmlakProApp.Mapper;
using EmlakProApp.Models.Identity;
using EmlakProApp.Services.AccountServices;
using EmlakProApp.Services.EmailServices;
using EmlakProApp.Services.JWTServices;
using EmlakProApp.Utilities;
using EmlakProApp.Validators.AccountDtoValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;

namespace EmlakProApp.ServiceRegisterations
{
	public static class RegisterServices
	{
		public static void Register(this IServiceCollection services, IConfiguration config)
		{
			services.AddControllers()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				})
				.AddFluentValidation(opt =>
					opt.RegisterValidatorsFromAssemblyContaining<RegisterDtoValidator>()
				);

			services.AddDbContext<AppDbContext>(opt =>
				opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
			);

			services.AddAutoMapper(cfg => cfg.AddProfile(new MapperProfile()));
			services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();


			// Configure Identity
			services.AddIdentity<AppUser, IdentityRole>(opt =>
			{
				opt.Lockout.MaxFailedAccessAttempts = 3;
				opt.Lockout.AllowedForNewUsers = true;
				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);

				opt.Password.RequiredLength = 8;
				opt.Password.RequireLowercase = true;
				opt.Password.RequireUppercase = true;
				opt.Password.RequireNonAlphanumeric = true;

				opt.User.RequireUniqueEmail = true;
				opt.SignIn.RequireConfirmedEmail = true;
			})
			.AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

			// Configure Email Services
			services.Configure<EmailConfig>(config.GetSection("EmailSetting"));
			services.AddSingleton(config.GetSection("EmailSetting").Get<EmailConfig>());

			// Configure Authentication
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = config["Jwt:Issuer"],
					ValidAudience = config["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(config["Jwt:Key"])
					),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.Zero
				};
			})
			.AddGoogle(options =>
			{
				options.ClientId = config["Authentication:Google:ClientId"];
				options.ClientSecret = config["Authentication:Google:ClientSecret"];
				options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			});

			// Configure Swagger
			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
				opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Please enter token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "bearer"
				});

				opt.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[] { }
					}
				});
			});

			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<IJWTService, JWTService>();
			services.AddScoped<IEmailService, EmailService>();
		}
	}
}
