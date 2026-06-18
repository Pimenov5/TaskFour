using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("POST")]
	public class SignIn : IApiRequest
	{
		public class Request
		{
			public string Email { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
		}

		public async Task<(int?, object?)> RespondAsync(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			if (request is null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
			{
				return (400, "Email and password cannot be empty");
			}

			Db.User? user = Task4Context.Instance.Users.FirstOrDefault((Db.User user) => user.Email == request.Email && user.Password == request.Password);
			string? response = user is null ? "User not found" : user.Status switch
			{
				0 => null, // "Not verified user cannot sign in",
				1 => null,
				2 => "Blocked user cannot sign in",
				_ => throw new("User Status unknown value: " + user.Status.ToString())
			};

			if (response is not null || user is null)
			{
				return (401, response);
			}

			Db.SignInTimestamp timestamp = new() { UserId = user.Id, Timestamp = DateTime.UtcNow.ToOADate() };

			using var transaction = Task4Context.Instance.Database.BeginTransaction();
			try
			{
				Task4Context.Instance.SignInTimestamps.Add(timestamp);
				Task4Context.Instance.SaveChanges();

				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}

			httpContext.Session.SetInt32("userId", user.Id);
			httpContext.Session.SetString("userEmail", user.Email);
			httpContext.Session.SetString("userPassword", user.Password);
			await httpContext.Session.CommitAsync();

			httpContext.Response.Redirect($"/admin.html");

			return (null, null);
		}
	}
}