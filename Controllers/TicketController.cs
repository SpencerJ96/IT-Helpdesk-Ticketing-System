using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TicketController : ControllerBase
	{
		private readonly TicketService _ticketService;

		public TicketController(TicketService ticketService)
		{
			_ticketService = ticketService;
		}


	[HttpPost]
	public async Task <IActionResult> CreateTicket ([FromBody] CreateTicketRequest request)
		{
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
			var ticket = await _ticketService.CreateTicket(request.Title, request.Description, userId);
			return Ok(ticket);
		}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetTicket(int id)
		{
			var ticket = await _ticketService.GetTicketById(id);

			if (ticket == null)
				return NotFound();

			return Ok(ticket);
		}

	[HttpGet]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetAllTickets()
		{
			var tickets = await _ticketService.GetAllTickets();
			return Ok(tickets);
		}

	[HttpPut("{id}")]
	[Authorize (Roles = "Admin")]
	public async Task <IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
		{
			var ticket = await _ticketService.UpdateTicket(id, request.Status, request.Priority);

			if (ticket == null)
				return NotFound();

			return Ok(ticket);
		}
	
	}

	public record CreateTicketRequest(string Title, string Description);
	public record UpdateTicketRequest(string Status, string Priority);
}