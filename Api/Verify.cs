using TaskFour.Types;

namespace TaskFour.Api
{
	public class Verify : IApiRequest
	{
		[ApiRequestMethod("GET")]
		public async Task RespondAsync(HttpContext httpContext)
		{
			int userId = 0;
			const string EMPTY_PARAMS = "Parameters userId and guid cannot be empty";
			const string CANNOT_VERIFY = "Cannot verify email";

			string? response = !httpContext.Request.Query.ContainsKey("userId") || !int.TryParse(httpContext.Request.Query["userId"], out userId)
				|| !httpContext.Request.Query.ContainsKey("guid") ? EMPTY_PARAMS : null;
			string? guid = httpContext.Request.Query["guid"];

			response ??= string.IsNullOrEmpty(guid) ? EMPTY_PARAMS : Task4Context.Instance.Users.Find(userId) is not Db.User user ? CANNOT_VERIFY 
				: user.Status switch { 0 => null, 1 => "User already verified", 2 => "Blocked user cannot verify email", _ => $"User Status unknown value: {user.Status}"};

			response ??= Task4Context.Instance.VerifyGuids.FirstOrDefault((Db.VerifyGuid verifyGuid) => verifyGuid.UserId == userId && verifyGuid.Guid == guid) is null
				? CANNOT_VERIFY : null;
			if (response is not null)
			{
				httpContext.Response.StatusCode = 400;
				await httpContext.Response.WriteAsJsonAsync(response);
				return;
			}

			(Task4Context.Instance.Users.Find(userId) ?? throw new NullReferenceException()).Status = 1;
			Task4Context.Instance.SaveChanges();

			httpContext.Response.Redirect("/sign-in.html");
			return;
		}
	}
}
