

namespace HelpDeskApi.Models
{
	public class Comment
	{
		public int Id { get; set; }
		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public int TicketId { get; set; }
		public Ticket? Ticket { get; set; } //NAVIGATION PROPERTY: "?" Nullable. Eager Load it with .Include() when full user data is needed.

		public int UserId { get; set; }
		public User? User { get; set; } // NAVIGATION PROPERTY: "?" Nullable. Eager Load it with .Include() when full user data is needed.

		//FOREIGN KEYS : TicketId (From ID in ticket. Every Ticket HAS to have an ID [WHERE IT CAME FROM])
		//             : UserId [WHO IT CAME FROM]
		//EAGER LOADING: Fetch both of these from the DB at the sametime
	}
}