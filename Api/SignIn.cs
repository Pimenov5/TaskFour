using TaskFour.Types;

namespace TaskFour.Api
{
	public class SignIn : IApiRequest
	{
		public class Request
		{
			public string Email { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
		}

		[ApiRequestMethod("POST")]
		public async Task RespondAsync(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			if (request is null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsJsonAsync<string>("Email and password cannot be empty");
				return;
			}

			Db.User? user = Task4Context.Instance.Users.FirstOrDefault((Db.User user) => user.Email == request.Email && user.Password == request.Password);
			string? response = user is null ? "User not found" : user.Status switch
			{
				0 => "Not verified user cannot sign in",
				1 => null,
				2 => "Blocked user cannot sign in",
				_ => throw new("User Status unknown value: " + user.Status.ToString())
			};

			if (response is not null || user is null)
			{
				httpContext.Response.StatusCode = 401;
				await httpContext.Response.WriteAsJsonAsync(response);
				return;
			}

			httpContext.Session.SetInt32("userId", user.Id);
			httpContext.Session.SetString("userEmail", user.Email);
			httpContext.Session.SetString("userPassword", user.Password);
			await httpContext.Session.CommitAsync();

			httpContext.Response.Redirect($"/admin.html");

			Db.SignInTimestamp timestamp = new() { UserId = user.Id, Timestamp = DateTime.UtcNow.ToOADate() };
			Task4Context.Instance.SignInTimestamps.Add(timestamp);
			Task4Context.Instance.SaveChanges();

			return;
		}
	}
}
