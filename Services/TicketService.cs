using HelpDeskApi.Data;
using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Services
{
	public class TicketService
	{
		private readonly AppDbContext _context; //Appdbcontext class name, _context slo

		public TicketService(AppDbContext context) // pass in appdbcontext as a type and context as the name 
		{	//fill the empty slot with an instance of the class (our db)
			_context = context;
		}
	

											//pass in 3 variables 
	public async Task<Ticket> CreateTicket (string title, string description, int userId)
		{		//Establish ticket as a new Instance of the Ticket object
			var ticket = new Ticket
			{
				Title = title,	
				Description = description,	//Build Tickets shape. Matching DB shape
				UserId = userId
			};
				//Access dbs, Ticket table use add method and pass in ticket object.
			_context.Tickets.Add(ticket);
			await _context.SaveChangesAsync(); //Wait for db to run its save 
			return ticket; // return ticket object
		}

						// May return a ticket "?" 
	public async Task <Ticket?> GetTicketById (int id) // pass in int called id 
		{	//Wait for db to fetch Tickets table . EAGER LOADING. Whilst fetching it return the User, and Comments from the table
			return await _context.Tickets
			.Include (t => t.User)
			.Include (t => t.Comments)
			.FirstOrDefaultAsync (t => t.Id == id); //Find the first t.id that is equal to id
												//api/ticket/5, 5 is passed into the method as int id 
		}

						//Returns a <List> of <Ticket> Items
	public async Task <List<Ticket>> GetAllTickets()
		{			//Wait for Db to fetch Tickets table. EAGLER LOADING. include user, comments,
			return await _context.Tickets
			.Include(t => t.User)
			.Include (t => t.Comments)
			.ToListAsync();	//<-- Fires the db query and collects results into <List<Ticket>> 
		}


	public async Task <Ticket?> UpdateTicket (int id, string status, string priority)
		{		//Wait for db table fetch. Findasync EFW method that looks up recrod by primary key (id) in this case.
							//Faster than using firstordefaultasync when searching by primary key specifically
			var ticket = await _context.Tickets.FindAsync(id);
				//If ticket == null, it doesnt exist by id return null
			if (ticket == null)
				return null;
			//Update ticket.status/priorirty property by whatever status was passed into method
			ticket.Status = status;
			ticket.Priority = priority;

			await _context.SaveChangesAsync(); // wait for DB to save
			 return ticket; // return ticket
		}
}
}