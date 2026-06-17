using TaskFour.Types;

namespace TaskFour.Api
{
	public class SetUsersStatus : IApiRequest
	{
		public class Request
		{
			public int Status { get; set; }
			public int[] Ids { get; set; } = null!;
		}

		[ApiRequestMethod("PATCH")]
		public virtual async Task RespondAsync(HttpContext httpContext)
		{
			string? response = httpContext.Session.GetInt32("userId") is not int userId || Task4Context.Instance.Users.Find(userId) is not Db.User user || user.Status != 1
				? $"Only active users can set other users status" : null;

			Request? request = await httpContext.Request.ReadFromJsonAsync<Request>();
			response ??= request is null || request.Ids.Length == 0 ? $"Not enough users' id to set status" : null;
			if (response is not null)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsJsonAsync(response);
				return;
			}

			foreach (int id in (request ?? throw new NullReferenceException()).Ids)
			{
				Task4Context.Instance.Users.Find(id)?.Status = request.Status;
			}

			Task4Context.Instance.SaveChanges();
			httpContext.Response.StatusCode = 200;
			return;
		}
	}
}
