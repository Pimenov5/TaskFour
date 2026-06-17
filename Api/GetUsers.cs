using TaskFour.Types;

namespace TaskFour.Api
{
	public class GetUsers : IApiRequest
	{
		public class VisibleUser
		{
			public int Id { get; set; }
			public string Name { get; set; } = null!;
			public string Email { get; set; } = null!;
			public string Status { get; set; } = null!;
			public string LastSignIn { get; set; } = null!;
		}

		public class Response
		{
			public List<VisibleUser> Users { get; set; } = [];
		}

		[ApiRequestMethod("GET")]
		public async Task RespondAsync(HttpContext httpContext)
		{
			List<Db.User> dbUsers = [..Task4Context.Instance.Users];
			Response response = new();
			response.Users.Capacity = dbUsers.Count;

			foreach (Db.User user in dbUsers)
			{
				double? oaDate = user.SignInTimestamps.Count == 0 ? Task4Context.Instance.SignInTimestamps.OrderByDescending((Db.SignInTimestamp timestamp) => timestamp.Timestamp)
					.FirstOrDefault((Db.SignInTimestamp timestamp) => timestamp.UserId == user.Id)?.Timestamp : user.SignInTimestamps.Last().Timestamp;

				response.Users.Add(new VisibleUser() { Id = user.Id, Name = user.Name, Email = user.Email, LastSignIn = oaDate is null ? "Not sign in" 
					: DateTime.FromOADate(oaDate ?? throw new NullReferenceException()).ToString() + " (UTC)", 
					Status = user.Status switch 
					{
						0 => "Not verified",
						1 => "Active",
						2 => "Blocked",
						_ => throw new($"User Status unknown value: {user.Status}")
					}
				});
			}

			httpContext.Response.StatusCode = 200;
			await httpContext.Response.WriteAsJsonAsync(response);
			return;
		}
	}
}
