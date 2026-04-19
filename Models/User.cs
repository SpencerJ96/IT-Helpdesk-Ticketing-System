


namespace HelpDeskApi.Models //namespace tells c# where the file lives in the project -
 //use "using HelpDeskApi.Models" to access this class. 
{
	public class User //User is anyone who has an account on the system (USER, AGENT, ADMIN) controls what they can do on the system
	{
		public int Id { get; set; }  // Unique to each user. DB Uses this to differentiate between users
		public string Name { get; set;} = string.Empty; 
		public string Email { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty; //Never store real password - only scrambled (hashed) for security.
		public string Role { get; set; } = "User"; // Default to "USER" Agent/Admin has to be manually done.
	}
}