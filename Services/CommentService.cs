using HelpDeskApi.Data;
using HelpDeskApi.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace HelpDeskApi.Services
{
	public class CommentService
	{
		private readonly AppDbContext _context;

		public CommentService(AppDbContext context)
		{
			_context = context;
		}
					//Promises to a return type of Comment(from Comment.cs)
		public async Task<Comment> AddComment(string content, int ticketId, int userId)
		{		
			var comment = new Comment	//New comment object
			{
				Content = content,
				TicketId = ticketId, //Build shape, matches Comment.cs
				UserId = userId
			};
				//access Comments table, add comment object.
			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();
			return comment; //Return 
		}
	}
}