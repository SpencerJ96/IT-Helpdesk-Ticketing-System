

namespace HelpDeskMvc.Models;

public class TicketViewModel
{
	public string Title { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string Priority { get; set; } = string.Empty;
	public int Id { get; set; } = 0;
	public int CommentCount { get; set; } = 0;
	public DateTime CreatedAt { get; set; } 
}