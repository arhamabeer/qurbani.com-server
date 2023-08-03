

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Qurabani.com_Server.Helpers;
using Qurabani.com_Server.Models;

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
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = "https://localhost:7267/", // Change this to your issuer
		ValidAudience = "https://localhost:7267/", // Change this to your audience
		IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Secret").GetSection("Key").Value)) // Change this to your secret key
	};
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<JwtGenerator>();

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
		c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
		{
			Description = "Token Authorization header using the ApiKey scheme",
			Type = SecuritySchemeType.ApiKey,
			In = ParameterLocation.Header,
			Name = "X-Api-Key"
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "ApiKey"
				}
			},
			new string[] {}
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
