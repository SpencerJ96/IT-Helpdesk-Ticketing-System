using System.Text;
using HelpDeskApi.Data;
using HelpDeskApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


		//Add AuthService to Program.cs so it can be injected in other places (Scoped lifetime - one instance per request. Created when request comes in anad disposed when done)
		//One authservice per request = every reg or login gets its own dedicated instance
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TicketService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddAuthentication("Bearer") // Whenever someone needs auth the DEFAULT AUTHENTICATION SCHEME method is the one labelled "bearer" 
		//Addjwtbearer registers the logic and files it under the name bearer 
	.AddJwtBearer(options => //options is the settings for JWT Handler ready to config
	{
		options.TokenValidationParameters = new TokenValidationParameters //Tokenvalidparms = class the holds rules for making valid tokens. Create fresh instance so the JWT handler knows what to check.
		{	//Fill it with our own rules, always check validateissuer, validateaudience etc.
			ValidateIssuer = true, 
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			//Three lines below are the expected values from appsettings, this is what the Validate booleans above are compared against.
			ValidIssuer = builder.Configuration["JwtSettings:Issuer"], 
			ValidAudience = builder.Configuration["JwtSettings:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
		};
	});
	builder.Services.AddAuthorization();
	builder.Services.AddControllers();

//Everything above this is configuration. Adding services, telling it what DB and what it communicates in

//Everything below this is runtime behaviour, telling the app how to handle requests.
var app = builder.Build();

	if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

//