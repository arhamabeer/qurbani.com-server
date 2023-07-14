

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


#region Api Versioning
// Add API Versioning to the Project
builder.Services.AddApiVersioning(config =>
{
	// Specify the default API Version as 1.0
	config.DefaultApiVersion = new ApiVersion(1, 0);
	// If the client hasn't specified the API version in the request, use the default API version number 
	config.AssumeDefaultVersionWhenUnspecified = true;
	// Advertise the API versions supported for the particular endpoint
	config.ReportApiVersions = true;
});
// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
	setup.GroupNameFormat = "'v'VVV";
	setup.SubstituteApiVersionInUrl = true;
});
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
	{
		// top level
		c.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "aaa",
			Version = "v1",
			Contact = new OpenApiContact
			{
				Name = "aas",
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

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
