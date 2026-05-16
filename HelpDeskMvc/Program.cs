using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
					//Method, Registers controllers and view rendering
builder.Services.AddControllersWithViews();

					//Add to DI container, call it Helpdeskapi
builder.Services.AddHttpClient("HelpDeskApi", client =>
{					//C# Class typed object that reps a web address so C# can work with it 
								//SET THE ROOT url for all requests made by that client
								//http://localhost:5148/api/ticket turns into api/ticket client auto prepares to every request 
	client.BaseAddress = new Uri("http://localhost:5148/");
});  
				//Add authentication to DI Container
							//Constant that equals the string "Cookies". Register authentication Scheme as "Cookies"
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => //AddCookie registers itself by default as "Cookies" - Here all the cookie logic will be set so auth knows what to do
	{   //Take the options object from the lambada and edit the Loginpath for unauth users to "/auth/login
		options.LoginPath = "/Auth/Login"; //Anyone without authen is redirected here
	});

builder.Services.AddAuthorization(); //Unlocks authorization on certain endpoints in controllers

var app = builder.Build(); //End of config, initiate app. 

if (!app.Environment.IsDevelopment())	//If app is not in dev mode (prod mode)
{				
	app.UseExceptionHandler("/Home/Error"); //If something crashes, show user friendly error page
	app.UseHsts(); //Force browser to only communicate over HTTPS in production - Security measure : In dev you want to see full error pages so you can debug. In prod you dont want users seeing it. 
}

app.UseHttpsRedirection(); //Force all HTTP requests to HTTPS. Security measurte - Ensures traffic is encrypyed even if interceptted.
app.UseStaticFiles(); //ASP.NET looks for wwwroot folder on default (where js, css, imgs etc are)
app.UseRouting(); //ASP.NET to start matching incoming URLS to routes. Has to be called before authen and author so routing is figured out before auth checks occur
app.UseAuthentication(); // Middleware. Read cookie, check validation figure out who the user is 
app.UseAuthorization(); //Checks that user is allowed to access what theyre requesting

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Auth}/{action=Login}/{id?}");
	//Set default route if someone visits localhost:5144 with or without more in the url (id etc) redirects to login page first.


app.Run();

