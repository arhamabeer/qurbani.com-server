

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Qurabani.com_Server.Helpers;
using Qurabani.com_Server.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


#region Api Versioning
// Add API Versioning to the Project
//builder.Services.AddApiVersioning(config =>
//{
//	// Specify the default API Version as 1.0
//	config.DefaultApiVersion = new ApiVersion(1, 0);
//	// If the client hasn't specified the API version in the request, use the default API version number 
//	config.AssumeDefaultVersionWhenUnspecified = true;
//	// Advertise the API versions supported for the particular endpoint
//	config.ReportApiVersions = false;
//});
//// Add ApiExplorer to discover versions
//builder.Services.AddVersionedApiExplorer(setup =>
//{
//	setup.GroupNameFormat = "'v'VVV";
//	setup.SubstituteApiVersionInUrl = false;
//});
#endregion

#region SeriLog Confg
Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.File(@"D:\.NET logs\qurbani.com_server_Logs\logs.txt", rollingInterval: RollingInterval.Day)
			.CreateLogger();
#endregion



#region SYNC SERVICE
//IHost host = Host.CreateDefaultBuilder(args)
//// Below code has been commented for testing purpose
//.ConfigureServices((hostContext, services) =>
//{
	

//})
//.UseSerilog().Build();

//await host.RunAsync();
#endregion


builder.Services.AddControllers();
builder.Services.AddDbContext<QurbaniContext>();

//CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.WithOrigins("http://localhost:3000")
			   .AllowAnyHeader()
			   .AllowAnyMethod();
	});
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// JWT AUTHENTUCATION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration.GetSection("Secret").GetSection("Issuer").Value,
		ValidAudience = builder.Configuration.GetSection("Secret").GetSection("Audience").Value,
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Secret").GetSection("Key").Value))
	});

builder.Services.AddSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddScoped<Salt>();
builder.Services.AddScoped<Pepper>();
builder.Services.AddScoped<Hasher>();
builder.Services.AddScoped<VerifyPasswords>();

builder.Services.AddSwaggerGen(c =>
	{
		// top level
		c.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "Qurbani.com",
			Version = "v1",
			Contact = new OpenApiContact
			{
				Name = "Arham Abeer",
				Url = new Uri("https://aaa-portfolio.web.app")
			}
		});

		// security header
		c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
		{
			Name = "Authorization",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.ApiKey,
			Scheme = JwtBearerDefaults.AuthenticationScheme
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme
				},
				Scheme = "Oauth2",
				Name = JwtBearerDefaults.AuthenticationScheme,
				In = ParameterLocation.Header
			},
			new List<string>()
			}
		});
	});

var app = builder.Build();

//var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS
app.UseCors();

// AUTH
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
