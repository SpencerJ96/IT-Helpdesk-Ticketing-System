using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using HelpDeskMvc.Models;



namespace HelpDeskMvc.Controllers
{
	[Authorize]
	public class TicketController : Controller
	{
		private readonly IHttpClientFactory _HttpClientFactory; 

		public TicketController(IHttpClientFactory httpClientFactory)
		{
			_HttpClientFactory = httpClientFactory;
		}

			//Instead of manually adding the JWT token to every single API request.
			// we can call this method and get a client that already has a token attached
		private HttpClient GetAuthenticatedClient()
		{				 //Create a fresh client using the config labelled helpdeskapi in program.cs (Just a base address)
			var client = _HttpClientFactory.CreateClient("HelpDeskApi");
						//Access the user property from controller to see the cookie claims. Find the first lablled "JwtToken" if null return safely
			var token = User.FindFirst("JwtToken")?.Value;
			//If token isnt null (is present)
			if (token != null)
				//access the clients DefaultRequestHeaders auth property. Set its new value to "Bearer" <token>. What the API expects to see
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				//Return client instance 
			return client;
		}

		public async Task <IActionResult> Index()
		{
			var client = GetAuthenticatedClient();
			var response = await client.GetAsync("api/ticket");

			if (!response.IsSuccessStatusCode)
				return View (new List<TicketViewModel>());

			var json = await response.Content.ReadAsStringAsync();
			var tickets = JsonSerializer.Deserialize<List<TicketViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return View(tickets);
		}

		public async Task <IActionResult> Details (int id)
		{
			var client = GetAuthenticatedClient();
			var response = await client.GetAsync($"api/ticket/{id}");

			if (!response.IsSuccessStatusCode)
				return RedirectToAction("Index");

			var json = await response.Content.ReadAsStringAsync();
			var ticket = JsonSerializer.Deserialize<TicketDetailViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			return View(ticket);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View(new CreateTicketViewModel());
		}
		
		[HttpPost]
		public async Task <IActionResult> Create (CreateTicketViewModel model)
		{
			var client = GetAuthenticatedClient();

			var json = JsonSerializer.Serialize(model);

			var ticket = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("api/ticket", ticket);

			if (!response.IsSuccessStatusCode)
				return View(model);

			return RedirectToAction("Index");
		}
		
	}
}