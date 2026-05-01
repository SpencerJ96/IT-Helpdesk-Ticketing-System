using HelpDeskApi.Data;
using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Services
{
	public class TicketService
	{
		private readonly AppDbContext _context;

		public TicketService(AppDbContext context)
		{
			_context = context;
		}
	


	public async Task<Ticket> CreateTicket (string title, string description, int userId)
		{
			var ticket = new Ticket
			{
				Title = title,
				Description = description,
				UserId = userId
			};

			_context.Tickets.Add(ticket);
			await _context.SaveChangesAsync();
			return ticket;
		}


	public async Task <Ticket?> GetTicketById (int id)
		{
			return await _context.Tickets
			.Include (t => t.User)
			.Include (t => t.Comments)
			.FirstOrDefaultAsync (t => t.Id == id);
		}


	public async Task <List<Ticket>> GetAllTickets()
		{
			return await _context.Tickets
			.Include(t => t.User)
			.Include (t => t.Comments)
			.ToListAsync();
		}


	public async Task <Ticket?> UpdateTicket (int id, string status, string priority)
		{
			var ticket = await _context.Tickets.FindAsync(id);

			if (ticket == null)
				return null;

			ticket.Status = status;
			ticket.Priority = priority;

			await _context.SaveChangesAsync();
			 return ticket;
		}
}
}