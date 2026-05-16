namespace HelpDeskMvc.Models;

public class TicketDetailViewModel
{
	public int Id { get; set; } = 0;
	public string Title { get; set; } = string.Empty;
	public string Status { get; set; } = string.Empty;
	public string Priority { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
}
