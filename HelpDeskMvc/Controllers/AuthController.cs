using Microsoft.AspNetCore.Authentication; //Gives us methods that create and destroy cookies on login and log out 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc; //Gives us controller, get post view etc 
using System.Net.Http.Headers; //Gives us AuthenticationHeaderValue used to attach the jwt token to api requests. 
using System.Security.Claims; // gives us claim and claimtypes for building the cookie with user identity
using System.Text; //Encoding - converting request body to bytes 
using System.Text.Json; //Gives us JsonSerializer for converting c# objects to JSON and back. 
using HelpDeskMvc.Models;

namespace HelpDeskMvc.Controllers
{			
	public class AuthController : Controller
	{						//Create an instance of a scope into HelpDeskApi (from program.cs)
		private readonly IHttpClientFactory _httpClientFactory;

		public AuthController(IHttpClientFactory httpClientFactory)
		{					//Set the instance straight away in constructor so i cannot be changed again for security 
			_httpClientFactory = httpClientFactory;
		}

	[HttpGet]
	public IActionResult Login()
		{
			return View();
		}

	[HttpPost]
	public async Task <IActionResult> Login (LoginViewModel model)
		{					//Create instance of HelpDeskApi
			var client = _httpClientFactory.CreateClient("HelpDeskApi");

							//Turn the c# object to a json string ready to send to the API
			var json = JsonSerializer.Serialize(new { email = model.Email, password = model.Password });
							//Package the JSON String. Encode the text into bytes. Tell API what format the body is. "StringContent" C# Class. Request body as string
			var content = new StringContent(json, Encoding.UTF8, "application/json");

								 //Wait for client to run login method passing in JSON string 
			var response = await client.PostAsync("api/auth/login", content);

			//If the response does not return successful status code. Return login page
			if (!response.IsSuccessStatusCode)
			return View(model);


				//Wait for response to complete its post. Take the .Body from the response (Token) and read it as a string
			var responseBody = await response.Content.ReadAsStringAsync();
							//Turn the body from a string to a c# object - called <TokenResponse> 
							//Pass in responseBody, create fresh instance of the class options with case insensitivity turned ON. so "token" in the JSON maps to Token in c# class regardless of casing.
			var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						//claims is a list object that contains two Claim Objects.
			var claims = new List<Claim>
			{		//Two items inside Claim, the type "ClaimType" and where it is
				new Claim(ClaimTypes.Email, model.Email),
						//jwt label, tokenData the JWT string the api sent back "!" null preventer
				new Claim("JwtToken", tokenData!.Token)	
			};

					//Wraps the claims into ClaimsIdentity an ASP.NET class to package ID. Tell it to use the "cookies" auth scheme
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			//Wrap that ID into a format that ASP.NET can use to represent the currently logged in user 
			var principal = new ClaimsPrincipal(identity);


				  //HttpContext Property from controller. Represents the current HTTP Request/Response (This case a post)
				  //Call SignInAsync method. Creates the browser cookie using "Cookies" scheme and passes in the principal (email and JWT Token)
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				//After successful login, redirect to ticketControllers Index method which will be the ticket list page.
			return RedirectToAction("Index", "Ticket");
		}



		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task <IActionResult> Register(RegisterViewModel model)
		{				//Create instance of API Client 
			var client = _httpClientFactory.CreateClient("HelpDeskApi");

								//Turn the login string to a JSON string that fits user model in DB
			var json = JsonSerializer.Serialize( new { name = model.Name, email = model.Email, password = model.Password});

											//Turn that string into bytes. Tell it the language is JSON
			var content = new StringContent(json, Encoding.UTF8, "application/json");

										//Await api to complete register method passing in content
			var response = await client.PostAsync("api/auth/register", content);
				//If response is not a successful status code, return register model
			if (!response.IsSuccessStatusCode)
				return View(model);
			//if successful redirect to "login" page
			return RedirectToAction("Login");
		}
	}
}