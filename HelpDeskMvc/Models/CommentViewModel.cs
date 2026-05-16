namespace HelpDeskMvc.Models;

	public class CommentViewModel
	{
		public string Content { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } 
		public string UserName { get; set; } = string.Empty;
	}
