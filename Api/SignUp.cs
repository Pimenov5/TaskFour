using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("POST")]
	public class SignUp : IApiRequest
	{
		public class Request
		{
			public string Name { get; set; } = null!;
			public string Email { get; set; } = null!;
			public string Password { get; set; } = null!;
			public string RepeatPassword {  get; set; } = null!;
		}

		public async Task<(int?, object?)> RespondAsync(HttpContext httpContext)
		{
			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			string? response = request is null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) 
				|| string.IsNullOrEmpty(request.RepeatPassword) ? "Name, email and both passwords password cannot be empty" : request.Password != request.RepeatPassword ? "Passwords are not equal" : null;
			/*
			Db.User? user = request is null ? null : Task4Context.Instance.Users.FirstOrDefault((Db.User user) => user.Email == request.Email);
			response ??= user is null ? null : $"User {user.Email} already exists";
			*/
			if (response is not null || request is null)
			{
				return (400, response);
			}

			Db.VerifyGuid? verifyGuid = null;
			string? strGuid;
			Db.User user = new() { Name = request.Name, Email = request.Email, Password = request.Password };


			using var transaction = Task4Context.Instance.Database.BeginTransaction();
			try
			{
				Task4Context.Instance.Users.Add(user);
				Task4Context.Instance.SaveChanges();

				strGuid = Guid.NewGuid().ToString();
				verifyGuid = new() { UserId = user.Id, Guid = strGuid };
				Task4Context.Instance.VerifyGuids.Add(verifyGuid);
				Task4Context.Instance.SaveChanges();

				transaction.Commit();
			}
			catch
			{
				strGuid = null;
				Task4Context.Instance.Users.Remove(user);
				if (verifyGuid is not null)
					Task4Context.Instance.VerifyGuids.Remove(verifyGuid);

				transaction.Rollback();
				throw;
			}

			response = $"/api/verify?userId={user.Id}&guid={strGuid}";
			return (200, response);
		}
	}
}