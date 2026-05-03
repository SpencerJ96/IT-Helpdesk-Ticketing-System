using HelpDeskApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Controllers
{
	[ApiController] //Tells ASP.NET this class is an api controller, gives us auto behaviours like reading json requests from bodies without manual conversion
	[Route("api/[controller]")]	//Set base URL for every endpoint. [Controller] is replaced by ASP.NET with the class name minus "Controller" So it becomes api/auth
	public class AuthController : ControllerBase //Our controller inherits from ControllerBase giving us reponse helper methods ok(), BadRequest() etc
	{
		//Make it a private variable only changeable once - Only things inside AuthController need to use this.

		private readonly AuthService _authService; //Create a private field (empty slot) of AuthService but dont fill it yet (call it _authservice)

		public AuthController(AuthService authService) //Passes in an instance of the class that ASP.NET creates and hands in 
		{	//Assign it immediately so it cannot be changed again for security 
			_authService = authService; //Assign _authService an instance of authService. Now every method in the class can see it. Our Register and login methods can now call _authService.Register()
		}


			//Attribute as a Post request called Register
		[HttpPost("register")] 
		public async Task<IActionResult> Register ([FromBody] RegisterRequest request)//Async task that returns a HTTP response. Frombody = read body request and map it to request
		{
			var result = await _authService.Register(request.Name, request.Email, request.Password); //Wait for register to complete, take properties from body

			if (result == "User Already Exists") //If register method in authservice returns that 
			return BadRequest(result); //Send back badrequest

			return Ok(result);
		}
		//attribute for controller, called login
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var result = await _authService.Login(request.Email, request.Password);

			if (result == "User not found" || result == "Invalid Password") //Return failed credentials
				return Unauthorized(result);
				
				//Return JWT token string from login, wrap it in an unlabelled anon object with property called token so client can understand better
				return Ok (new { token = result });
		}

		//Records can hold data without having to define it as explicity as a class.
		//Records auto-generate the properties from what you put in the brackets.
		public record RegisterRequest(string Name, string Email, string Password);
		public record LoginRequest(string Email, string Password);


	}
}	

	