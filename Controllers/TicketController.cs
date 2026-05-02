using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{
	[ApiController] //ASP.NET Framework attribute. Applies specifically to the class below it
	[Route("api/[controller]")]
	[Authorize] //Tells ASP.NET that every endpoint in this class requires a valid JWT Token
	public class TicketController : ControllerBase
	{				//Type(THIS CLASS)  //Call it this  (the slot)
		private readonly TicketService _ticketService;
							//Take this Class
		public TicketController(TicketService ticketService)
		{	//Fill this slot	//With an instance of this classd
			_ticketService = ticketService;
		}
			//^^^ LETS OUR Service and Controller speak to each other. Can call methods from Service (Create ticket etc)


			//Any tickets SENT to the db hit this endpoint. TDLR; Tickets hit here, in the shape of our record map it to request. Unpack token, string to int save it as userId.
			//Wait for createticket in ticketservice to complete, passing in request properties (title,desc and userid)  send ok server and ticket back

			//<IActionResult> from ASPNET.MVC + [FromBody]
	[HttpPost] //Promises to return a server status  CreateTicketRequest = Type(Our record). (Shape of JSON body) Read from Body automatically, Map it to request
	public async Task <IActionResult> CreateTicket ([FromBody] CreateTicketRequest request)
		{	
			//Turn The ID from a string into an int. 
			//ControllerBase User property, asp.net unpacked data token. Find the first ClaimType called NameIdentifer "!" it will never be null and give me its value.
			//Claim was set when token was generated in AuthService, So its the ID of whoever logged in. Tickets can be tracked via user token
			//Cannot be faked by sending a different ID in the request body. If userId was in the request body malicious users could fake ID's
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			//Wait for ticketService to finish its CreateTicket method passing in the title and desc values from the request the user sent us  
			var ticket = await _ticketService.CreateTicket(request.Title, request.Description, userId);
			return Ok(ticket); // Return Ok server response and Ticket object. 
		}


	//Any fetch requests to the DB hit this endpoint and pass in an {id} param "/api/ticket/{id}. ASP.net does heavy lifting
	[HttpGet("{id}")]	//Passes ID to getTicket Method
	public async Task<IActionResult> GetTicket(int id)
		{				//Await TicketService to run gettickedbyid passing in URL Id to method
			var ticket = await _ticketService.GetTicketById(id);

			if (ticket == null)
				return NotFound();

			return Ok(ticket);
		}


	//Any fetch requests hitting this end point MUST HAVE admin authorization. 
	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetAllTickets()
		{	//Wait for ticketservice to complete getalltickets and return ok and tickets list 
			var tickets = await _ticketService.GetAllTickets();
			return Ok(tickets);
		}

	//Any update requests go through this. They have to have admin clearance to his this endpoint
	[HttpPut("{id}")]
	[Authorize (Roles = "Admin")]				  //Pass id and map the body of the json (ticket update) to request. UpdateTicketRequest is the type (Shape of it)
	public async Task <IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
		{					//wait for updateTicket in ticketservice take the request status and priortiy 
			var ticket = await _ticketService.UpdateTicket(id, request.Status, request.Priority);
			//If ticket not found return 404 null
			if (ticket == null)
				return NotFound();
			//Send back updated ticket to client 
			return Ok(ticket);
		}
	
	}

	public record CreateTicketRequest(string Title, string Description);
	public record UpdateTicketRequest(string Status, string Priority);
}