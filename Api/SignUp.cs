using TaskFour.Types;

namespace TaskFour.Api
{
	public class SignUp : IApiRequest
	{
		public class Request
		{
			public string Name { get; set; } = null!;
			public string Email { get; set; } = null!;
			public string Password { get; set; } = null!;
			public string RepeatPassword {  get; set; } = null!;
		}

		[ApiRequestMethod("POST")]
		public async Task RespondAsync(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			string? response = request is null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) 
				|| string.IsNullOrEmpty(request.RepeatPassword) ? "Name, email and both passwords password cannot be empty" : request.Password != request.RepeatPassword ? "Passwords are not equal" : null;

			Db.User? user = request is null ? null : Task4Context.Instance.Users.FirstOrDefault((Db.User user) => user.Email == request.Email);
			response ??= user is null ? null : $"User {user.Email} already exists";
			if (response is not null || request is null)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsJsonAsync(response);
				return;
			}

			user = new() { Name = request.Name, Email = request.Email, Password = request.Password };
			Task4Context.Instance.Users.Add(user);
			Task4Context.Instance.SaveChanges();

			Db.VerifyGuid verifyGuid = new() { UserId = user.Id, Guid = Guid.NewGuid().ToString() };
			Task4Context.Instance.VerifyGuids.Add(verifyGuid);
			Task4Context.Instance.SaveChanges();

			httpContext.Response.StatusCode = 200;
			await httpContext.Response.WriteAsJsonAsync($"/api/verify?userId={user.Id}&guid={verifyGuid.Guid}");
			return;
		}
	}
}
