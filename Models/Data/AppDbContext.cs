using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Data

{
//AppDbContext is the bridge between C# code and the SQL Server database. It inherits from Entities DbContext. ": DbContext"
//This gives it all the logic to connect to the DB and run queries, save data etc. 
//We Register it with our app settings to tell it what DB To connect too, and call it options (dbContextOptions<AppDbContext> options )
//Then we send this back to the parent via base() 
//Now anything DB related goes through this so we never have to write SQL ourselves. 
//Our PROPERTIES  represent the table SQL will create [DbSet] <User> (Our User model) call it Users. Allow this data to be fetched and edited.

	public class AppDbContext : DbContext //AppDbContext inherits classes from DbContext (ENTITY FRAMEWORK ITEM: Contains all logic needed to connect to DBS run queries, save data etc)
	{		
		//CONSTRUCTOR: Receives a settings container [ENTITY FRAMEWORK](DBContextOptions) configured for AppDbContext
		// And passes them up to the parent DbContext through base() so it can establish a DB Connection.
		//The Constructors body is empty {} because all the work is done by the parent DbContext via base()
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

	//DbSet: Entity framework - Make this table:
	//Our DB Will make Three tables; User, Tickets & Comments. Each shaped like their matching model from "using HelpDeskApi.Models;"
	public DbSet<User> Users {get; set;} 
	public DbSet<Ticket> Tickets { get; set; }
	public DbSet<Comment> Comments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Comment>()
			.HasOne(c => c.User)
			.WithMany()

			.HasForeignKey( c => c.UserId)
			.OnDelete(DeleteBehavior.Restrict);				//	USER DELETED ---> TICKET DELETES + COMMENT DELETES (simultaneously)
               //                        										//	 ↓
           //               											    TICKET ALSO DELETES COMMENT
			//---BUG--- comment is a child of both ticket (Ticketid) and User (userid). Ticket is a child of User. 
			// When user is deleted, comment is hit twice with deletion due to cascade deletion. SQL doesnt give priority to either and flags an error.
			// BUG FIXED!!: We use the OnModelCreating method from EF and overide it, adding additional rules
			//We tell EF to tell the DB <Comment> has one user, that user has many comments.
			//We highlight its foreign Key USERID and alter the deletion behaviour to restrict deletion of comments via UserID.
			// Therefore the cascade can no longer occur on this path and rather flows like
			//USER DELETED --> TICKET DELETED ---> COMMENT DELETED

		//BUG FIX - Here we're overiding the original OnModelCreating method from EF Inherited from " : dbContext"
		// Originally, when a user was deleted :
		//  			User deleted - ticket deleted - Comments Deleted
		//  	 AT THE SAME TIME SQL SEES THIS AND REFUSES TO CARRY ON (Two deletions on one table, who gets prio?)
		//   			User deleted - comments deleted directly (Via UserId On Comment)
		// 			--	This is due to comments 2 foreign keys: TicketId, and UserId causing a cascade delete.	--
		//	THE FIX: 
		// Restrict the user deletion path by telling EF (which then tells SQL) do not delete comments via the c.UserID
		}
	
	public void SeedData()
		{
			if (!Users.Any(u => u.Role == "Admin"))
			{
				Users.Add(new User
				{
					Name = "Admin",
					Email = "admin@helpdesk.com",
					PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"), //For portfolio/learning putting the password here is ok. In actual production the password would be stored in envrionemntal variables or a secrets manager.
					Role = "Admin"
				});
				SaveChanges();
			}
		}

	}
}

//Whenever An API Occurs, a new instance of AppDBContext is created. Our constructor runs automaically.
//It receives a settings container (DbContextOptions) an entity framework class that holds things like which DB and where, we call it options.
//These settings are then passed up to the parent "DbContext" via base(). DbContext then uses that to open the connection
// AppDbContext then inhertits DbContext's ability ":" to run the queries and save data. 