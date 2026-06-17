using TaskFour.Types;

namespace TaskFour.Api
{
	public class CanAdmin : IApiRequest
	{
		public class Response
		{
			public int Id { get; set; }
			public string Name { get; set; } = null!;
			public string Email { get; set; } = null!;
		}

		[ApiRequestMethod("GET")]
		public async Task RespondAsync(HttpContext httpContext)
		{
			if (httpContext.Session.GetInt32("userId") is int userId && Task4Context.Instance.Users.Find(userId) is Db.User user && user.Status == 1)
			{
				httpContext.Response.StatusCode = 200;

				Response response = new() { Id = user.Id, Name = user.Name, Email = user.Email };
				await httpContext.Response.WriteAsJsonAsync(response);
			}
			else
				httpContext.Response.StatusCode = 401;

			return;
		}
	}
}