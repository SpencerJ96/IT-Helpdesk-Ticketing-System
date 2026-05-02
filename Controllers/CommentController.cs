using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{	
	[ApiController]		//ticketId whatever ticketid theyre trying to add a comment too
	[Route("api/ticket/{ticketId}/comment")]
	[Authorize] //Only those with authorization can hit this endpoint
	public class CommentController : ControllerBase
	{
		private readonly CommentService _commentService; //Create instance of comment service and empty slot for it 
		
		public CommentController(CommentService commentService)
		{
			_commentService = commentService; //Fill empty slot with instance of commentService file. Now have access to it
		}

		[HttpPost] // Post that reads JSON body and maps it to request addcommentrequest record is the shape(type)
		public async Task<IActionResult> AddComment(int ticketId, [FromBody] AddCommentRequest request)
		{		//		Controllerbase from ASP.NET gives us access to validated JWT token claims
				//		ASP.NET hands us that unpacked token. "User" Is a property. Find the first ClaimType that matches
			var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
			var comment = await _commentService.AddComment(request.Content, ticketId, userId);
			return Ok(comment);
		}
	}

	public record AddCommentRequest(string Content);
}