namespace HelpDeskApi.Models
{
	public class Ticket
	{
		public int Id { get; set; }   //Different from User ID. 
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Status { get; set; } = "Open";
		public string Priority { get; set; } = "Low";
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


		//FOREIGN KEY (DATABASE): UserID is the number in the DB that links these two models together.
		//Every Ticket MUST have a USERID, it has to have come from somewhere
		public int UserId { get; set; }
		public User? User { get; set; } // NAVIGATION PROPERTY : Lets us access the object. (Name, email etc)
		public List<Comment> Comments {get; set; } = new();									// "?" May be null, Entity wont load this, only when we ask for it (EAGER LOADING)
			//EAGER LOADING : Telling Entity to load related objects in a single DB query rather than 2
				//E.G Tickets.Include(t => T.User)
	}			
}		 