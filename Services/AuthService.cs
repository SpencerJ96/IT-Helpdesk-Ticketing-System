using HelpDeskApi.Data;
using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HelpDeskApi.Services
{
	public class AuthService	
		//READONLY : Once inside a constructor they cannot be changed.	
	{	//Our DB Bridge : Models,Tables.   // Our JWT Config.
		private readonly AppDbContext _context;			//Establish two private READONLY Variables. 
		private readonly IConfiguration _configuration;
		
							//DEPENDENCY INJECTION 
		public AuthService(AppDbContext context, IConfiguration configuration)//Type Declration. "AppDbContext context" C# needs to know where/what its injecting here.
		{				
	
			_context = context;		//Store the injected values in the private fields so all methods in class can have access.
			_configuration = configuration;
		}


				//Promise to return string
		public async Task<string> Register(string name, string email, string password) //type annotation
		{
			 //Wait for DB to search .Users table. Find first element that satisfies a specific condition or return default value.
			var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
				   //If existingUser is not equal to null. (If existing user returns something)
				if (existingUser != null)
					return "User Already Exists";
			

			// Create fresh instance of the User Model from (User.cs)
			var user = new User
			{		//Object initialiser, set all properties on object creation
				Name = name,
				Email = email,
				PasswordHash = HashPassword(password), //Never store as plain text. Hash for security with hash method.
				Role = "User"
			};

			//Access DB User table and use add method passing in the User object just created
			_context.Users.Add(user);
			//Wait for the DB to save all changes to the DB
			await _context.SaveChangesAsync();
			//Return success message.
			return "User Registered Successfully";
		}

				//Runs everytime a user object is initialised on whatever "PasswordHash" value was. 
				// Password: hello 123 --> HashPassword(hello 123) --->BCrypt.Hashpassword(hello 123) scrambles. ---> Return the scramble --> PasswordHash = "the scramble".
				//Save to DB - plain text passwrod never touches the DB
				
		private string HashPassword(string password) //Pass in a string called password
		{
			return BCrypt.Net.BCrypt.HashPassword(password); //Call Brypt NuGet packages built in hashing method.
					 //Benefits of BCrypt; Slow on Purpose, making brute force harder.
					 //Adds a "salt" automatically (a random value added to the password to prevent two users with the same password returning the same hashed password)
		}
	
		public async Task <string> Login(string email, string password)
		{			//Find User by email, if not found return user not found
			 var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
				 if (user == null)
			 		return "User not found";
				//Verify the password against stored hash, if wrong "!" return invalid password
				//Hashing cannot be unscrambled to the original, so plain text cant be compared - The verify method:
					//Take the password the user typed in, hash it the same way and check if it matches, this returns a boolean ; True [password matches], false [it doesnt.]
				 if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
				 	return "Invalid Password";

				//If credentials are all valid - generate and return a JWT for the user. 
				return GenerateJwtToken(user);
		}



		private string GenerateJwtToken(User user)
		{
			//SymmetricSecurityKey from Microsoft.Identitymodel.tokens 
			//Holds Cryptographic key information. We pass in raw bytes of secret key and it wraps them into an object
			//that JWT Library knows how to use for signing 
			//UTF8 is text coding standard for how text gets converted to bytes 
			//GetBytes is a method of UTF8 takes string converts it into an array of bytes
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
				_configuration["JwtSettings:SecretKey"]!));


			//Signing Credentials is a class from JWT Library
			//Passing in the key (our secret key in cryptographic form), The algorithm used to sign the token
			//HmacSha256 Is indsutry standard for signing JWT Tokens
			//SecurityAlogrithms.HMACSHA256 is a constant from JWT Library, represents the name of the algorithm as a string.
			// Key = Unique stamp, Hmacshag256 = technique to press stamp, Credentitals = combo of both to stamp token.
			//Combine our secret Key with hmacsha256 signing algorithim to create credentials we'll use to digitally sign the token.
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



			//Creating a array of Claims. Claims = piece of info about the user baked into the token
			//Takes the type and what kind of info
			//ClaimTypes is a class from JWT Library - standard predefined claim type names. So other systems can use tokens and understand
			// Instead of : Request comes in → hit database → who is this person? → are they an admin? → allow/deny
			// Request comes in → read token claims → role = "Admin" → allow
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role)
			};

			// Create new Token instance and pass everything in 

			var token = new JwtSecurityToken(
				issuer: _configuration["JwtSettings:Issuer"], //Who created token (from appsettings.json)
				audience: _configuration["JwtSettings:Audience"], // Who is it for ("")
				claims: claims,						   //The user info array we built above
				expires: DateTime.UtcNow.AddMinutes(   //Current UTC Time plus 60 mins - Take current UTC time and add 60 mins
					Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),	//Value from appsettings.json comes back as a string "60" so we conver it to a number before passing it 
						signingCredentials: credentials
			);

					//JWT SecurityTokenHandler class that handles JWT Tokens. 
												//Takes our token object and converts it to an actual JWT string
												//That gets sent back to user on login. 
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}