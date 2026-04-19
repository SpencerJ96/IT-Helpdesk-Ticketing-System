using HelpDeskApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Everything above this is configuration. Adding services, telling it what DB and what it communicates in

//Everything below this is runtime behaviour, telling the app how to handle requests.
var app = builder.Build();

	if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();

//