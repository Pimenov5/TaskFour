using TaskFour.Types;

namespace TaskFour.Api
{
	[ApiRequest("POST")]
	public class SignOut : AdminApiRequest, IApiRequest
	{
		protected override async Task<(int?, object?)> GetStatusCode(HttpContext httpContext)
		{
			httpContext.Session.Remove("userId");
			httpContext.Session.Remove("userEmail");
			httpContext.Session.Remove("userPassword");

			await httpContext.Session.CommitAsync();
			httpContext.Response.Redirect("/sign-in.html");
			return (null, null);
		}
	}
}
