namespace HelpDeskMvc.Models
{
	public class LoginViewModel
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}


	public class RegisterViewModel
	{
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}

	public class TokenResponse
	{
		public string Token { get; set; } = string.Empty;
	}
}